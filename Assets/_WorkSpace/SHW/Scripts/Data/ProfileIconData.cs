using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProfileIconData")]
public class ProfileIconData : ScriptableObject
{
    [SerializeField] private Sprite[] icon;

}
