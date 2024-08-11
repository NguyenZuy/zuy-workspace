using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraSystem : MonoBehaviour {
    public Camera cam;
    public Transform target;
    protected Vector3 offset;
    public float speed;
    public float rotationSpeed;
    public FollowMode mode;

    private void Update() {
        if (mode == FollowMode.Absolute) {
            transform.position = target.position + offset;
            transform.rotation = target.rotation;
        }

        if (mode == FollowMode.Lerp) {
            transform.position = Vector3.Lerp(
                transform.position,
                target.position + offset,
                Time.deltaTime * speed
                );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                target.rotation,
                Time.deltaTime * rotationSpeed
                );
        }
    }

    public enum FollowMode {
        Absolute,
        Lerp
    }
}
