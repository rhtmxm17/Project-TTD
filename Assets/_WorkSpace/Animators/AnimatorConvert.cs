#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;
using System.IO;
using Assets.FantasyMonsters.Common.Scripts;

[CreateAssetMenu()]
public class AnimatorConvert : ScriptableObject
{
    [SerializeField] AnimatorController baseControllerAsset;
    [SerializeField] Object souceAnimatorFolder;
    [SerializeField] Object targetAnimatorFolder;

    [SerializeField] Object characterModelFolder;
    [SerializeField] CharacterModel model;

    [ContextMenu("Create Override Test")]
    private void CreateOverrideController()
    {
        //overrideController.ApplyOverrides()
        string[] GUIDs = AssetDatabase.FindAssets($"t:AnimatorController", new string[] { AssetDatabase.GetAssetPath(souceAnimatorFolder) });
        foreach (string guid in GUIDs)
        {
            string sourceAssetPath = AssetDatabase.GUIDToAssetPath(guid);
            string sourceAssetDirectory = Path.GetDirectoryName(sourceAssetPath);
            string sourceAssetFolderName = Path.GetFileName(sourceAssetDirectory);

            AnimatorController found = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(guid));
            if (found == null)
            {
                Debug.Log("이건 뭐지");
                continue;
            }
            
            AnimatorOverrideController overrideController = new AnimatorOverrideController(baseControllerAsset);

            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
            overrideController.GetOverrides(overrides);
            foreach (var animClip in found.animationClips)
            {
                for (int i = 0; i < overrides.Count; ++i)
                {
                    if (overrides[i].Key.name == animClip.name)
                    {
                        overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, animClip);
                        break;
                    }
                }
            }
            overrideController.ApplyOverrides(overrides);
            AssetDatabase.CreateAsset(overrideController, $"{AssetDatabase.GetAssetPath(targetAnimatorFolder)}/{sourceAssetFolderName}Controller.overrideController");
        }

        Debug.Log("Done");

    }

    [ContextMenu("Convert Test")]
    public void Convert()
    {
        string[] GUIDs = AssetDatabase.FindAssets($"t:prefab", new string[] { AssetDatabase.GetAssetPath(characterModelFolder) });
        foreach (string guid in GUIDs)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (prefab.GetComponent<CharacterModel>() == null)
            {
                Debug.Log($"캐릭터 모델이 아님:{prefab.name}");
                continue;
            }
            var monsterAssetAnimator = prefab.GetComponent<Animator>();
            if (monsterAssetAnimator == null)
            {
                Debug.Log($"컨버팅 대상이 아님:{prefab.name}");
                continue;
            }
            AnimatorController originalControllerAsset = monsterAssetAnimator.runtimeAnimatorController as AnimatorController;

            string sourceAssetPath = AssetDatabase.GetAssetPath(originalControllerAsset);
            string sourceAssetDirectory = Path.GetDirectoryName(sourceAssetPath);
            string sourceAssetFolderName = Path.GetFileName(sourceAssetDirectory);

            if (sourceAssetFolderName == "Monsters")
            {
                Debug.Log("이미 완료한 작업");
                continue;
            }

            AnimatorOverrideController generatedAnimCtrl = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>($"{AssetDatabase.GetAssetPath(targetAnimatorFolder)}/{sourceAssetFolderName}Controller.overrideController");
            if (generatedAnimCtrl == null)
            {
                Debug.Log("이게아닌데");
                continue;
            }
            monsterAssetAnimator.runtimeAnimatorController = generatedAnimCtrl;
            EditorUtility.SetDirty(monsterAssetAnimator);
        }


    }

}
#endif //UNITY_EDITOR