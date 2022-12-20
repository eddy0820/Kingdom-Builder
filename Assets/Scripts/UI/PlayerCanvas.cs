using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] GameObject crosshair;
    public GameObject Crosshair => crosshair;
    [SerializeField] GameObject buildMenu;
    public GameObject BuildMenu => buildMenu;

    [Space(15)]

    [ReadOnly, SerializeField] bool buildMenuEnabled = false;
    public bool BuildMenuEnabled => buildMenuEnabled;

    public void ToggleBuildMenu()
    {
        if(buildMenuEnabled)
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

    public void ToggleBuildMenu(bool b)
    {
        if(!b)
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
