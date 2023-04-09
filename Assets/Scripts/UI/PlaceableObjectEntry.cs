using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

public class PlaceableObjectEntry : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;

    [Space(10)]

    [SerializeField, ReadOnly] PlaceableGridObjectSO placeableObjectSO;

    public void Init(PlaceableGridObjectSO _placeableObjectSO)
    {
        placeableObjectSO = _placeableObjectSO;

        nameText.text = placeableObjectSO.Name;

        if(placeableObjectSO.MainIcon != null)
        {
            icon.sprite = placeableObjectSO.MainIcon;
        }
    }
}
