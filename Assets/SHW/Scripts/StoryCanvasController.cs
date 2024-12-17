using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCanvasController : MonoBehaviour
{
    [SerializeField] private GameObject storyPrefab;

    public void PlayStory()
    {
        Instantiate(storyPrefab);
    }

    public void CloseStory()
    {
        Destroy(gameObject);
    }
}
