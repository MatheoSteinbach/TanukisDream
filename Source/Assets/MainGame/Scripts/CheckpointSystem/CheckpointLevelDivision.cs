using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointLevelDivision : MonoBehaviour
{
    [SerializeField] LevelDivision levelDiv;
    private CheckpointSystem checkpointSystem;
    private void Start()
    {
        checkpointSystem = GameObject.FindGameObjectWithTag("CheckpointSystem").GetComponent<CheckpointSystem>();
        if(checkpointSystem.checkpointActivated)
        {
            levelDiv.CheckpointLoad();
        }
    }
}
