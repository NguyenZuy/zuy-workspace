using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Suriyun.MCS;
using System;

public class NavMeshActor : MonoBehaviour {

    public UniversalButton inputMove;
    public UniversalButton skillButton;
    public SkillCanceller skillCanceller;

    public GameObject skillMarker;
    public Transform destination;

    protected NavMeshAgent agent;


    public GameObject minionPrefab;

    protected virtual void Start() {
        agent = GetComponent<NavMeshAgent>();
        for (int i = 0; i < 10; i++) {
            SpawnSkillAt(Vector3.zero);
        }
    }

    Ray ray;
    RaycastHit hit;
    protected virtual void Update() {
        ray = new Ray(transform.position + inputMove.directionXZ, Vector3.down);
        Physics.Raycast(ray, out hit);

        if (inputMove.isFingerDown) {
            agent.isStopped = false;
            destination.position = hit.point;
            agent.destination = destination.position;
        } else {
            agent.isStopped = true;
        }

        if (skillCanceller.isAnyFingerDown) {
            skillMarker.SetActive(true);
            skillMarker.transform.position = GetSkillMarkerPosition();
        } else {
            skillMarker.SetActive(false);
        }
    }

    protected float skillRange = 5f;
    protected Vector3 GetSkillMarkerPosition() {
        return this.transform.position + skillButton.directionXZ * skillRange;
    }

    #region === Event Handler ===
    public virtual void OnEnable() {
        skillButton.onActivateSkill.AddListener(OnActivateSkill);
        skillButton.onCancelSkill.AddListener(OnCancelSkill);
    }

    public virtual void OnDisable() {
        skillButton.onActivateSkill.RemoveListener(OnActivateSkill);
        skillButton.onCancelSkill.RemoveListener(OnCancelSkill);
    }

    protected virtual void OnActivateSkill(int btnId) {
        Debug.Log("[Demo] " + "OnActivateSkill");
        SpawnSkillAt(skillMarker.transform.position);
        //skillMarker.SetActive(false);
        skillMarker.transform.position = this.transform.position;
    }

    protected virtual void OnCancelSkill(int i) {
        Debug.Log("[Demo] " + "OnCancelSkill");
        //skillMarker.SetActive(false);
        skillMarker.transform.position = this.transform.position;
        skillButton.directionXZ = Vector3.zero;
    }

    protected virtual void SpawnSkillAt(Vector3 pos) {
        Debug.Log("[Demo] " + "Spawn minion at : " + pos.ToString());
        tmpGo = Instantiate(minionPrefab, pos, Quaternion.identity);
        tmpGo.GetComponent<AIFollow>().SetTarget(this.transform);
    }


    protected GameObject tmpGo;
    #endregion
}
