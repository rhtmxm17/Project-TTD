
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

struct StatIncreaseBlock 
{
    public float hpIncrease{ get; private set;}
    public float atkIncrease{ get; private set; }
    public float defIncrease{ get; private set; }
    public int needCost{ get; private set; }

    public StatIncreaseBlock(float hpIncrease, float atkIncrease, float defIncrease, int needCost)
    { 
        this.hpIncrease = hpIncrease;
        this.atkIncrease = atkIncrease;
        this.defIncrease = defIncrease;
        this.needCost = needCost;
    }

}

public class CharacterCombatable : Combatable
{

    enum curState { WAITING, OTHERS }

    curState state = curState.WAITING;

    Vector3 originPos;

    ReactiveProperty<int> stageLevel = new ReactiveProperty<int>();
    public ReadOnlyReactiveProperty<int> StageLevel;
    StatIncreaseBlock[] levelIncreasement = new StatIncreaseBlock[4];
    const int MAX_STAGE_LEVEL = 3;

    protected override void Awake()
    {
        base.Awake();

        originPos = transform.position;
        waveClearEvent.AddListener(BackToOriginPos);

        StageLevel = stageLevel.ToReadOnlyReactiveProperty();
    }

    public void InitCharacterData(BasicSkillButton basicSkillButton, LevelupButton levelupButton, SecondSkillButton secondSkillButton)
    {
        if (characterData == null)
        {
            Debug.LogError($"캐릭터 데이터 등록이 선행되어야 함");
        }

        basicSkillButton.transform.GetChild(0).GetComponent<Image>().sprite = characterData.NormalSkillIcon;
        basicSkillButton.InitTargetingFunc(() => { return characterData.SkillDataSO.TargetingLogic.GetTarget(this) != null ? true : false; });
        basicSkillButton.GetComponent<Button>().onClick.AddListener(() => {
            if (!basicSkillButton.Interactable || !IsAlive) { Debug.Log("사용 불가");  return; }
            if (!OnSkillCommanded(characterData.SkillDataSO, 1)) { Debug.Log("스킬 타깃이 없음.  또는 더 큰 우선순위의 행동중  사용 취소"); basicSkillButton.DisplayNonTargetText(); return; }
            basicSkillButton.StartCoolDown(characterData.StatusTable.BasicSkillCooldown);//쿨타임을 매개변수로 전달.
        });

        secondSkillButton.transform.GetChild(0).GetComponent<Image>().sprite = characterData.SpecialSkillIcon;
        secondSkillButton.InitTargetingFunc(() => { return characterData.SecondSkillDataSO.TargetingLogic.GetTarget(this) != null ? true : false; });
        secondSkillButton.SetSkillCooltime(characterData.StatusTable.SecondSkillCooldown);
        secondSkillButton.GetComponent<Button>().onClick.AddListener(() => {
            if (!secondSkillButton.Interactable || !IsAlive) { Debug.Log("사용 불가"); return; }
            if (!secondSkillButton.LevelArrived) { Debug.Log("만랩이 아님, 사용 불가"); return; }
            if (!OnSkillCommanded(characterData.SkillDataSO, 3)) { Debug.Log("스킬 타깃이 없음. 또는 더 큰 우선순위의 행동중 사용 취소"); return; }
                secondSkillButton.StartCoolDown();
        });

        characterModel.transform.localRotation = Quaternion.Euler(new Vector3(-90, -90, -90));

        stageLevel.Value = 1;

        //레벨업 스텟 상승치 지정.
        levelIncreasement[2] = new StatIncreaseBlock(maxHp.Value * 0.1f, attackPoint.Value * 0.1f, defense.Value * 0.1f, 1);
        levelIncreasement[3] = new StatIncreaseBlock(maxHp.Value * 0.15f, attackPoint.Value * 0.15f, defense.Value * 0.15f, 2);

        levelupButton.GetComponent<Button>().onClick.AddListener(() => {

            if (!levelupButton.Interactable || !IsAlive) { Debug.Log("사용 불가"); return; }
            if (stageLevel.Value >= MAX_STAGE_LEVEL) { Debug.Log("만랩"); return; }
            if(characterModel.NextEvolveModel == null) { Debug.Log("진화할 모델이 지정되지 않음"); return; }
            if(curActionPriority > 2) { Debug.Log("더 큰 우선순위의 행동중"); return; }
            if (levelIncreasement[stageLevel.Value + 1].needCost < StageManager.Instance.PartyCost)//비용이 충분한지 확인.
            {
                curActionPriority = 2;
                SetCharacterModel(characterModel.NextEvolveModel);
                StageManager.Instance.UsePartyCost(levelIncreasement[stageLevel.Value + 1].needCost);
                LevelUp();

                //TODO : 인터럽트 방지 우선순위가 필요하다면 우선순위를 돌려놓는 콜백을 추가하여 적용시킬 것.
                //애니메이션이나 쿨타임등의 행동 뒤에 돌려놓을것.
                curActionPriority = 0;
            }

        });

        onDeadEvent.AddListener(basicSkillButton.OffSkillButton);
        onDeadEvent.AddListener(secondSkillButton.OffSkillButton);
        onDeadEvent.AddListener(levelupButton.OffSkillButtonOnDead);

        stageLevel.Subscribe((x) => {

            if (x >= MAX_STAGE_LEVEL)
            {
                basicSkillButton.SetLevel(x);
                levelupButton.SetLevel(x);
                levelupButton.SetLevelupCost(int.MaxValue);
                secondSkillButton.ArrivedReqLevel();
            }
            else
            {
                basicSkillButton.SetLevel(x);
                levelupButton.SetLevel(x);
                levelupButton.SetLevelupCost(levelIncreasement[stageLevel.Value + 1].needCost);
                Debug.Log("레벨업!" + x);
            }

        });


        //=============HP 게이지 위치 조정==================

        RectTransform hpBarRect = hpSlider.GetComponent<RectTransform>();
        hpSlider.transform.SetParent(secondSkillButton.transform);
        hpBarRect.anchoredPosition = new Vector3(0, -100, 5);
        hpBarRect.sizeDelta = new Vector2(140, 60);
        hpBarRect.rotation = Quaternion.identity;
        hpBarRect.localScale = Vector3.one;

    }
    public override void StartCombat(CombManager againstL)
    {
        state = curState.OTHERS;
        base.StartCombat(againstL);
    }

