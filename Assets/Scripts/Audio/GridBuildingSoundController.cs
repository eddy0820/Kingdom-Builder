using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSoundController : AbstractSoundController
{
    [Header("Audio Source Configurations")]
    [SerializeField] AudioSourceConfigurationSO buildingSoundConfig;

    [Header("Enable Building Audio")]
    [SerializeField] AudioSource enableBuildingSource;
    [SerializeField] AudioClip enableBuildingAudioClip;
    [SerializeField] AudioClip disableBuildingAudioClip;

    [Header("Build & Demolish Audio")]
    [SerializeField] AudioSource buildAndDemolishSource;
    [SerializeField] AudioClipVariationSO buildAudioClipVariation;
    [SerializeField] AudioClip demolishAudioClip;
    [SerializeField] AudioClip invalidBuildAudioClip;
    AudioClip prevBuildAudioClip;

    [Header("Building Ghost Audio")]
    [SerializeField] AudioClip buildingGhostAudioClip;
    

    private void Awake()
    {
        ApplySourceConfig(enableBuildingSource, buildingSoundConfig);
        ApplySourceConfig(buildAndDemolishSource, buildingSoundConfig);
    }

    public void PlayToggleBuildingSound(bool isBuildingEnabled)
    {
        enableBuildingSource.Stop();

        if(isBuildingEnabled)
            enableBuildingSource.clip = enableBuildingAudioClip;
        else
            enableBuildingSource.clip = disableBuildingAudioClip;

        enableBuildingSource.Play();
    }

    public void PlayBuildSound(bool placed)
    {
        AudioClip audioClip;

        if(placed)
        {
            do
            {
                audioClip = buildAudioClipVariation.GetRandomClip();
            }
            while(audioClip == prevBuildAudioClip); 

            prevBuildAudioClip = audioClip;
        }
        else
        {
            audioClip = invalidBuildAudioClip;
        }

        buildAndDemolishSource.Stop();
        buildAndDemolishSource.clip = audioClip;
        buildAndDemolishSource.Play();
    }

    public void PlayDemolishSound()
    {
        buildAndDemolishSource.Stop();
        buildAndDemolishSource.clip = demolishAudioClip;
        buildAndDemolishSource.Play();
    }

    public void PlayBuildingGhostSnapSound()
    {
        PlayClipAtPoint(buildingGhostAudioClip, Camera.main.transform.position, "Building Ghost Snap Sound", buildingSoundConfig);
    }
}
