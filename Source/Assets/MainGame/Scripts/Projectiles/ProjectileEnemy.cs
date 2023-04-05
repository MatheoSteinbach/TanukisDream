using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    [SerializeField] AudioSource splashSFX;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] GameObject shootVFX;
    [SerializeField] Animator splashVFX;
    [SerializeField] Animator shootHitVFX;
    Vector3 targetPosition;
    bool isNormalShot = false;
    bool hit = false;
    private void Awake()
    {

    }
    private void Update()
    {
        if (isNormalShot)
        {
            NormalShot();
        }

    }

    public void ShootNormalShot(Vector3 destinationPos)
    {
        var dirToAttack = (destinationPos - transform.position).normalized;
        targetPosition = dirToAttack * 4000f;
        isNormalShot = true;
        StartCoroutine(DestroyOnTime());
    }

    private void NormalShot()
    {
        if (!hit)
        {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, attackRadius))
            {
                GrassTall grass;
                if (grass = collider.GetComponent<GrassTall>())
                {
                    grass.GetHit(1f, transform.gameObject);
                }
                PlayerMovement2D health;
                if (health = collider.GetComponent<PlayerMovement2D>())
                {
                    health.GetHit(16.67f, transform.gameObject);
                    targetPosition = transform.position;
                    //Destroy(gameObject);
                    hit = true;
                    splashVFX.transform.position = health.transform.position;
                    splashVFX.Play("SplatterAnim");
                    splashSFX.Play();
                    shootVFX.gameObject.SetActive(false);
                }
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        }

    }
    IEnumerator DestroyOnTime()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = transform == null ? Vector3.zero : transform.position;
        Gizmos.DrawWireSphere(position, attackRadius);
    }
}
