using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Human : MonoBehaviour {

    public GameObject player;
    public Transform startPos;

    private float walkSpeed = 2.25f;
    private float jumpSpeed = 5.5f;
    private Transform target;
    public int hugPlayerTime;

    private Vector3[] path;
    private int targetIndex;
    private Vector3 destination;
    private List<Human> humans = new List<Human>();
    private bool finishedPath = true;

    void Start() {
        target = player.transform;
        transform.position = startPos.position;
       // PathRequestManager.RequestPath(transform.position, player.position, OnPathFound);
    }

    #region Pathfinding
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        if (pathSuccessful) {
            Debug.Log("Path is succesful");
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath() {
        Vector3 currentWaypoint = path[0];

        Debug.Log("Current waypoint: " + currentWaypoint);
        Debug.Log("Position: " + transform.position);
        Debug.Log("Player position: " + target.position);
        
        while (true) {
            if (transform.position == currentWaypoint) {
                targetIndex++;
                if (targetIndex >= path.Length) {
                    finishedPath = true;
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, walkSpeed / 10);
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
    void CheckPlayer() {
        
        float distanceToPlayer = Vector3.Distance(target.position, transform.position);
        if (distanceToPlayer < 15.0f) {
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
            IsChasingTarget = true;
            finishedPath = false;
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            Task.current.Succeed();
        }
    }

    [Task]
    void CheckIfReachedTarget() {
        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        Debug.Log(target);
        if (distanceToTarget < 2.0f) {
            IsChasingTarget = false;
            finishedPath = true;
            Task.current.Succeed();
        } 
    }

    [Task]
    void HoldPlayer() {
        player.GetComponent<PlayerMovement>().FreezeControls(true);
        Task.current.Succeed();
    }

    [Task]
    void ReleasePlayer() {
        player.GetComponent<PlayerMovement>().FreezeControls(false);
        Task.current.Succeed();
    }

    [Task]
    void GoToStartPosition() {
        target = startPos;
        Debug.Log("Target = " + target);

        if (finishedPath) {
            IsChasingTarget = true;
            finishedPath = false;
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            Task.current.Succeed();
        }
    }

    #endregion
}
