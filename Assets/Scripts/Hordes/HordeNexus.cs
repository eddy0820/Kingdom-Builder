using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class HordeNexus : MonoBehaviour
{
    [MinMaxSlider(1, 500), SerializeField] Vector2 hordeSpawnRadius;
    [SerializeField] Color hordeMinGizmoColor = Color.red;
    [SerializeField] Color hordeMaxGizmoColor = Color.green;

    [Space(10)]

    [SerializeField] float playerReachRadius = 6f;
    [SerializeField] Color playerReachGizmoColor = Color.blue;

    [Space(10)]

    [SerializeField] HordePresetSO hordePreset;

    //public void StartHorde()
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = hordeMinGizmoColor;
        Gizmos.DrawWireSphere(transform.position, hordeSpawnRadius.x);

        Gizmos.color = hordeMaxGizmoColor;
        Gizmos.DrawWireSphere(transform.position, hordeSpawnRadius.y);

        Gizmos.color = playerReachGizmoColor;
        Gizmos.DrawWireSphere(transform.position, playerReachRadius);
    }
}

