using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEffect : MonoBehaviour
{
    [SerializeField] float lifeTime = 2f;
    public RectTransform RectTransform => GetComponent<RectTransform>();

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
