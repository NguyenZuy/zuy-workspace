using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIFollow : MonoBehaviour {

    protected NavMeshAgent agent;
    public Transform target;

    protected void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }

    protected void Update() {
        agent.destination = target.position;
        agent.isStopped = false;
    }

}
