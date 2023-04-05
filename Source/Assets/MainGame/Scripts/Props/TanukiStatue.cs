using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanukiStatue : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject shine;

    [Header("Interaction Prompt")]
    [SerializeField] string prompt;
    public string InteractionPrompt => prompt;
    public bool CanInteract => canInteract;
    private bool canInteract = true;

    private CheckpointSystem checkpointSystem;

    [Header("Dialogue")]
    [SerializeField] private Dialogue dialogue;

    private void Start()
    {
        checkpointSystem = GameObject.FindGameObjectWithTag("CheckpointSystem").GetComponent<CheckpointSystem>();
        shine.SetActive(false);
        if(!checkpointSystem.checkpointActivated)
        {
            StartCoroutine(StartStartDialogue());
        }
    }
    IEnumerator StartStartDialogue()
    {
        yield return new WaitForSeconds(0.1f);
        dialogue.gameObject.SetActive(true);
        dialogue.StartDialogue();
    }
    public bool Interact(Interactor interactor)
    {
        shine.SetActive(false);
        dialogue.gameObject.SetActive(true);
        dialogue.StartDialogue();
        return true;
    }

    public void SetInteractToActive(bool condition)
    {
        canInteract = condition;
        if (canInteract)
        {
            shine.SetActive(true);
        }
    }
}
