using Cinemachine;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyTanukiStatue : MonoBehaviour, IInteractable
{
    [Header("ID")]
    [SerializeField] private int statueID;

    [Header("Model")]
    [SerializeField] private GameObject model;

    [Header("Doors")]
    [SerializeField] private List<SmallDoor> smallDoors;
    [SerializeField] private BossDoor bossDoor;
    [SerializeField] private GameObject bossDoorLight;

    [Header("Cables")]
    [SerializeField] private GameObject cable;

    [Header("CameraShake")]
    [SerializeField] private CameraShake camShake;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera fadeToBlackCam;
    [SerializeField] private CinemachineVirtualCamera doorCam;
    [SerializeField] private CinemachineVirtualCamera bigDoorCam;

    [Header("MoviePanel")]
    [SerializeField] private MoviePanel moviePanel;

    [Header("Combat Areas")]
    [SerializeField] List<CombatArena> combatArenas = new List<CombatArena>();

    [Header("VFX")]
    [SerializeField] GameObject shine;

    [Header("Interaction Prompt")]
    [SerializeField] string prompt;
    public string InteractionPrompt => prompt;
    public bool CanInteract => canInteract;
    private bool canInteract = true;

    private PlayerMovement2D player;
    private AudioSource sfx;
    private Animator animator;
    private CheckpointSystem checkpointSystem;
    private float bossLightTimer;
    private bool onBigDoorScene = false;
    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement2D>();
        animator = GetComponentInChildren<Animator>();
        sfx = GetComponent<AudioSource>();
    }
    private void Start()
    {
        checkpointSystem = GameObject.FindGameObjectWithTag("CheckpointSystem").GetComponent<CheckpointSystem>();
        if (checkpointSystem.activeStatues[statueID])
        {
            CheckpointLoad();
        }
        else
        {
            animator.Play("BabyTanuki_Idle");
        }
    }
    private void Update()
    {
        if(onBigDoorScene)
        {
            bossLightTimer += Time.deltaTime;
        }
        if (bossLightTimer > 1f && onBigDoorScene)
        {
            bossDoorLight.SetActive(true);
            onBigDoorScene = false;
        }
    }
    private void CheckpointLoad()
    {
        foreach (var arena in combatArenas)
        {
            arena.Cleared();
        }
        cable.GetComponentInChildren<PathFollower>().speed = 100;
        canInteract = false;
        shine.SetActive(false);
        cable.SetActive(true);
        bossDoorLight.SetActive(true);
        animator.Play("BabyTanuki_Active");
        foreach (var door in smallDoors)
        {
            door.OpenDoor();
        }
    }
    public bool Interact(Interactor interactor)
    {
        
        checkpointSystem.activeStatues[statueID] = true;
        shine.SetActive(false);
        cable.SetActive(true);
        animator.Play("BabyTanuki_Activation");
        player.DisableMovement();
        canInteract = false;
        moviePanel.StartCutscene();
        StartCoroutine(PlayAudio());
        StartCoroutine(StartSequence());
        return true;
    }
    IEnumerator PlayAudio()
    {
        yield return new WaitForSeconds(1f);
        {
            sfx.Play();
        }
    }
    // Start Cutscene Sequence -> Change cam to black out cam
    IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(2f);
        fadeToBlackCam.m_Priority = 14;
        fadeToBlackCam.m_Lens.OrthographicSize = 6f;
        fadeToBlackCam.transform.position = playerCam.transform.position;
        fadeToBlackCam.gameObject.SetActive(true);
        StartCoroutine(BigDoorActivation());
    }
    // Part 2 of Cutscene -> Change cam to Big Door Cam
    IEnumerator BigDoorActivation()
    {
        yield return new WaitForSeconds(2f);
        fadeToBlackCam.transform.position = bigDoorCam.transform.position;
        fadeToBlackCam.gameObject.SetActive(false);
        bigDoorCam.gameObject.SetActive(true);
        onBigDoorScene = true;
        StartCoroutine(FadeScreenToBlackToPlayer());
    }
    // Part 3 of Cutscene -> Change cam to black out cam
    IEnumerator FadeScreenToBlackToPlayer()
    {
        yield return new WaitForSeconds(2f);
        fadeToBlackCam.gameObject.SetActive(true);
        bigDoorCam.gameObject.SetActive(false);
        StartCoroutine(ChangeToPlayerCam());
    }
    // Part 4 of Cutscene -> Change cam to Player & enable movement
    IEnumerator ChangeToPlayerCam()
    {
        yield return new WaitForSeconds(2f);
        moviePanel.EndCutscene();
        playerCam.transform.position = player.transform.position;
        fadeToBlackCam.transform.position = playerCam.transform.position;
        fadeToBlackCam.gameObject.SetActive(false);
        player.EnableMovement();
        StartCoroutine(ShakeyShakey());
    }
    // Open Door after Cutscene with Small Earthquake effect
    IEnumerator ShakeyShakey()
    {
        yield return new WaitForSeconds(1f);
        camShake.ShakeCamera(7.5f, 1.25f);
        foreach (var door in smallDoors)
        {
            door.OpenDoor();
        }
    }
}
