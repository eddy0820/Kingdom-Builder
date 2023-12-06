using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class NPCHealthBarCanvas : MonoBehaviour
{
    [SerializeField] NPCStatUI statUI;

    [Space(15)]

    [SerializeField] MonoBehaviour ITargetableMono;
    ITargetable ITargetable => ITargetableMono as ITargetable;
    [SerializeField] MonoBehaviour IHoldStatsMono;
    IHoldStats IHoldStats => IHoldStatsMono as IHoldStats;
    IDamageable IDamageable => IHoldStats.IDamageable;
    CharacterStats Stats => IHoldStats.Stats;

    LockedOnCharacterControllerState lockedOnCharacterControllerState;
    PlayerController PlayerController => PlayerController.Instance;
    PlayerCanvas PlayerCanvas => PlayerController.UICanvas;

    [Space(15)]

    [SerializeField] LayerMask raycastLayerMask;

    bool doRaycast = true;

    private void Start()
    {
        statUI.SetupStatUI(this);

        IDamageable.OnHealthChanged += statUI.UpdateHealthBar;
        Stats.OnStatModifierChanged += statUI.OnStatModifierChangedHealthChanged;
        statUI.HealthHUDFade.Tweens.Find(t => t.TweenValues.TweenType == ETweenType.Fade).TweenValues.CanvasGroup.alpha = 0;

        PlayerController.StateMachine.GetState(out lockedOnCharacterControllerState);

        lockedOnCharacterControllerState.OnAcquiredTarget += OnAquiredTargetLockedOnState;
        lockedOnCharacterControllerState.OnLostTarget += OnLostTargetLockedOnState;
    }

    private void FixedUpdate()
    {
        if(!doRaycast) return;

        Ray ray = new(transform.position, Camera.main.transform.position - transform.position);
        
        if(Physics.Raycast(ray, out RaycastHit hit, Vector3.Distance(transform.position, Camera.main.transform.position), raycastLayerMask))
        {
            if(hit.collider.gameObject != null)
            {
                if(statUI.HealthHUDFade.GameObj.activeSelf) statUI.HealthHUDFade.GameObj.SetActive(false);
            }
        }
        else
        {
            if(!statUI.HealthHUDFade.GameObj.activeSelf) statUI.HealthHUDFade.GameObj.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 180, 0);
    }

    private void OnAquiredTargetLockedOnState(ITargetable aquiuredTarget)
    {
        if(aquiuredTarget != ITargetable) return;

        doRaycast = false;
        statUI.HealthHUDFade.GameObj.SetActive(false);

        PlayerCanvas.PlayerStatUI.ToggleSingleTargetHealthBar(true, Stats, IDamageable);
    }

    private void OnLostTargetLockedOnState(ITargetable lostTarget)
    {
        if(lostTarget != ITargetable) return;

        doRaycast = true;
        statUI.HealthHUDFade.GameObj.SetActive(true);

        PlayerCanvas.PlayerStatUI.ToggleSingleTargetHealthBar(false, Stats, IDamageable);
    }

    [Serializable]
    public class NPCStatUI : StatUI
    {
        NPCHealthBarCanvas nPCHealthBarCanvas;

        protected override IDamageable IDamageable => nPCHealthBarCanvas.IDamageable;
        protected override Transform DamageNumberSpawnTransform => nPCHealthBarCanvas.transform;
        protected override Vector3 DamageNumberSpawnPosition => DamageNumberSpawnTransform.position;
        protected override Stat MaxHealthStat => nPCHealthBarCanvas.Stats.GetStatFromName[CommonStatTypeNames.MaxHealth];

        public void SetupStatUI(NPCHealthBarCanvas _nPCHealthBarCanvas)
        {
            nPCHealthBarCanvas = _nPCHealthBarCanvas;
        }
    }
}
