using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace BSM_Character_State
{
    public class CharacterState : CommonBind
    { 
        [field: SerializeField] public int UnitID { get; set; }

        private Slider _hpBar;
        public int AttackDamage;
        private int _hp;  
        public int HP
        {
            get => _hp;
            set
            {
                _hp = value;
                _hpBar.value = HP * 0.1f;
                
                if (_hp < 1)
                {
                    //캐릭터 사망 조건            
                } 
            }
        }
         
        private void Awake()
        {
            Bind(); 
            _hp = 50;
            _hpBar = GetCommonComponent<Slider>("HP_Bar");  
        }
    }
}


