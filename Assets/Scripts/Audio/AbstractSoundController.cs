using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSoundController : MonoBehaviour
{
    protected void PlayClipAtPoint(AudioClip audioClip, Vector3 position, string audioSourceName, AudioSourceConfigurationSO audioSourceConfig)
    {
        GameObject gameObj = new(audioSourceName);

        gameObj.AddComponent<AudioSource>();
        gameObj.transform.position = position;

        AudioSource audioSource = gameObj.GetComponent<AudioSource>();

        audioSource.clip = audioClip;
        ApplySourceConfig(audioSource, audioSourceConfig);

        audioSource.Play();
        Destroy(gameObj, audioClip.length + 1f);
    }

    protected void ApplySourceConfig(AudioSource audioSource, AudioSourceConfigurationSO audioSourceConfig)
    {
        audioSource.volume = audioSourceConfig.Volume;
        audioSource.spatialBlend = audioSourceConfig.SpatialBlend;
        audioSource.pitch = audioSourceConfig.Pitch;
        audioSource.panStereo = audioSourceConfig.Pan;
    }
}
