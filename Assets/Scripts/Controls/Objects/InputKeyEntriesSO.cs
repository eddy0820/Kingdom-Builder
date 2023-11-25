using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Input Key Entries", menuName = "Misc/Input Key Entries")]
public class InputKeyEntriesSO : SingletonScriptableObject<InputKeyEntriesSO>
{
    [SerializeField] Sprite emptyDarkModeSprite;
    public Sprite EmptyDarkModeSprite => emptyDarkModeSprite;
    [SerializeField] Sprite emptyLightModeSprite;
    public Sprite EmptyLightModeSprite => emptyLightModeSprite;

    [Space(10)]

    [ArrayElementTitle("keyCode")]
    [SerializeField] List<InputKeyEntry> inputKeyEntries;
    public List<InputKeyEntry> InputKeyEntries => inputKeyEntries;

    [System.Serializable]
    public class InputKeyEntry
    {
        [SerializeField] KeyCode keyCode;
        public KeyCode KeyCode => keyCode;

        [SerializeField] bool replaceSprite;
        public bool ReplaceSprite => replaceSprite;

        [AllowNesting]
        [SerializeField, ShowIf("replaceSprite")] Sprite darkModeSprite;
        public Sprite DarkModeSprite => darkModeSprite;
        [AllowNesting]
        [SerializeField, ShowIf("replaceSprite")] Sprite lightModeSprite;
        public Sprite LightModeSprite => lightModeSprite;

        [AllowNesting, TextArea]
        [SerializeField, HideIf("replaceSprite")] string text;
        public string Text => text;
    }
}
