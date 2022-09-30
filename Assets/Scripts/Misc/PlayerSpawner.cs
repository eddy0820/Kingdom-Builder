using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Scene]
    [SerializeField] string playerScene;

    [Space(15)]

    [SerializeField] Transform spawnPoint;
    public Transform SpawnPoint => spawnPoint;

    private void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(playerScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    private void Start()
    {
        PlayerCharacterController playerCharacter = FindObjectOfType<PlayerCharacterController>();
        playerCharacter.Motor.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
    }
}
