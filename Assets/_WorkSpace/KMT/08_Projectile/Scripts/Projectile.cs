using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    ElementType projectileEleType;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void StartChase(Combatable target, float damage, float igDefRate, Sprite sprite, ElementType attackerType)
    {
        spriteRenderer.sprite = sprite;
        projectileEleType = attackerType;
        StartCoroutine(ChaseCO(target, damage, igDefRate));
    }

    IEnumerator ChaseCO(Combatable target, float damage, float igDefRate)
    {
        Vector3 moveDir = (target.transform.position - transform.position).normalized;

        transform.LookAt(target.transform);

        float trackTime = 0.2f;
        float time = 0;

        while (target != null && 0.2f < Vector3.SqrMagnitude(target.transform.position - transform.position))
        {
            if (time > trackTime)
            {
                time = 0;
                moveDir = (target.transform.position - transform.position).normalized;
                transform.LookAt(target.transform);
            }

            //TODO : 투사체 속도 상수 제거
            transform.position += 10 * moveDir.normalized * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        if (target.IsAlive)
        {
            target.Damaged(damage, igDefRate, projectileEleType);
        }

        Destroy(gameObject);

    }
}
