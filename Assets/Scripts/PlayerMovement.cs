using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float movementSpeed = 5.0f;
    
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;

    private float lastMovementSpeed;

    void Start() {
        characterController = GetComponent<CharacterController>();
    }
    void Update() {
        if (characterController.isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection = moveDirection * movementSpeed;
        }
        //Gravity
        moveDirection.y -= 10f * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    public void FreezeControls(bool freeze) {
        if (freeze) {
            lastMovementSpeed = movementSpeed;
            movementSpeed = 0;
        } else {
            movementSpeed = lastMovementSpeed;
        }
    }
}