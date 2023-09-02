using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public class PlayerSoundController : AbstractSoundController
{
    [Header("Footstep Sounds")]
    [SerializeField] List<StepSoundMaterialVariations> stepSoundMaterialVariations;

    [Space(10)]

    [SerializeField] FeetInfo feetInfo;

    [Space(10)]

    [SerializeField] float footRaycastLength = 1.5f;
    [SerializeField] LayerMask footstepRaycastLayerMask;
    [SerializeField] EMaterialSoundType defaultMaterialSoundType = EMaterialSoundType.Asphalt;
    AudioClip prevFootstepAudioClip;

    [Header("Build Menu Sounds")]
    [SerializeField] AudioClip clickBuildingCategoryButtonSound;
    [SerializeField] AudioClip clickBuildingTypeButtonSound;
    [SerializeField] AudioClip hoverButtonSound;
    [SerializeField] AudioClip buildMenuOpenSound;
    [SerializeField] AudioClip buildMenuCloseSound;
    [SerializeField] AudioClip selectFromHotbarSound;
    [SerializeField] AudioClip putIntoHotbarSound;

    [Header("Audio Source Configurations")]
    [SerializeField] AudioSourceConfigurationSO soundConfig2D;
    
    PlayerCharacterController playerCharacterController;

    float timeSinceLastFootstepSound;

    private void Awake()
    {
        playerCharacterController = GetComponent<PlayerCharacterController>();
    }
 
#region Footstep Sound Methods

    public void PlayFootstepSound(Foot foot)
    {
        if(Time.time - timeSinceLastFootstepSound < 0.1f) return;
    
        timeSinceLastFootstepSound = Time.time;

        if(playerCharacterController.IsCrouching || playerCharacterController.IsWalking)
            PlayFootstepSoundInternal(GetMaterialVariationSounds(foot).WalkSounds);
        else
            PlayFootstepSoundInternal(GetMaterialVariationSounds(foot).RunSounds);
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

    private void PlayFootstepSoundInternal(AudioClipVariationSO audioClipVariation)
    {
        AudioClip audioClip;

        do
        {
            audioClip = audioClipVariation.GetRandomClip();
        }
        while(audioClip == prevFootstepAudioClip); 

        prevFootstepAudioClip = audioClip;

        PlayClipAtPoint(audioClip, Camera.main.transform.position, audioClipVariation.Name + " Footstep Sound", soundConfig2D);
    }

#endregion

#region Build Menu Sound Methods

    public void PlayClickBuildingCategoryButtonSound()
    {
        PlayClipAtPoint(clickBuildingCategoryButtonSound, Camera.main.transform.position, "Click Building Category Button Sound", soundConfig2D);
    }

    public void PlayClickBuildingTypeButtonSound()
    {
        PlayClipAtPoint(clickBuildingTypeButtonSound, Camera.main.transform.position, "Click Building Type Button Sound", soundConfig2D);
    }

    public void PlayerHoverButtonSound()
    {
        PlayClipAtPoint(hoverButtonSound, Camera.main.transform.position, "Hover Button Sound", soundConfig2D);
    }

    public void PlayBuildMenuAppearanceSound(bool isAppearing)
    {
        if(isAppearing)
            PlayClipAtPoint(buildMenuOpenSound, Camera.main.transform.position, "Build Menu Open Sound", soundConfig2D);
        else
            PlayClipAtPoint(buildMenuCloseSound, Camera.main.transform.position, "Build Menu Close Sound", soundConfig2D);
    }

    public void PlaySelectFromHotbarSound()
    {
        PlayClipAtPoint(selectFromHotbarSound, Camera.main.transform.position, "Select From Hotbar Sound", soundConfig2D);
    }

    public void PlayPutIntoHotbarSound()
    {
        PlayClipAtPoint(putIntoHotbarSound, Camera.main.transform.position, "Put Into Hotbar Sound", soundConfig2D);
    }

#endregion

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
