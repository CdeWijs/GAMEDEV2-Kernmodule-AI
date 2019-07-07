using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public Movement movement;
    public enum Movement {
        Idle,
        Run
    }

    private float movementSpeed = 2.5f;
    private float rotationSpeed = 10f;
    private float direction = 0;

    private Vector3 moveDirection = Vector3.zero;

    private float lastMovementSpeed;
    
    void Update() {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection = moveDirection * movementSpeed;
        
        GetComponent<Rigidbody>().MovePosition(transform.position + moveDirection * movementSpeed * Time.deltaTime);
        
        if (Input.GetAxis("Vertical") == 1 && Input.GetAxis("Horizontal") == 1) {
            direction = 45;
        } else if (Input.GetAxis("Vertical") == -1 && Input.GetAxis("Horizontal") == 1) {
            direction = 135;
        } else if (Input.GetAxis("Vertical") == 1 && Input.GetAxis("Horizontal") == -1) {
            direction = -45;
        } else if (Input.GetAxis("Vertical") == -1 && Input.GetAxis("Horizontal") == -1) {
            direction = -135;
        } else if (Input.GetAxis("Vertical") == -1 && Input.GetAxis("Horizontal") == 0) {
            direction = 180;
        } else if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 1) {
            direction = 90;
        } else if (Input.GetAxis("Vertical") == 1 && Input.GetAxis("Horizontal") == 0) {
            direction = 0;
        } else if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == -1) {
            direction = -90;
        }

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) {
            movement = Movement.Run;
        } else {
            movement = Movement.Idle;
        }

        Vector3 v3 = new Vector3(0 , direction, 0);
        Quaternion qTo = Quaternion.LookRotation(v3);
        Quaternion rot = Quaternion.Euler(v3);

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
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