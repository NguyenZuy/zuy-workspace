using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suriyun.MCS;

public class CCActor : MonoBehaviour {

    public Camera cam;
    public UniversalButton inputMove;
    public float moveSpeed;

    protected CharacterController controller;

    protected virtual void Start() {
        controller = gameObject.AddComponent<CharacterController>();
    }

    protected Vector3 desiredDirection;
    protected virtual void Update() {
        this.UpdateMovementAxis();

        if (inputMove.isFingerDown) {
            desiredDirection = inputMove.directionXZ.x * horizontalAxis + inputMove.directionXZ.z * verticalAxis;
            transform.forward = desiredDirection;
        } else {
            desiredDirection = Vector3.zero;
        }

        controller.Move(desiredDirection * Time.deltaTime * moveSpeed);
    }

    protected Vector3 verticalAxis;
    protected Vector3 horizontalAxis;
    protected virtual void UpdateMovementAxis() {
        verticalAxis = cam.transform.forward;
        verticalAxis.y = 0;
        verticalAxis.Normalize();

        horizontalAxis = cam.transform.right;
        horizontalAxis.y = 0;
        horizontalAxis.Normalize();
    }

}
