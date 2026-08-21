// Author: Shivakant kurmi
// Summary: An editor script to automatically fix missing or broken prefab references in the scene.
using UnityEngine;
using UnityEditor;
using Doofus.Pulpits;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
[InitializeOnLoad]
public class FixPulpitReference
{
    static FixPulpitReference()
    {
        EditorApplication.delayCall += DoFix;
    }

    private static void DoFix()
    {
        // 1. Fix the Spawner's prefab reference
        var spawner = Object.FindObjectOfType<PulpitSpawner>();
        if (spawner != null)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<Pulpit>("Assets/Prefabs/Pulpit.prefab");
            if (prefab != null)
            {
                var serializedObject = new SerializedObject(spawner);
                var prefabProperty = serializedObject.FindProperty("pulpitPrefab");
                if (prefabProperty.objectReferenceValue != prefab)
                {
                    prefabProperty.objectReferenceValue = prefab;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        // 2. Delete any leftover Pulpit(Clone) in the active scene
        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        bool deletedClone = false;
        foreach (var go in rootObjects)
        {
            if (go.name == "Pulpit(Clone)")
            {
                Object.DestroyImmediate(go);
                deletedClone = true;
            }
        }

        if (deletedClone)
        {
            // Mark scene as dirty so the user can save it
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
#endif
