using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Child : MonoBehaviour {

    public GameObject player;
    public Transform startPos;
    public float noticePlayerDistance;
    public float walkSpeed = 2f;

    private Transform target;
    private Vector3[] path;
    private int targetIndex;
    private bool finishedPath = true;

    void Start() {
        target = player.transform;
        transform.position = startPos.position;
       // PathRequestManager.RequestPath(transform.position, player.position, OnPathFound);
    }

    private IEnumerator JumpUpAndDown() {
        GetComponent<Rigidbody>().AddForce(new Vector3(0,1,0) * 5, ForceMode.Impulse);
        yield return new WaitForSeconds(1);
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
    public bool IsCaptured = false;

    [Task]
    public bool AtStartPosition = true;

    [Task]
    public void ResetVariables() {
        IsPlayerNear = false;
        IsChasingTarget = false;
        finishedPath = true;
        IsCaptured = false;
        AtStartPosition = true;
        target = player.transform;
        Task.current.Succeed();
    }

    [Task]
    void CheckPlayer() {
        float distanceToPlayer = Vector3.Distance(target.position, transform.position);
        if (distanceToPlayer < noticePlayerDistance) {
            IsPlayerNear = true;
            Task.current.Succeed();
        } else {
            IsPlayerNear = false;
            Task.current.Fail();
        }
    }

    [Task] 
    void RequestPathAndGoToTarget() {
        if (finishedPath) {
            AtStartPosition = false;
            IsChasingTarget = true;
            finishedPath = false;
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            Task.current.Succeed();
        }
    }

    [Task]
    void CheckIfReachedTarget() {
        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (distanceToTarget < 2.0f) {
            IsChasingTarget = false;
            finishedPath = true;
            if (target == startPos) {
                AtStartPosition = true;
            }
            Task.current.Succeed();
        } 
    }

    [Task]
    void HoldPlayer() {
        if (IsPlayerNear) {
            player.GetComponent<PlayerMovement>().FreezeControls(true);
            Task.current.Succeed();
        } else {
            Task.current.Fail();
        }
    }

    [Task]
    void ReleasePlayer() {
        if (IsPlayerNear) {
            player.GetComponent<PlayerMovement>().FreezeControls(false);
            Task.current.Succeed();
        } else {
            Task.current.Fail();
        }
    }

    [Task]
    void ChangeTarget(string newTarget) {
        if (newTarget == "startPos") {
            target = startPos;
            Task.current.Succeed();
        } else if (newTarget == "player") {
            target = player.transform;
            Task.current.Succeed();
        } else {
            Task.current.Fail();
        }
    }

    [Task]
    void Jump() {
        StartCoroutine("JumpUpAndDown");
        Task.current.Succeed();
    }

    #endregion
}
