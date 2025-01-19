using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

/// <summary>
/// 준비 동작 후 즉발로 모든 적에게 피해를 입히는 샘플
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Skill/AllTargetDamage")]
public class AllTargetDamage : Skill
{
    [SerializeField] float atkMultiplier = 1f;
    // 스킬이펙트
    [SerializeField] ParticleSystem hitEffect;
    
    // 캐싱 데이터
    private WaitForSeconds waitPreDelay;
    private WaitForSeconds waitPostDelay;

    private void OnEnable()
    {
        // 런타임 진입시 필요한 데이터 캐싱
        waitPreDelay = new WaitForSeconds(preDelay);
        waitPostDelay = new WaitForSeconds(postDelay);
    }

    protected override IEnumerator SkillRoutineImplement(Combatable self, Combatable target)
    {
        yield return waitPreDelay;

        self.PlayAttckSnd();

        //도중 반복자 순회 오류 대체코드
        List<Combatable> dest = new List<Combatable>();

        if (self.Group.Enemy != null)
        {
            foreach (Combatable enemy in self.Group.Enemy.CharList) 
            {
                // 실제로 공격이 적용되는 구간
                if (enemy != null && enemy.IsAlive)
                {
                    dest.Add(enemy);
                    if (hitEffect != null)
                    {
                        float rndX = Random.Range(-5, 15);
                        float rndZ = Random.Range(-1, 2);
                        Instantiate(hitEffect, new Vector3(rndX,4,rndZ), Quaternion.Euler(90,90,90));
                        // TODO: ricochet(피격효과) 나온는 위치에서 나오게 하면될거같은데 지금 잘모르겠음
                        
                        
                    }
                }
                    //enemy.Damaged(self.AttackPoint.Value * atkMultiplier, self.igDefenseRate, self.characterData.StatusTable.type); // 타겟에게 데미지 적용
            }
        }

        //도중 반복자 순회 오류 대체코드
        foreach (Combatable enemy in dest)
        {
            enemy.Damaged(self.CurAttackPoint * atkMultiplier, self.igDefenseRate, self.characterData.StatusTable.type); // 타겟에게 데미지 적용
        }


        // 히트스캔 이펙트 추가

        yield return waitPostDelay;
    }
}
