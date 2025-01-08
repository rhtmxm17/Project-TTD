using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTest : MonoBehaviour
{
    [SerializeField] private int idx;
    
    [ContextMenu("이름")]
    public void test()
    {
        CharacterApplier.ApplyCharacter(idx);
    }
    
}
