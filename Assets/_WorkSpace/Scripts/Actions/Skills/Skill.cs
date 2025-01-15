using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Skill : ScriptableObject
{

    [field: SerializeField]
    public Targeting TargetingLogic { get; protected set; }

    public IEnumerator SkillRoutine(Combatable self, Combatable target, UnityAction onCompletedCallback)
    {
        yield return null;
        yield return SkillRoutineImplement(self, target);
        onCompletedCallback?.Invoke();
    }

    protected abstract IEnumerator SkillRoutineImplement(Combatable self, Combatable target);
}
