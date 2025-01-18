using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedEffect : MonoBehaviour
{
    [SerializeField]
    ParticleSystem damagedParticle;

    ObjectPool<ParticleSystem> damagedPool;
    private void Awake()
    {
        damagedPool = new ObjectPool<ParticleSystem>(damagedParticle, true);
    }


    /// <summary>
    /// 피격 이펙트 생성
    /// </summary>
    /// <param name="position">생성될 중심 위치</param>
    /// <param name="range">랜덤생성될 범위</param>
    public void PlayDamagedParticle(Vector3 position, float range)
    {
        ParticleSystem item = damagedPool.GetItem();
        item.transform.position = position + new Vector3(Random.Range(-range, range), position.y, Random.Range(-range, range));
        StartCoroutine(DamagedEffectCO(item));
    }

    IEnumerator DamagedEffectCO(ParticleSystem item)
    {
        item.Play();
        yield return new WaitForSeconds(1);
        damagedPool.BackItem(item);
    }
}
