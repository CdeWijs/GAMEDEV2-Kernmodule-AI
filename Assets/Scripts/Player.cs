using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public Animator animator;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update() {
        Debug.Log(playerMovement.movement);
        if (playerMovement.movement == PlayerMovement.Movement.Idle) {
            animator.SetBool("running", false);
        } else {
            animator.SetBool("running", true);
        }
    }

}
