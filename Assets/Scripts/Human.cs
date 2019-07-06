using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Human : MonoBehaviour {
    public float walkSpeed = 3.5f;
    public float jumpSpeed = 5.5f;
    public float rightPos;
    public float leftPos;
    public float upPos;

    public Player player;
    private Vector3 destination;

    private List<Human> humans = new List<Human>();

    void Start() {
        player = FindObjectOfType<Player>();
    }

    #region Tasks

    [Task]
    bool IsPlayerNear = false;

    [Task]
    void CheckPlayer() {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer < 8.0f) {
            IsPlayerNear = true;
            Task.current.Succeed();
        } else {
            IsPlayerNear = false;
            Task.current.Fail();
        }
        Task.current.Succeed();
    }

    [Task] 
    void SetDestinationToPlayer() {
        destination = player.transform.position;
        Task.current.Succeed();
    }

    [Task] 
    void MoveToDestination() {
        Vector3 distance = destination - transform.position;

        if (transform.position != destination) {
            Vector3 velocity = distance.normalized * walkSpeed;
            transform.position = transform.position + velocity * Time.deltaTime;

            Vector3 newDistance = destination - transform.position;
            if (Vector3.Dot(newDistance, distance) < 0.0f) {
                transform.position = destination;
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < 2.0f) {
            Task.current.Succeed();
        } else {
            Task.current.Fail();
        }
    }
    #endregion
}
