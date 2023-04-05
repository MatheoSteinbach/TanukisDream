using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CheckpointSystem : MonoBehaviour
{
    private static CheckpointSystem instance;
    public Vector2 lastCheckpointPos;
    public bool checkpointActivated;
    public bool[] activeStatues;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            activeStatues = new bool[4];
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
