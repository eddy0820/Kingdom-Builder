using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Expandable, SerializeField] GameplaySettingsSO defaultGameplaySettings;

    [Space(15)]
    [SerializeField] bool enableHealthDebugMessages = true;
    public bool EnableHealthDebugMessages => enableHealthDebugMessages;
    [SerializeField] bool enableStaminaDebugMessages = true;
    public bool EnableStaminaDebugMessages => enableStaminaDebugMessages;
    [Space(15)]
    [SerializeField] bool showNonPlayerHealthAndStaminaText = true;
    public bool ShowNonPlayerHealthAndStaminaText => showNonPlayerHealthAndStaminaText;

    Vector3 gravity = Vector3.zero;
    public Vector3 Gravity =>  gravity;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        if(defaultGameplaySettings != null)
        {
            Debug.Log("Using scene specific gameplay settings");
            gravity = defaultGameplaySettings.GlobalGravity;
        }
        else
        {
            Debug.Log("No scene specific gameplay settings found. Using global gameplay settings");
            gravity = GlobalGameplaySettings.Instance.GlobalGravity;
        }
            
    }
}