    protected override void SetCharacterModel(CharacterModel modelData)
    {
        base.SetCharacterModel(modelData);
        characterModel.transform.rotation = Quaternion.Euler(-90, -90, -90);
    }

    public bool IsWaiting()
    {
        return state == curState.WAITING;
    }

    void BackToOriginPos()
    {
        StopCurActionCoroutine();
        curActionCoroutine = StartCoroutine(BackToPosCO());
    }


    IEnumerator BackToPosCO()
    {
        yield return null;

        agent.stoppingDistance = 0.05f;
        agent.destination = originPos;

        yield return new WaitWhile(() => agent.pathPending);

        Look(originPos);

        while (0.1f < agent.remainingDistance)
        {
            Look(originPos);
            yield return null;
        }

        transform.position = originPos;

        agent.stoppingDistance = range;//TODO : 개체별 크기가 다른 경우, 해당 로직에 추가 수정.

        characterModel.transform.localRotation = Quaternion.Euler(-90, -90, -90);

        state = curState.WAITING;
    }

    public void LevelUp()
    {
        //만랩 이전이라면 레벨업시킴.
        if(stageLevel.Value < MAX_STAGE_LEVEL)
        {        
            stageLevel.Value += 1;
            maxHp.Value += levelIncreasement[stageLevel.Value].hpIncrease;
            hp.Value += levelIncreasement[stageLevel.Value].hpIncrease;
            attackPoint.Value += levelIncreasement[stageLevel.Value].atkIncrease;
            defense.Value += levelIncreasement[stageLevel.Value].defIncrease;
        }
    }

}