using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using CharacterState = BSM_Character_State.CharacterState;

namespace _WorkSpace.BSM.Scripts.PartyOrg
{
    public class BuffSystem : MonoBehaviour
    {
        public string TestBuff = "";

        public int FrontCount { get; set; }
        public int MiddleCount { get; set; }
        public int BackCount { get; set; }

        private bool isCheck;
        private int _totalCount;

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;

                if (_totalCount < 5)
                {
                    isCheck = false;
                }

                if (_totalCount == 5 && !isCheck)
                {
                    isCheck = true;
                    OnTypeUpdateEvent?.Invoke();
                }
            }
        }

        public UnityEvent OnTypeUpdateEvent = new();

        private BuffType _curBuffType;
        private BuffType _nextBuffType;

        private Dictionary<UnitType, int> _unitTypeDict = new Dictionary<UnitType, int>()
        {
            { UnitType.FIRE, 0 },
            { UnitType.GRASS, 0 },
            { UnitType.WATER, 0 },
            { UnitType.ELECTRIC, 0 },
            { UnitType.GROUND, 0 },
        };

        public void CurrentBuff()
        {
            if (FrontCount == 0 || MiddleCount == 0 || BackCount == 0)
            {
                _curBuffType = BuffType.NONE;
                TestBuff = _curBuffType.ToString();
                return;
            }

            _nextBuffType = (BuffType)(1 << (FrontCount * 3) + (MiddleCount * 2));
            if (_nextBuffType.Equals(_curBuffType)) return;
            if (!Enum.IsDefined(typeof(BuffType), _nextBuffType)) return;

            _curBuffType = _nextBuffType;
            TestBuff = _curBuffType.ToString();
        }

        public void UpdateTypeBuff(CircleChild[] childList)
        {
            Debug.Log(childList.Length);
            //TODO: 속성 개수 구하기.
            //childList가 가지고 있는 속성들의 개수를 모두 더함?
            for (int i = 0; i < childList.Length; i++)
            {
                if (childList[i].Character != null)
                {
                    Debug.Log($"{i}Key : {childList[i].ReturnUnitType} / {childList[i].ReturnValue}");
                }
            }
        }

        private void TotalUnitType()
        {
            Debug.Log($"불 속성 개수 :{_unitTypeDict[UnitType.FIRE]}");
            Debug.Log($"물 속성 개수 :{_unitTypeDict[UnitType.WATER]}");
            Debug.Log($"번개 속성 개수 :{_unitTypeDict[UnitType.ELECTRIC]}");
            Debug.Log($"풀 속성 개수 :{_unitTypeDict[UnitType.GRASS]}");
            Debug.Log($"땅 속성 개수 :{_unitTypeDict[UnitType.GROUND]}");
        }
    }
}