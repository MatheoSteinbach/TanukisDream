using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAllGrass : MonoBehaviour
{
    [SerializeField] List<GameObject> lights = new List<GameObject>();
    [SerializeField] SmallDoor door;
    [SerializeField] CameraShake camShake;
    private List<GrassTall> grassList = new List<GrassTall>();
    [SerializeField] CombatDoor combatDoor;

    private bool isOnFinalLevel = false;
    private bool doorIsOpened = false;
    private void Awake()
    {
        if (GetComponentsInChildren<EnemyWave>() != null)
        {
            var grassArray = GetComponentsInChildren<GrassTall>();
            grassList.AddRange(grassArray);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isOnFinalLevel = true;
            combatDoor.Close();
        }
    }

    private void Update()
    {
        if(isOnFinalLevel && !doorIsOpened)
        {
            var enemiesToRemove = new List<GrassTall>();
            if (grassList.Count > 0)
            {
                foreach (var grass in grassList)
                {
                    if (grass.IsDead)
                    {
                        enemiesToRemove.Add(grass);
                    }
                }
            }
            else if (grassList.Count == 0)
            {
                //open door
                combatDoor.Open();
                door.OpenDoor();
                camShake.ShakeCamera(9f, 1.75f);
                foreach (var light in lights)
                {
                    light.SetActive(false);
                }
                doorIsOpened = true;
                
            }
            foreach (var enemy in enemiesToRemove)
            {
                grassList.Remove(enemy);
            }
        }
    }
}
