using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterCreationStartButtonInterface : ButtonInterface<CharacterCreationStartButtonInterface.StartButtonEntry>
{
    [SerializeField] Color defaultColor;
    [SerializeField] Color hoverColor;

    [Space(15)]

    [Scene]
    [SerializeField] string gameScene;

    protected override void OnEnter(GameObject obj)
    {
        Buttons[0].Button.GetComponent<Image>().color = hoverColor;
    }

    protected override void OnExit(GameObject obj)
    {
        Buttons[0].Button.GetComponent<Image>().color = defaultColor;
    }

    public override void OnSelectButton(ButtonEntry buttonEntry)
    {
        SceneManager.LoadScene(gameScene);
    }

    [System.Serializable]
    public class StartButtonEntry : ButtonEntry {}
}
