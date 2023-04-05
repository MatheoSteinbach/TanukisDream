using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Action/Spkiey Attack")]
public class ActionSpikeyAttack : AIAction
{
    public override void Act(AIController controller)
    {
        //controller.MoveTowardsDestination();
        controller.SpikeyMeleeAttack();
    }
}