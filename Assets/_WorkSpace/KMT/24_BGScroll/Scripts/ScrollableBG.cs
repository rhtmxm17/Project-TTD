using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollableBG : BaseUI
{
    List<MeshRenderer> renderers = new List<MeshRenderer>();

    public float[] scrollSpeed;

    protected override void Awake()
    {
        base.Awake();

        renderers.Add(GetUI<MeshRenderer>("FarC"));
        renderers.Add(GetUI<MeshRenderer>("FarB"));
        renderers.Add(GetUI<MeshRenderer>("FarA"));
        renderers.Add(GetUI<MeshRenderer>("Base"));
        renderers.Add(GetUI<MeshRenderer>("Front"));

        foreach (var renderer in renderers)
        {
            renderer.material.mainTextureOffset = Vector2.zero;
        }

    }

    public IEnumerator ScrollCO()
    {
        yield return null;

        while (true)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].material.mainTextureOffset += Vector2.right * scrollSpeed[i] * Time.deltaTime;
            }

            yield return null;
        }
    }

}
