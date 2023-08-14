using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Clip Variation", menuName = "Audio/Audio Clip Variation")]
public class AudioClipVariationSO : ScriptableObject
{
    [SerializeField] new string name;
    public string Name => name;

    [SerializeField] AudioClip[] audioClips;

    public AudioClip GetRandomClip()
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
