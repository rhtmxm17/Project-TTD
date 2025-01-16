using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GraphicSettingPanel : MonoBehaviour
{
    [SerializeField] Toggle[] gradeToggles;
    [SerializeField] RenderPipelineAsset[] renderPipelineAssets;

    private void Awake()
    {
        // QualityLevel의 번호는 Project Settings->Quality에서 위에서부터 0번
        int currentLevel = UserSettingData.Instance.Data.graphicQualityLevel;

        for (int i = 0; i < gradeToggles.Length; i++)
        {
            gradeToggles[i].isOn = (i == currentLevel); // 현재 설정된 레벨의 토글을 켜기

            // RenderPipelineAsset switchTo = renderPipelineAssets[i];
            int qualityLevel = i;
            gradeToggles[i].onValueChanged.AddListener(isOn =>
            {
                // 이 토글이 true로 변경되었다면
                if (isOn)
                {
                    string from = GraphicsSettings.currentRenderPipeline.name;
                    QualitySettings.SetQualityLevel(qualityLevel);
                    Debug.Log($"{from}->{GraphicsSettings.currentRenderPipeline.name}");
                }
            });
        }
    }

    /// <summary>
    /// 지정한 렌더 파이프라인으로 교체
    /// </summary>
    /// <param name="switchTo"></param>
    private void SwitchRenderPipeline(RenderPipelineAsset switchTo)
    {
        GraphicsSettings.defaultRenderPipeline = switchTo;
    }
}
