using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] GameObject crosshair;
    public GameObject Crosshair => crosshair;
    [SerializeField] GameObject buildMenu;
    public GameObject BuildMenu => buildMenu;
    [SerializeField] GameObject buildHotbar;
    public GameObject BuildHotbar => buildHotbar;

    [Space(15)]

    [ReadOnly, SerializeField] bool buildMenuEnabled = false;
    public bool BuildMenuEnabled => buildMenuEnabled;

    BuildHotbarInterface buildHotbarInterface;
    public BuildHotbarInterface BuildHotbarInterface => buildHotbarInterface;

    private void Awake()
    {
        buildHotbarInterface = buildHotbar.GetComponent<BuildHotbarInterface>();
    }

    public void ToggleBuildMenu()
    {
        Toggle(buildMenuEnabled);
    }

    public void ToggleBuildMenu(bool b)
    {
        Toggle(!b);
    }

    private void Toggle(bool b)
    {
        if(b)
        {
            buildMenu.SetActive(false);
            crosshair.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            buildMenuEnabled = false;
        }
        else
        {
            buildMenu.SetActive(true);
            crosshair.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            buildMenuEnabled = true;
        }
    }

}
