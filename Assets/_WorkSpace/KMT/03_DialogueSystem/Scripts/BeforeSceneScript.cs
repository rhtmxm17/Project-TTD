using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeforeSceneScript : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(changeScene);
    }

    private void changeScene()
    {
        SceneManager.LoadScene("DialogueScene_kmt");
    }
}
