using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour, IInteractable
{
    [Header("Spawn Point")]
    [SerializeField] Transform spawnPoint;
    [Header("VFX")]
    [SerializeField] GameObject shine;
    [SerializeField] GameObject spawnVFX;
    [Header("Camera Shake")]
    [SerializeField] CameraShake camShake;
    [Header("Interaction Prompt")]
    [SerializeField] string prompt;
    public string InteractionPrompt => prompt;

    public bool CanInteract => canInteract;
    private bool canInteract = true;
    private CheckpointSystem checkpointSystem;
    private AudioSource sfx;
    private bool playOnce = false;
    private void Start()
    {
        sfx = GetComponent<AudioSource>();
        checkpointSystem = GameObject.FindGameObjectWithTag("CheckpointSystem").GetComponent<CheckpointSystem>();
        if(checkpointSystem.checkpointActivated)
        {
            canInteract = false;
            shine.gameObject.SetActive(false);
            spawnVFX.gameObject.SetActive(true);
        }
    }

    public bool Interact(Interactor interactor)
    {
        sfx.Play();
        canInteract = false;
        shine.gameObject.SetActive(false);
        checkpointSystem.lastCheckpointPos = spawnPoint.position;
        checkpointSystem.checkpointActivated = true;
        spawnVFX.gameObject.SetActive(true);
        camShake.ShakeCamera(5.5f, 1f);
        playOnce = true;
        return true;
    }
}
