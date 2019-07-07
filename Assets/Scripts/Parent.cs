using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Parent : MonoBehaviour {

    public GameObject player;
    public Child child;
    public Transform startPos;
    public Transform guardPos1;
    public Transform guardPos2;
    public Transform guardPos3;
    public Transform guardPos4;
    public Transform guardPos5;
    public Transform guardPos6;
    public float noticePlayerDistance;
    public float walkSpeed = 2f;

    private Transform currentTarget;
    private Vector3[] path;
    private int targetIndex;
    private bool finishedPath = true;

    void Start() {
        currentTarget = child.transform;
        transform.position = startPos.position;
       // PathRequestManager.RequestPath(transform.position, player.position, OnPathFound);
    }

    #region Pathfinding
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        if (pathSuccessful) {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath() {
        Vector3 currentWaypoint = path[0];
        
        while (true) {
            if (new Vector3(transform.position.x, 1, transform.position.z) == new Vector3(currentWaypoint.x, 1, currentWaypoint.z)) {
                targetIndex++;
                if (targetIndex >= path.Length) {
                    targetIndex = 0;
                    path = new Vector3[0];
                    finishedPath = true;
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentWaypoint.x, 1, currentWaypoint.z), walkSpeed / 10);
            yield return null;
        }
    }

    #endregion

    #region Tasks
    [Task]
    bool IsPlayerNear = false;

    [Task]
    bool IsChasingTarget = false;

    [Task]
    bool IsChildGone = false;

    [Task]
    void ResetVariables() {
        IsPlayerNear = false;
        IsChasingTarget = false;
        IsChildGone = false;
        child.AtStartPosition = true;
        finishedPath = true;
        currentTarget = child.transform;
        Task.current.Succeed();
    }

    [Task]
    void CheckChild() {
        if (!child.AtStartPosition) {
            IsChildGone = true;
            currentTarget = child.transform;
            Task.current.Succeed();
        }
    }

    [Task] 
    void RequestPathAndGoToTarget() {
        if (finishedPath) {
            IsChasingTarget = true;
            finishedPath = false;
            PathRequestManager.RequestPath(transform.position, currentTarget.position, OnPathFound);
            Task.current.Succeed();
        }
    }

    [Task]
    void CheckIfReachedTarget() {
        float distanceToTarget = Vector3.Distance(currentTarget.position, transform.position);
        if (distanceToTarget < 2.0f) {
            IsChasingTarget = false;
            finishedPath = true;
            Task.current.Succeed();
        } 
    }

    [Task]
    void PickupChild() {
        child.transform.parent = transform;
        child.IsCaptured = true;
        child.StopCoroutine("FollowPath");
        Task.current.Succeed();
    }

    [Task]
    void ReleaseChild() {
        child.transform.parent = null;
        child.IsCaptured = false;
        child.ResetVariables();
        Task.current.Succeed();
    }

    [Task]
    void ChangeTarget(string newTarget) {
        if (newTarget == "startPosChild") {
            currentTarget = child.startPos;
            Task.current.Succeed();
        } else if (newTarget == "startPos") {
            currentTarget = startPos;
            Task.current.Succeed();
        } else if (newTarget == "GuardPos1") {
            currentTarget = guardPos1;
            Task.current.Succeed();
        } else if (newTarget == "GuardPos2") {
            currentTarget = guardPos2;
            Task.current.Succeed();
        } else if (newTarget == "GuardPos3") {
            currentTarget = guardPos3;
            Task.current.Succeed();
        } else if (newTarget == "GuardPos4") {
            currentTarget = guardPos4;
            Task.current.Succeed();
        } else if (newTarget == "GuardPos5") {
            currentTarget = guardPos5;
            Task.current.Succeed();
        } else if (newTarget == "GuardPos6") {
            currentTarget = guardPos6;
            Task.current.Succeed();
        } else {
            Task.current.Fail();
        }
    }

    #endregion
}
