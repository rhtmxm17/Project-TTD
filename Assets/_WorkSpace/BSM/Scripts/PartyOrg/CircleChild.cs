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
        private CharacterState _compareCharacter;

        private Coroutine _childCo;
        private UnitType _curUnitType;

        private Dictionary<UnitType, int> _checkDict = new Dictionary<UnitType, int>()
        {
            { UnitType.FIRE, 0 },
            { UnitType.GRASS, 0 },
            { UnitType.WATER, 0 },
            { UnitType.GROUND, 0 },
            { UnitType.ELECTRIC, 0 }
        };


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
                    Debug.Log($"나 자식 있음:{gameObject.name}");
                    
                    _compareCharacter = transform.GetChild(0).GetComponent<CharacterState>();

                    if (Character != _compareCharacter)
                    {
                        Character = _compareCharacter;

                        if (!_curUnitType.Equals(Character.CurUnitType))
                        {
                            _curUnitType = Character.CurUnitType;

                            UnitTypeCheck(_curUnitType);
                        } 
                    }
                }
                else
                {
                    Debug.Log($"난 자식 없음 :{gameObject.name}");
                    _curUnitType = UnitType.NONE;
                    Character = null;
                }

                yield return null;
            }
        }

        private void UnitTypeCheck(UnitType unitType)
        {
            _checkDict[UnitType.FIRE] = 0;
            _checkDict[UnitType.GRASS] = 0;
            _checkDict[UnitType.WATER] = 0;
            _checkDict[UnitType.ELECTRIC] = 0;
            _checkDict[UnitType.GROUND] = 0;
 
            _checkDict[unitType] = 1;
        }
        
        public int ReturnValue => _checkDict[_curUnitType];
        public UnitType ReturnUnitType => _curUnitType;

    }
}