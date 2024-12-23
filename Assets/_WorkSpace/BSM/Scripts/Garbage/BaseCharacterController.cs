using System;
using System.Collections;
using System.Collections.Generic;
using BSM_Character_State;
using UnityEngine;
using CharacterState = BSM_Character_State.CharacterState;

namespace BSM_Chracter_BaseController
{
    public class BaseCharacterController : MonoBehaviour
    {
        
        protected CharacterState _characterState; 
        private void Awake() => Init();

        private void Init()
        {
            // _characterState = GetComponent<CharacterState>();
            // SetCharacter();
        }
    
        private void SetCharacter()
        {
            string path = ""; 
            int category = _characterState.UnitID / 1000;

            switch (category)
            {
                case 1:
                    path = $"Sprite/Unit/{_characterState.UnitID}";
                    break;
                case 4:
                    path = $"Sprite/Enemy/{_characterState.UnitID}";
                    break;
            }
             
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path);
             
        }

    }
}


