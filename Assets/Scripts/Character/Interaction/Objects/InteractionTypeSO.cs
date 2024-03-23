using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using NaughtyAttributes;
using System.Text;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Interaction Type", menuName = "Interaction/Interaction Type")]
public class InteractionTypeSO : ScriptableObject
{
    [SerializeField] new string name;
    public string Name => name;

    [SerializeField] string id = "";
    public string ID => id;

    [Header("Behavior")]
    [SerializeField] string scriptPath = "Assets/Resources/GameData/InteractionTypes/";
    [SerializeField] string prefabPath = "Assets/Resources/GameData/InteractionTypes/";
    [Space(5)]
    [ReadOnly, SerializeField] string pathName = "";
    [ReadOnly, SerializeField] string actualScriptPath = "";
    [ReadOnly, SerializeField] string actualPrefabPath = "";
    [Space(5)]
    [Required, ReadOnly, SerializeField] InteractionTypeBehavior interactionTypeBehavior;
    public Type InteractionTypeBehaviorType => interactionTypeBehavior.GetType();
    public Action<Interactable.InteractionTypeEntry> Interact => interactionTypeBehavior.Interact;


    [Button]
    private void CreateBehaviorScript()
    {
        #if UNITY_EDITOR

        pathName = Regex.Replace(name, @"\s+", "");

        actualScriptPath = scriptPath + pathName + "InteractionBehavior.cs";
        actualPrefabPath = prefabPath + pathName + "InteractionBehavior.prefab";

        if(!File.Exists(actualScriptPath))
        {
            StringBuilder sb = new();

            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("");
            sb.AppendLine("public class " + pathName + "InteractionBehavior : InteractionTypeBehavior");
            sb.AppendLine("{");
            sb.AppendLine("    public override void Interact(Interactable interactable)");
            sb.AppendLine("    {");
            sb.AppendLine("        throw new System.NotImplementedException();");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(actualScriptPath, sb.ToString());

            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("Behavior Script already exists!");
        }

        #endif
    }

    [Button]
    private void CreateBehaviorPrefab()
    {
        #if UNITY_EDITOR

        GameObject obj = new(pathName + "InteractionBehavior");
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        obj.transform.localScale = Vector3.one;

        obj.AddComponent(Type.GetType(pathName + "InteractionBehavior"));

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(obj, actualPrefabPath);
        DestroyImmediate(obj);

        interactionTypeBehavior = prefab.GetComponent(Type.GetType(pathName + "InteractionBehavior")) as InteractionTypeBehavior;

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        #endif
    }

    [Button]
    private void DeleteBehavior()
    {
        #if UNITY_EDITOR

        if(File.Exists(actualScriptPath))
        {
            File.Delete(actualScriptPath);
            File.Delete(actualScriptPath + ".meta");
        }
        else
        {
            Debug.LogError("Script does not exist!");
        }

        if(File.Exists(actualPrefabPath))
        {
            File.Delete(actualPrefabPath);
            File.Delete(actualPrefabPath + ".meta");
        }
        else
        {
            Debug.LogError("Prefab does not exist!");
        }

        pathName = "";
        actualScriptPath = "";
        actualPrefabPath = "";

        AssetDatabase.Refresh();

        #endif
    }

    [Button]
    private void OpenScript()
    {
        #if UNITY_EDITOR

        if(File.Exists(actualScriptPath))
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(actualScriptPath, 0);
        }
        else
        {
            Debug.LogError("Script does not exist!");
        }

        #endif
    }

}
