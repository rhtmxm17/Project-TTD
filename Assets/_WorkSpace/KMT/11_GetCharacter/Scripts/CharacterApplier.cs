using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterApplier
{

    public static void ApplyCharacter(int characterId)
    {
        Debug.Log(UserDataManager.Instance.HasCharacter(characterId));

        //없으면 추가
        if (!UserDataManager.Instance.HasCharacter(characterId))
        {
            UserDataManager.Instance.ApplyCharacter(characterId);
        }
        else//중복이면 다른 재화 주는 코드 추가 
        {
            Debug.Log("중복!");
            //TODO : 재화추가코드
        }

       
    }

}
