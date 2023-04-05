using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnsenHeal : MonoBehaviour, IInteractable
{
    [Header("Interaction Prompt")]
    [SerializeField] string prompt;
    public string InteractionPrompt => prompt;
    public bool CanInteract => canInteract;
    private bool canInteract = true;
    private AudioSource sfx;

    private void Start()
    {
        sfx = GetComponent<AudioSource>();
    }

    public bool Interact(Interactor interactor)
    {
        if (interactor.GetComponent<PlayerMovement2D>() != null)
        {
            sfx.Play();
            interactor.GetComponent<PlayerMovement2D>().HealFullHealth();
        }
        return true;
    }
}
