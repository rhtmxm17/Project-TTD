using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Skill : ScriptableObject
{

    [field: SerializeField]
    public Targeting TargetingLogic { get; protected set; }

    [SerializeField, Tooltip("선딜레이")] protected float preDelay = 0.25f;
    [SerializeField, Tooltip("후딜레이")] protected float postDelay = 0.75f;

    public float PreDelay { get { return preDelay; } }

    public IEnumerator SkillRoutine(Combatable self, Combatable target, UnityAction onCompletedCallback)
    {
        yield return null;
        yield return SkillRoutineImplement(self, target);
        onCompletedCallback?.Invoke();
    }

    protected abstract IEnumerator SkillRoutineImplement(Combatable self, Combatable target);
}
