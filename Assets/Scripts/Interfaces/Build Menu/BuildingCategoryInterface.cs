using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingCategoryInterface : ButtonInterface<BuildingCategoryInterface.SelectionButtonEntry>
{
    [Header("Colors")]
    [SerializeField] Color defaultButtonColor;
    public Color DefaultButtonColor => defaultButtonColor;
    [SerializeField] Color selectedButtonColor;

    [Header("Prefab Parents")]
    [SerializeField] RectTransform screenParent;

    [Header("Prefabs")]
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] GameObject screenPrefab;

    protected override void OnAwake()
    {
        foreach(string name in Enum.GetNames(typeof(BuildingCategoryTypes)))
        {
            GameObject button = Instantiate(buttonPrefab, buttonPrefab.transform.position, buttonPrefab.transform.rotation, transform);
            GameObject screen = Instantiate(screenPrefab, screenParent.position, screenPrefab.transform.rotation, screenParent);
            string newName = AddSpacesToString(name, true);
            button.GetComponentInChildren<TextMeshProUGUI>().text = newName;
            screen.GetComponentInChildren<TextMeshProUGUI>().text = newName;
            buttons.Add(new SelectionButtonEntry(button, screen));
        }

        base.OnAwake();

        foreach(SelectionButtonEntry buttonEntry in Buttons)
        {
            buttonEntry.Button.GetComponent<Image>().color = defaultButtonColor;
            buttonEntry.Screen.SetActive(false);
        }

        OnSelectButton(Buttons[0]);
    }

    public override void OnSelectButton(ButtonEntry buttonEntry)
    {
        DeselectAllButtons();
        buttonEntry.Button.GetComponent<Image>().color = selectedButtonColor;
        ((SelectionButtonEntry) buttonEntry).Screen.SetActive(true);
    }

    private void DeselectAllButtons()
    {
        foreach(ButtonEntry buttonEntry in Buttons)
        {
            buttonEntry.Button.GetComponent<Image>().color = defaultButtonColor;
            ((SelectionButtonEntry) buttonEntry).Screen.SetActive(false);
        }
    }

    [System.Serializable]
    public class SelectionButtonEntry : ButtonEntry
    {
        [SerializeField] GameObject screen;
        public GameObject Screen => screen;

        public SelectionButtonEntry(GameObject _button, GameObject _screen)
        {
            button = _button;
            screen = _screen;
        }
    }

    private string AddSpacesToString(string text, bool preserveAcronyms)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);

        for(int i = 1; i < text.Length; i++)
        {
            if(char.IsUpper(text[i]))
                if((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (preserveAcronyms && char.IsUpper(text[i - 1]) && 
                    i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    newText.Append(' ');
            newText.Append(text[i]);
        }
        
        return newText.ToString();
    }
}
