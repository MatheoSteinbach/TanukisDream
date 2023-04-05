using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Speed")]
    [SerializeField] float moveSpeed = 300f;
    
    [Header("Attack")]
    [SerializeField] Transform attackPosition;
    [SerializeField] float distanceToAttack = 0.4f;
    [SerializeField] private Transform attackPivot;
    [SerializeField] float attackCooldown = 0.65f;

    [Header("Audio")]
    [SerializeField] AudioSource abilityChangeSFX;
    [SerializeField] AudioSource rangedAttackSFX;
    [SerializeField] AudioSource deathSFX;
    [SerializeField] AudioSource walkingSound;
    [SerializeField] AudioSource RollSound;
    [SerializeField] AudioSource HitSFX;

    [Header("Health")]
    [SerializeField] float currentHealth, maxHealth;
    [SerializeField] float knockbackDistance;

    [Header("Dash/Roll")]
    [SerializeField] float rollCooldown = 2f;
    [SerializeField] float rollSpeedEditor = 65f;

    [Header("Ranged Attack")]
    [SerializeField] GameObject projectile;
    [SerializeField] float specialCooldown = 2f;
    
    [Header("VFX")]
    [SerializeField] Animator hurtVignetteVFX;
    [SerializeField] Animator HitVFX;
    [SerializeField] Animator clawVFX;
    [SerializeField] private Animator abilityChangeVFX;
    [SerializeField] private Animator abilityRechargeVFX;
    [SerializeField] Animator spawnVFX;
    [SerializeField] Animator healVFX;
    [SerializeField] Animator dashVFX;

    [Header("UI")]
    //Icon
    [SerializeField] GameObject ability;
    [SerializeField] Sprite dashAbilityIcon;
    [SerializeField] Sprite rangedAbilityIcon;
    [SerializeField] Image icon;

    //Avatar
    [SerializeField] Image avatar;
    [SerializeField] Sprite normalAvatar;
    [SerializeField] Sprite dashAvatar;
    [SerializeField] Sprite rangedAvatar;

    //Health
    [SerializeField] Image hpBar;
    [SerializeField] Image[] extraHeartsFill;
    [SerializeField] Image[] extraHeartsBackground;
    [SerializeField] Image heart1Fill;
    [SerializeField] Image heart1Background;

    [Header("Camera")]
    [SerializeField] CameraShake camShake;
    [SerializeField] CinemachineVirtualCamera camBlackout;


    private enum State { Idle, Walking, Rolling, Attacking, Dead }
    private State state;

    private Rigidbody2D rb;
    private Vector3 movementInput;
    private Vector3 rollDir;
    private Vector3 lastMoveDir;
    private float rollTime;
    private float rollSpeed;

    private bool canMove = true;
    private AnimHandler animHandler;

    private float attackTime;
    private bool isAttacking = false;

    private bool attackBlocked;
    Vector2 pointerInput;
    private Collider2D collision2D;
    private PlayerWeapon weapon;

    private bool isSpecialOn = false;
    private float specialTime;
    private bool isDashOn = false;
    private bool hasHitOnDash = false;

    private int heartsCollected = 0;
    private bool abilityRecharged = false;
    private float walkTimer = 0f;
    private bool isDead = false;
    private CheckpointSystem checkpointSystem;

    private void Awake()
    {
        weapon = GetComponentInChildren<PlayerWeapon>();
        animHandler = GetComponent<AnimHandler>();
        rb = GetComponent<Rigidbody2D>();
        state = State.Idle;
        collision2D = GetComponent<Collider2D>();
    }

    private void Start()
    {
        checkpointSystem = GameObject.FindGameObjectWithTag("CheckpointSystem").GetComponent<CheckpointSystem>();
        transform.position = checkpointSystem.lastCheckpointPos;
    }
    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        pointerInput = Camera.main.ScreenToWorldPoint(mousePos);

        ManageHealth();
        PlayAnimations();
        HandleAbilityRechargeVFX();
        
        rollTime += Time.deltaTime;
        attackTime += Time.deltaTime;
        specialTime += Time.deltaTime;
        if (attackBlocked) { return; }
        weapon.PointerPosition = pointerInput;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                rb.velocity = Vector2.zero;
                break;

            case State.Walking:
                if(canMove)
                {
                    rb.velocity = movementInput * moveSpeed * Time.deltaTime;
                    walkTimer += Time.deltaTime;
                    if (walkTimer > 0.5f)
                    {
                        walkTimer = 0f;
                    }
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
                break;

            case State.Rolling:
                rb.velocity = rollDir * rollSpeed;
                break;

            case State.Attacking:
                if(canMove)
                {
                    rb.velocity = movementInput * (moveSpeed / 2) * Time.deltaTime;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
                break;
            default:
                break;
        }
    }

    public void AttackEnded()
    {
        if(!isDead)
        {
            animHandler.StopAttacking(lastMoveDir);
            isAttacking = false;
            canMove = true;

            if (movementInput.x != 0 || movementInput.y != 0)
            {
                state = State.Walking;
            }
            else
            {
                state = State.Idle;
            }
        }
    }

    private void PlayAnimations()
    {
        switch (state)
        {
            case State.Idle:
                if (movementInput != Vector3.zero && !isDead)
                {
                    state = State.Walking;
                }
                if(!isAttacking && !isDead)
                {
                    animHandler.PlayIdle(lastMoveDir);
                }
                break;

            case State.Walking:
                if (movementInput == Vector3.zero && !isDead)
                {
                    state = State.Idle;
                }
                if(!isAttacking)
                {
                    if(movementInput != Vector3.zero && !isDead)
                    {
                        lastMoveDir = movementInput;
                        animHandler.PlayWalk(lastMoveDir);
                    }
                }
               
                if (!walkingSound.isPlaying)
                {
                    walkingSound.Play();
                }
                break;

            case State.Rolling:
                RollSound.Play();
                // find angle to where is moving and dash to it ONCE
                var dir = lastMoveDir * 6000;
                float AngleRad = Mathf.Atan2(-dir.y + attackPivot.position.y, -dir.x + attackPivot.position.x);
                float AngleDeg = (180 / Mathf.PI) * AngleRad;
                attackPivot.rotation = Quaternion.Euler(0, 0, AngleDeg);
                dashVFX.Play("DodgeAnim");
                CheckColliders();
                
                camShake.ShakeCamera(5f, 0.2f);
                float rollSpeedDropMultiplier = 5f;
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                float rollSpeedMinimum = 50f;
                if (rollSpeed < rollSpeedMinimum)
                {
                    hasHitOnDash = false;
                    if (movementInput != Vector3.zero)
                    {
                        state = State.Walking;
                    }
                    else
                    {
                        state = State.Idle;
                    }
                }

                break;
            case State.Attacking:
                
                break;
            case State.Dead:
                
                    break;
            default:
                break;
        }
    }
    private void CheckColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, 1))
        {
            if (!hasHitOnDash)
            {
                EnemyHealth enemyHealth;
                if (enemyHealth = collider.GetComponent<EnemyHealth>())
                {
                    hasHitOnDash = true;
                    camShake.ShakeCamera(5f, 0.2f);
                    if (enemyHealth.GetComponent<AISpikeyAnimHandler>() != null)
                    {
                        enemyHealth.GetHit(100f, transform.gameObject);
                    }
                    else
                    {
                        enemyHealth.GetHit(33.34f, transform.gameObject);
                    }
                }
            }

            GrassTall grass;
            if (grass = collider.GetComponent<GrassTall>())
            {
                camShake.ShakeCamera(5f, 0.2f);
                grass.GetHit(1f, gameObject);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = transform.position == null ? Vector3.zero : transform.position;
        Gizmos.DrawWireSphere(position, 1);
    }

    private void OnMove(InputValue movementValue)
    {
        if (canMove)
        {
            movementInput = movementValue.Get<Vector2>();
            if (movementInput.x != 0 || movementInput.y != 0)
            {
                lastMoveDir = movementInput;
            }
        }
    }

    private void OnAttack()
    {
        if(attackTime > attackCooldown && canMove && !isDead)
        {
            state = State.Attacking;
            StartCoroutine(StopAttackOnTime());

            var dirToAttack = (attackPosition.position - transform.position).normalized;
            lastMoveDir = dirToAttack;
            animHandler.PlayAttack(lastMoveDir);
            attackTime = 0;
            isAttacking = true;
        }
    }
    private void OnRoll()
    {
        if (isSpecialOn && canMove && !isDead)
        {
            if (specialTime > specialCooldown)
            {
                rangedAttackSFX.Play();
                var dirToAttack = (attackPosition.position - transform.position).normalized;
                lastMoveDir = dirToAttack;
                animHandler.PlayThrow(lastMoveDir);
                var projectile1 = Instantiate(projectile, transform.position, Quaternion.identity);

                projectile1.GetComponent<ProjectilePlayer>().SetTargetPosition(pointerInput);
                specialTime = 0;
                isAttacking = true;
                abilityRecharged = false;
            }
        }
        if (isDashOn && canMove && rollTime > rollCooldown)
        {
            switch (state)
            {
                case State.Idle:
                    abilityRecharged = false;
                    rollTime = 0;
                    rollDir = lastMoveDir;
                    rollSpeed = rollSpeedEditor;
                    state = State.Rolling;
                    break;
                case State.Walking:
                    abilityRecharged = false;
                    rollTime = 0;
                    rollDir = lastMoveDir;
                    rollSpeed = rollSpeedEditor;
                    state = State.Rolling;
                    break;
                default:
                    break;
            }
        }
    }
    public void ActivateSpecial()
    {
        abilityChangeSFX.Play();
        spawnVFX.Play("SpawnVFX");
        animHandler.ActivateKappa();
        ability.SetActive(true);
        icon.sprite = rangedAbilityIcon;
        avatar.sprite = rangedAvatar;
        if(!isDashOn && !isSpecialOn)
        {
            abilityChangeVFX.Play("TanukiToKappaAnim");
        }
        else if(isDashOn)
        {
            abilityChangeVFX.Play("FroggoToKappaAnim");
        }
        isDashOn = false;
        isSpecialOn = true;
    }
 
    public void ActivateDash()
    {
        abilityChangeSFX.Play();
        spawnVFX.Play("SpawnVFX");
        animHandler.ActivateFroggo();
        ability.SetActive(true);
        icon.sprite = dashAbilityIcon;
        avatar.sprite = dashAvatar;
        if (!isDashOn && !isSpecialOn)
        {
            abilityChangeVFX.Play("TanukiToFroggoAnim");
        }
        else if (isSpecialOn)
        {
            abilityChangeVFX.Play("KappaToFroggoAnim");
        }
        isSpecialOn = false;
        isDashOn = true;
    }


    // REMOVE COOLDOWN
    // DEACTIVATE ON DMG
    private IEnumerator StopAttackOnTime()
    {
        yield return new WaitForSeconds(0.45f);
        
        if(isAttacking && !isDead)
        {
            animHandler.StopAttacking(lastMoveDir);
            isAttacking = false;
            canMove = true;

            if (movementInput.x != 0 || movementInput.y != 0)
            {
                state = State.Walking;
            }
            else
            {
                state = State.Idle;
            }
        }
    }
    public void GetHit(float amount, GameObject sender)
    {
        if (sender.layer == gameObject.layer) { return; }
        hurtVignetteVFX.Play("HurtOn");
        camShake.ShakeCamera(5f, 0.2f);
        currentHealth -= amount;
        Knockback(sender);
        HitSFX.Play();
        HitVFX.Play("HitAnim");

        animHandler.ActivateNormal();
        avatar.sprite = normalAvatar;
        ability.SetActive(false);
        if(isSpecialOn)
        {
            abilityChangeVFX.Play("KappaToTanukiAnim");
            spawnVFX.Play("SpawnVFX");
            abilityChangeSFX.Play();
            isSpecialOn = false;
        }
        if(isDashOn)
        {
            abilityChangeVFX.Play("FroggoToTanukiAnim");
            abilityChangeSFX.Play();
            spawnVFX.Play("SpawnVFX");
            isDashOn = false;
        }
        
        if (currentHealth <= 0)
        {
            state = State.Dead;
            canMove = false;
            movementInput = Vector3.zero;
            lastMoveDir = Vector3.zero;
            rb.velocity = Vector2.zero;
            collision2D.enabled = false;
            isDead = true;
            animHandler.PlayDead(lastMoveDir);
            StartCoroutine(StartDeathSequence());
            deathSFX.Play();
        }
    }
    IEnumerator StartDeathSequence()
    {
        yield return new WaitForSeconds(3f);
        camBlackout.m_Lens.OrthographicSize = 6f;
        camBlackout.transform.position = transform.position;
        camBlackout.gameObject.SetActive(true);
        StartCoroutine(ResetLevel());
    }
    IEnumerator ResetLevel()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(2);
    }
    private void Knockback(GameObject other)
    {
        Vector3 dirFromPlayer = (transform.position - other.transform.position).normalized;
        transform.position += dirFromPlayer * knockbackDistance;
    }
    private void HandleAbilityRechargeVFX()
    {
        if (isSpecialOn)
        {
            if (specialTime > specialCooldown && !abilityRecharged)
            {
                abilityRechargeVFX.Play("AbilityRechargeAnim");
                abilityRecharged = true;
            }
        }
        if (isDashOn)
        {
            if (rollTime > rollCooldown && !abilityRecharged)
            {
                abilityRechargeVFX.Play("AbilityRechargeAnim");
                abilityRecharged = true;
            }
        }
    }
    private void ManageHealth()
    {
        hpBar.fillAmount = currentHealth / 100f;
        if (currentHealth > 100f)
        {
            extraHeartsFill[0].gameObject.SetActive(true);
        }
        else
        {
            extraHeartsFill[0].gameObject.SetActive(false);
        }
        if (currentHealth > 116.67f)
        {
            extraHeartsFill[1].gameObject.SetActive(true);
        }
        else
        {
            extraHeartsFill[1].gameObject.SetActive(false);
        }
        if (currentHealth > 133.34f)
        {
            extraHeartsFill[2].gameObject.SetActive(true);
        }
        else
        {
            extraHeartsFill[2].gameObject.SetActive(false);
        }
    }

    public void HealFullHealth()
    {
        currentHealth = maxHealth;
        healVFX.Play("Heal");
    }

    public void DisableMovement()
    {
        canMove = false;
        movementInput = Vector3.zero;
        lastMoveDir = Vector3.zero;
        rb.velocity = Vector2.zero;
    }

    public void HeartCollected()
    {
        heartsCollected++;
        maxHealth += 16.67f;
        currentHealth += 16.67f;
        extraHeartsBackground[0].gameObject.SetActive(true);

        if(heartsCollected == 2)
        {
            extraHeartsBackground[1].gameObject.SetActive(true);
        }

        if (heartsCollected == 3)
        {
            extraHeartsBackground[2].gameObject.SetActive(true);
        }
    }

    public void EnableMovement()
    {
        canMove = true;
    }
}
