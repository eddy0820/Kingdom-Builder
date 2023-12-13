using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using DamageNumbersPro;

[RequireComponent(typeof(Collider))]
public class TestEnemyController : MonoBehaviour, IInteractable
{
    [SerializeField] List<InteractionTypeSO> interactionTypes;
    public List<InteractionTypeSO> InteractionTypes => interactionTypes;

    [SerializeField] DamageNumberMesh damageNumberMesh;

    TestEnemyStats testEnemyStats;
    public IDamageable IDamageable => testEnemyStats;

    private void Awake()
    {
        testEnemyStats = GetComponent<TestEnemyStats>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightBracket))
        {
            DamageNumber damageNumber = damageNumberMesh.Spawn(new Vector3(0, 2, 0));
        }
    }
}
