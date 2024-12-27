using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public int Id { get; set; }

    public Button Button => button;
    public TMP_Text Text => text;

    [SerializeField] Button button;
    [SerializeField] TMP_Text text;
}
