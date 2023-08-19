using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Source Configuration", menuName = "Audio/Audio Source Configuration")]
public class AudioSourceConfigurationSO : ScriptableObject
{
    [Range(0.0f, 1.0f)]
    [SerializeField] float volume = 1.0f;
    public float Volume => volume;

    [Range(0.0f, 1.0f)]
    [SerializeField] float spatialBlend = 0f;
    public float SpatialBlend => spatialBlend;

    [Range(-3.0f, 3.0f)]
    [SerializeField] float pitch = 1.0f;
    public float Pitch => pitch;

    [Range(-1.0f, 1.0f)]
    [SerializeField] float pan = 0f;
    public float Pan => pan;
}
