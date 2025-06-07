using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IListenToPlayerSpawn
{
    public void OnPlayerSpawned(Transform spawnTransform);
}
