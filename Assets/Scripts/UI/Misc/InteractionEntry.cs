using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class InteractionEntry : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] TextMeshProUGUI interactionNameText;

    public Action OnInteract;

    public void SetInteraction(string bindingPath, string interactionText, Action interactionAction)
    {
        gameObject.SetActive(true);

        key.SetKey(bindingPath);
        interactionNameText.text = interactionText;

        OnInteract += () => interactionAction();
    }

    public void HideInteraction()
    {
        gameObject.SetActive(false);

        OnInteract = null;
    }
}
