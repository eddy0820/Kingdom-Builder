using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using DamageNumbersPro;


public class TestEnemyController : MonoBehaviour
{
    [SerializeField] DamageNumberMesh damageNumberMesh;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightBracket))
        {
            DamageNumber damageNumber = damageNumberMesh.Spawn(new Vector3(0, 2, 0));
        }
    }
}
