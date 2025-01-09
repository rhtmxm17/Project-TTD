using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoBattleLogic : MonoBehaviour
{

    List<BasicSkillButton> basicSkillButtons;
    List<LevelupButton> levelupButtons;
    List<SecondSkillButton> secondSkillButtons;

    Coroutine levelAutoCoroutine = null;
    WaitForSeconds waitDelay = new WaitForSeconds(0.1f);

    /// <summary>
    /// 초기화를 위한 함수.
    /// 편성된 캐릭터의 숫자를 전달.
    /// </summary>
    /// <param name="count">편성된 캐릭터의 수</param>
    public void InitLogicCount(int count)
    {
        basicSkillButtons = new List<BasicSkillButton>(count);
        levelupButtons = new List<LevelupButton>(count);
        secondSkillButtons = new List<SecondSkillButton>(count);
    }

    public void AddSkillButtons(BasicSkillButton basicSkillBtn, LevelupButton levelupBtn, SecondSkillButton secondSkillBtn)
    { 
        basicSkillButtons.Add(basicSkillBtn);
        levelupButtons.Add(levelupBtn);
        secondSkillButtons.Add(secondSkillBtn);
    }

    public void StartBasicSkill()
    {
        foreach (BasicSkillButton basicBtn in basicSkillButtons)
        {
            basicBtn.IsInAuto = true;
        }
    }

    public void OnAutoBattle()
    {

        if (levelAutoCoroutine != null)
        { 
            StopCoroutine(levelAutoCoroutine);
        }

        levelAutoCoroutine = StartCoroutine(LevelAutoCO());

        foreach (SecondSkillButton sndBtn in secondSkillButtons)
        {
            sndBtn.IsInAuto = true;
        }

    }
    public void OffAutoBattle()
    {
        if (levelAutoCoroutine != null)
        {
            StopCoroutine(levelAutoCoroutine);
            levelAutoCoroutine = null;
        }

        foreach (SecondSkillButton sndBtn in secondSkillButtons)
        {
            sndBtn.IsInAuto = false;
        }
    }

    IEnumerator LevelAutoCO()
    {
        yield return null;

        LevelupButton destButton = null;

        //만랩이거나 죽은 친구들은 리스트에서 제거.
        List<LevelupButton> victims = new List<LevelupButton>(5);
        foreach (LevelupButton lvBtn in levelupButtons)//현재 유효한 레벨 버튼들중에서 만랩이거나 죽은 캐릭터의 버튼을 찾음
        { 
            if(lvBtn.IsMaxLevel || !lvBtn.IsAlive)
                victims.Add(lvBtn);
        }

        if (victims.Count > 0)//제거 대상인 버튼들이 있다면, 유효 리스트에서 제거
        {
            foreach (LevelupButton victimButton in victims)
            {
                levelupButtons.Remove(victimButton);
            }
        }

        while (true)//유효 리스트를 가지고 연산 진행.
        {
            if (levelupButtons.Count <= 0)//유효한 버튼이 없었다. 종료.
            {
                levelAutoCoroutine = null;
                yield break;
            }

            destButton = null;

            //하나 선별
            while (destButton == null)
            {
                if (levelupButtons.Count <= 0)//찾아보니 없었다[모두 만랩이거나 죽은 캐릭터의 버튼]...종료
                {
                    levelAutoCoroutine = null;
                    yield break;
                }

                int randomIdx = Random.Range(0, levelupButtons.Count);//랜덤으로 선택.

                if (levelupButtons[randomIdx].IsMaxLevel || !levelupButtons[randomIdx].IsAlive)//찾아보니까 만랩이거나 죽은 친구가 골라졌다.
                {
                    levelupButtons.Remove(levelupButtons[randomIdx]);//유효 리스트에서 제거한 뒤 다시 탐색 진행
                }
                else
                {
                    destButton = levelupButtons[randomIdx];//선택 대상이 정해졌으므로 자동으로 while문 탈출
                }

            }

            int lastLevel = destButton.CurLevel;//레벨업 하기 전 레벨을 보관

            //레벨업까지 0.1초 주기로 레벨업 버튼을 누름. [ 이전의 저장 레벨과 다르다면 레벨업한 것으로 판정 ]
            while (lastLevel == destButton.CurLevel)
            {
                //도중 해당 버튼이 유효하지 않게 된 경우에는 유효 리스트에서 제거 후 중지.
                if (destButton.IsMaxLevel || !destButton.IsAlive)
                { 
                    levelupButtons.Remove(destButton);
                    break;
                }

                Debug.Log("렙업시도오오");

                destButton.LvButton.onClick.Invoke();
                yield return waitDelay;
            }

            //그게 만랩이 되거나 죽은거면 리스트에서 제거.
            if (destButton.IsMaxLevel || !destButton.IsAlive)
                levelupButtons.Remove(destButton);
           
        }

    }



}
