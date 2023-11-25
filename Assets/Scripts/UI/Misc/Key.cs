using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Key : MonoBehaviour
{
    [SerializeField] Image keyImage;
    [SerializeField] TextMeshProUGUI keyText;

    public void SetKey(string bindingPath)
    {
        KeyCode keycode = InputSystemExensions.GetKeyCodeFromBindingPath(bindingPath);
        InputKeyEntriesSO.InputKeyEntry inputKeyEntry = InputKeyEntriesSO.Instance.InputKeyEntries.Find(x => x.KeyCode == keycode);

        if(inputKeyEntry.ReplaceSprite)
        {
            keyImage.sprite = inputKeyEntry.DarkModeSprite;
            keyText.gameObject.SetActive(false);
        }
        else
        {
            keyImage.sprite = InputKeyEntriesSO.Instance.EmptyDarkModeSprite;
            keyText.text = inputKeyEntry.Text;
        }
    }
}
