using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    [field: SerializeField]
    public CharacterModel NextEvolveModel {  get; private set; }

    [field: SerializeField]
    public float ModelSize { get; private set; } = 2f;
}
