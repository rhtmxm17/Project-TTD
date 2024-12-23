using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Skill : ScriptableObject
{
    [SerializeField] Sprite skillSprite;
    public Sprite SkillSprite => skillSprite;

    public IEnumerator SkillRoutine(Combatable self, UnityAction onCompletedCallback)
    {
        yield return SkillRoutineImplement(self);
        onCompletedCallback?.Invoke();
    }

    protected abstract IEnumerator SkillRoutineImplement(Combatable self);
}
