using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterImmune : MonoBehaviour
{
    [SerializeField]
    GameObject characterCombManager;

    bool isClicked = false;

    public void OnClick()
    {

        isClicked = true;

        var characters = characterCombManager.GetComponentsInChildren<CharacterCombatable>();

        if (characters.Length > 0)
        {
            foreach (var character in characters)
            {

                character.defConst = float.MaxValue / 10 * 9;

            }
        }

    }

}
