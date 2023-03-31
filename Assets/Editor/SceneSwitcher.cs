using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

static class ToolbarStyles
{
    public static readonly GUIStyle commandButtonStyle;

    static ToolbarStyles()
    {
        commandButtonStyle = new GUIStyle("Command")
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove,
            fontStyle = FontStyle.Bold
        };
    }
}

[InitializeOnLoad]
public class SceneSwitchLeftButton
{
    static SceneSwitchLeftButton()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        if(GUILayout.Button(new GUIContent("W", "TestWorld"), ToolbarStyles.commandButtonStyle))
        {
            SceneHelper.StartScene("TestWorld");
        }

        if(GUILayout.Button(new GUIContent("P", "PlayerCharacter"), ToolbarStyles.commandButtonStyle))
        {
            SceneHelper.StartScene("PlayerCharacter");
        }
    }
}

[InitializeOnLoad]
public class SceneSwitchRightButton
{
    static SceneSwitchRightButton()
    {
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();
        
        if(GUILayout.Button(new GUIContent("I", "IconCreator"), ToolbarStyles.commandButtonStyle))
        {
            SceneHelper.StartScene("IconCreator");
        }

        if(GUILayout.Button(new GUIContent("C", "CharacterCreation"), ToolbarStyles.commandButtonStyle))
        {
            SceneHelper.StartScene("PlayerCharacter");
        }
    }
}

static class SceneHelper
{
    static string sceneToOpen;

    public static void StartScene(string sceneName)
    {
        if(EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }

        sceneToOpen = sceneName;
        EditorApplication.update += OnUpdate;
    }

    static void OnUpdate()
    {
        if (sceneToOpen == null ||
            EditorApplication.isPlaying || EditorApplication.isPaused ||
            EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        EditorApplication.update -= OnUpdate;

        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // need to get scene via search because the path to the scene
            // file contains the package version so it'll change over time
            string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
            if (guids.Length == 0)
            {
                Debug.LogWarning("Couldn't find scene file");
            }
            else
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                EditorSceneManager.OpenScene(scenePath);
            }
        }
        sceneToOpen = null;
    }
}
