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

    [SerializeField, ReadOnly] GridPlaceableObjectSO placeableObjectSO;

    public void Init(GridPlaceableObjectSO _placeableObjectSO)
    {
        placeableObjectSO = _placeableObjectSO;

        nameText.text = placeableObjectSO.Name;

        if(placeableObjectSO.UIICon != null)
        {
            icon.sprite = placeableObjectSO.UIICon;
        }
    }
}
