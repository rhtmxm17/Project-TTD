using System;
using System.Collections;
using System.Collections.Generic;
using BSM_Character_State;
using UnityEngine;
using UnityEngine.Serialization;
using CharacterState = BSM_Character_State.CharacterState;


namespace _WorkSpace.BSM.Scripts.PartyOrg
{
    public class CircleChild : MonoBehaviour
    {
        [HideInInspector] public CharacterState Character;
        
        private Coroutine _childCo;
        
    
        private void Awake()
        {
            _childCo = StartCoroutine(ChildCoroutine());
        
        }

        private IEnumerator ChildCoroutine()
        {
            while (true)
            {
                if (transform.childCount != 0)
                {
                    if (Character != transform.GetChild(0).GetComponent<CharacterState>())
                    {
                        Character = transform.GetChild(0).GetComponent<CharacterState>();
                        
                        //TODO: 캐릭터 속성 검사
                        //여기서 속성들을 검사해서
                        //1을 증가 시킨다?
                        
                        
                        
                    }
                }
                else
                {
                    Character = null;
                }
                
                yield return null; 
            }    
        
        }
    
    }
}



