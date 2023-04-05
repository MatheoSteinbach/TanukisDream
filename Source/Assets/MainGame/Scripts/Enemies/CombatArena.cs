using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatArena : MonoBehaviour
{
    [Header("Music & SFX")]
    [SerializeField] AudioSource smokeBomb;
    [SerializeField] AudioSource combatMusic;
    [SerializeField] AudioSource normalMusic;
    [Header("Enemy Waves")]
    [SerializeField] List<EnemyWave> waves = new List<EnemyWave>();
    [SerializeField] float timeBetweenWaves = 2f;
    private CombatDoor[] doors;
    private BoxCollider2D boxCollider;
    private CameraShake camShake;
    private int currentWave = 0;
    private bool allWavesCleared = false;
    private bool startingNextWave = false;
    private void Awake()
    {
        if (GetComponentsInChildren<CombatDoor>() != null)
        {
            doors = GetComponentsInChildren<CombatDoor>();
        }
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        var player = FindObjectOfType<PlayerMovement2D>().gameObject;
        camShake = player.GetComponentInChildren<CameraShake>();
    }
    private void Update()
    {
        if (waves[currentWave].HasEnded)
        {
            // if a next Wave exists -> start next Wave
            if (waves.ElementAtOrDefault(currentWave + 1) != null)
            {
                if (!waves[currentWave + 1].HasStarted && !startingNextWave)
                {
                    StartCoroutine(StartNextWave());
                    startingNextWave = true;
                }
            }
            // if next Wave doesn't exist -> Combat Area cleared!
            else
            {
                if(!allWavesCleared)
                {
                    foreach (var door in doors)
                    {
                        door.Open();
                    }
                    combatMusic.Stop();
                    normalMusic.Play();

                    allWavesCleared = true;
                }
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            smokeBomb.Play();
            waves[0].StartWave();
            camShake.ShakeCamera(5, 0.4f);
            combatMusic.Play();
            normalMusic.Stop();
            foreach (var door in doors)
            {
                door.Close();
            }
            boxCollider.enabled = false;
        }
    }
    public void Cleared()
    {
        boxCollider.enabled = false;
    }
    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        if (waves.ElementAtOrDefault(currentWave + 1) != null)
        {
            waves[currentWave + 1].StartWave();
            smokeBomb.Play();
            camShake.ShakeCamera(5, 0.4f);
            currentWave++;
            startingNextWave = false;
        }
    }
}
