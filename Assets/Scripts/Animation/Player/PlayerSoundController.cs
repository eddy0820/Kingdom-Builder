using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public class PlayerSoundController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] List<StepSoundMaterialVariations> stepSoundMaterialVariations;

    [Header("One Shot Audio Source Configurations")]
    [SerializeField] AudioSourceConfiguration footstepSoundConfig;

    [Header("Footstep Audio Feet Info")]
    [SerializeField] FeetInfo feetInfo;

    [Header("Other")]
    [SerializeField] float footRaycastLength = 1.5f;
    [SerializeField] LayerMask footstepRaycastLayerMask;
    [SerializeField] EMaterialSoundType defaultMaterialSoundType = EMaterialSoundType.Asphalt;
    AudioClip prevFootstepAudioClip;
 
    public void PlayRunSound(Foot foot)
    {
        PlayFootstepSound(GetMaterialVariationSounds(foot).RunSounds);
    }

    public void PlayWalkSound(Foot foot)
    {
        PlayFootstepSound(GetMaterialVariationSounds(foot).WalkSounds);
    }

    private StepSoundMaterialVariations GetMaterialVariationSounds(Foot foot)
    {
        FootInfo footInfo = feetInfo.GetFootInfo(foot);

        return stepSoundMaterialVariations.Find(x => x.MaterialSoundType == DoOverlapSphereForFoot(foot));
    }

    private EMaterialSoundType DoOverlapSphereForFoot(Foot foot)
    {
        FootInfo footInfo = feetInfo.GetFootInfo(foot);

        Physics.Raycast(footInfo.OverlapTransform.position, Vector3.down, out RaycastHit hitInfo, footRaycastLength, footstepRaycastLayerMask, QueryTriggerInteraction.Ignore);

        return hitInfo.collider.gameObject.TryGetComponent(out IMateralSoundType i) ? i.GetMaterialSoundType() : defaultMaterialSoundType;
    }

    private void PlayFootstepSound(AudioClipVariationSO audioClipVariation)
    {
        AudioClip audioClip;

        do
        {
            audioClip = audioClipVariation.GetRandomClip();
        }
        while(audioClip == prevFootstepAudioClip); 

        prevFootstepAudioClip = audioClip;

        PlayClipAtPoint(audioClip, Camera.main.transform.position, audioClipVariation.Name + " Footstep Sound", footstepSoundConfig);
    }

    private void PlayClipAtPoint(AudioClip audioClip, Vector3 position, string audioSourceName, AudioSourceConfiguration audioSourceConfig)
    {
        GameObject gameObj = new(audioSourceName);

        gameObj.AddComponent<AudioSource>();
        gameObj.transform.position = position;

        AudioSource audioSource = gameObj.GetComponent<AudioSource>();

        audioSource.clip = audioClip;
        audioSource.volume = audioSourceConfig.Volume;
        audioSource.spatialBlend = audioSourceConfig.SpatialBlend;
        audioSource.pitch = audioSourceConfig.Pitch;
        audioSource.panStereo = audioSourceConfig.Pan;

        audioSource.Play();
        Destroy(gameObj, audioClip.length);
    }

    private void OnDrawGizmos()
    {
        feetInfo.ForEach(x => Gizmos.DrawRay(x.OverlapTransform.position, Vector3.down * footRaycastLength));
    }

    [System.Serializable]
    public class StepSoundMaterialVariations
    {
        [SerializeField] EMaterialSoundType materialSoundType;
        public EMaterialSoundType MaterialSoundType => materialSoundType;

        [Space(10)]

        [SerializeField] AudioClipVariationSO walkSounds;
        public AudioClipVariationSO WalkSounds => walkSounds;

        [SerializeField] AudioClipVariationSO runSounds;
        public AudioClipVariationSO RunSounds => runSounds;
    }

    [System.Serializable]
    public class AudioSourceConfiguration
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

    [System.Serializable]
    public class FeetInfo
    {
        [SerializeField] FootInfo leftFootInfo;
        public FootInfo LeftFootInfo => leftFootInfo;

        [SerializeField] FootInfo rightFootInfo;
        public FootInfo RightFootInfo => rightFootInfo;

        public FootInfo GetFootInfo(Foot foot)
        {
            switch(foot)
            {
                case Foot.Left:     return leftFootInfo;
                case Foot.Right:    return rightFootInfo;

                default:            return leftFootInfo;
            }
        }

        public void ForEach(Action<FootInfo> action)
        {
            action.Invoke(leftFootInfo);
            action.Invoke(rightFootInfo);
        }
    }

    [System.Serializable]
    public class FootInfo
    {
        [SerializeField] Foot foot;
        public Foot Foot => foot;

        [SerializeField] Transform overlapTranform;
        public Transform OverlapTransform => overlapTranform;
    }

    public enum Foot
    {
        Left,
        Right
    }
}
