using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [Scene, SerializeField] string playerScene;
    [SerializeField] Transform spawnPoint;

    private void Awake()
    {
        PlayerSpawner[] playerSpawners = FindObjectsOfType<PlayerSpawner>();

        if(playerSpawners.Length > 1)
        {
            Debug.LogWarning("Multiple PlayerSettings found in scene. Only one should exist.");
            
            for(int i = 1; i < playerSpawners.Length; i++)
                Destroy(playerSpawners[i]);
        }

        SceneManager.sceneLoaded += OnPlayerLoaded;
        SceneManager.LoadScene(playerScene, LoadSceneMode.Additive);
    }

    private void OnPlayerLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnPlayerLoaded;

        if(WasPlayerSceneLoaded())
           StartCoroutine(InitPlayer());
    }

    IEnumerator InitPlayer()
    {
        yield return null;

        List<IListenToPlayerSpawn> listeners = FindObjectsOfType<MonoBehaviour>().OfType<IListenToPlayerSpawn>().ToList();
        listeners.ForEach(listener => listener.OnPlayerSpawned(spawnPoint));

        yield break;
    }

    private bool WasPlayerSceneLoaded()
    {
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if(scene.name == playerScene)
            {
                return true;
            }
        }

        return false;
    }
}
