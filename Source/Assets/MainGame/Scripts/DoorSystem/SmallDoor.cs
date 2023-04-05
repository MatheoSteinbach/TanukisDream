using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallDoor : MonoBehaviour
{
    [SerializeField] private GameObject model;

    [SerializeField] private GameObject dirt;
    private AudioSource sfx;
    private Animator animator;
    private Collider2D collision;
    private void Awake()
    {
        if(animator != null)
        {
            animator = GetComponent<Animator>();
        }
        collision = GetComponent<Collider2D>();
        sfx = GetComponent<AudioSource>();
    }

    public void OpenDoor()
    {
        sfx.Play();
        model.SetActive(false);
        dirt.SetActive(true);
        //animator.Play("OpenDoor");
        collision.enabled = false;
    }
}
