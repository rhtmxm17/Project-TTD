using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;
using System.IO;

[CreateAssetMenu()]
public class AnimatorConvert : ScriptableObject
{
    [SerializeField] AnimatorController baseControllerAsset;

    [SerializeField] CharacterModel model;
    [SerializeField] AnimatorController controllerAsset;
    [SerializeField] AnimationClip[] Clips;

    [ContextMenu("Create Override Test")]
    private void CreateOverrideController()
    {
        AnimatorOverrideController overrideController = new AnimatorOverrideController(baseControllerAsset);
        //overrideController.ApplyOverrides()

        AssetDatabase.CreateAsset(overrideController, "Assets/_WorkSpace/Animators/ScriptCreated.overrideController");
        Debug.Log("Done");

    }

    [ContextMenu("Convert Test")]
    private void Convert() => Convert(model);

    public void Convert(CharacterModel modelPrefab)
    {

        var animController = modelPrefab.GetComponent<Animator>().runtimeAnimatorController;
        controllerAsset = animController as AnimatorController;

        string sourceAssetPath = AssetDatabase.GetAssetPath(controllerAsset);
        string sourceAssetDirectory = Path.GetDirectoryName(sourceAssetPath);
        string sourceAssetFolder = Path.GetFileName(sourceAssetDirectory);
        Debug.Log(sourceAssetFolder);

        Clips = controllerAsset.animationClips;
        foreach (var animClip in Clips)
        {
            Debug.Log($"애니메이션 클립:{animClip.name}");
        }

        foreach (var childState in controllerAsset.layers[0].stateMachine.states)
        {
            Debug.Log($"상태 머신:{childState.state.name}/모션:{childState.state.motion.name}");
            Debug.Log($"애니메이션 클립:{(childState.state.motion as AnimationClip)?.name}");

        }

        EditorUtility.SetDirty(this);
    }

}
