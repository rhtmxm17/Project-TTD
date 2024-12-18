using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Skill : ScriptableObject
{
    public event UnityAction onSkillCompleted;

    public IEnumerator SkillRoutine(SampleUnitClass self)
    {
        yield return SkillRoutineImplement(self);
        onSkillCompleted?.Invoke();
    }

    protected abstract IEnumerator SkillRoutineImplement(SampleUnitClass self);
}
