using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "test/CsvTestSO")]
public class CSVTestSO : ScriptableObject
{

    [SerializeField]
    [TextArea(2,10)]
    public string text;

}
