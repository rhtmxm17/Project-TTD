using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshTester : MonoBehaviour
{

    [SerializeField]
    Vector3 dest;

    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    [ContextMenu("tracking")]
    public void Track()
    {
        agent.destination = dest;
    }
}
