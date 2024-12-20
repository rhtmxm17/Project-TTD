using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Skill : ScriptableObject
{
    public IEnumerator SkillRoutine(Combatable self, UnityAction onCompletedCallback)
    {
        yield return SkillRoutineImplement(self);
        onCompletedCallback?.Invoke();
    }

    protected abstract IEnumerator SkillRoutineImplement(Combatable self);
}
