using UnityEditor;
using EddyLib.Util.Editor;

namespace KingdomBuilder.Editor
{

public static class KingdomBuilderMenuItems
{
    const string RESOURCES_MENU_ITEM_PATH = "Menu Items/";

    [MenuItem("GameObject/Volume/Presets/Quibli Preset")]
    private static void CreateGlobalVolumeQuibliPreset()
    {
        CreateUtil.CreatePrefabInScene(RESOURCES_MENU_ITEM_PATH + "Global Volume - Quibli Preset");
    }

    [MenuItem("GameObject/Player Spawner", priority = 0)]
    private static void CreatePlayerSpawner()
    {
        CreateUtil.CreatePrefabInScene(RESOURCES_MENU_ITEM_PATH + "Player Spawner");
    }
}

}
