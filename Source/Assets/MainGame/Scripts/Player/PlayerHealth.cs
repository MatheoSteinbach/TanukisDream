using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float currentHealth, maxHealth;
    [SerializeField] private Animator HitVFX;
    [SerializeField] private AudioSource HitSFX;
    [SerializeField] private float knockbackDistance;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image[] extraHeartsFill;
    [SerializeField] private Image[] extraHeartsBackground;
    [SerializeField] private Image heart1Fill;
    [SerializeField] private Image heart1Background;

}
