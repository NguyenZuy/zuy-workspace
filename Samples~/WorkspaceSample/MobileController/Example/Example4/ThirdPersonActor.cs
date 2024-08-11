using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suriyun.MCS;

public class ThirdPersonActor : MonoBehaviour {
    public UniversalButton inputMove;
    public bool lerpStopping = false;
    public float moveSpeed;
    public float rotationSpeed;
    public AnimationCurve turningPowerCurve;

    protected virtual void Start() {
        Application.targetFrameRate = 60;
    }

    protected Vector3 cachedInput;
    protected float cachedRotationValue;
    protected virtual void Update() {
        if (inputMove.isFingerDown) {
            cachedInput = inputMove.directionXZ;
        } else {
            if (lerpStopping) {
                cachedInput.x = Mathf.Lerp(cachedInput.x, 0f, rotationSpeed * Time.deltaTime);
                cachedInput.z = Mathf.Lerp(cachedInput.z, 0f, moveSpeed * Time.deltaTime);
            } else {
                cachedInput = Vector3.zero;
            }
        }

        if (cachedInput.x < 0f) {
            cachedRotationValue = -1f * turningPowerCurve.Evaluate(Mathf.Abs(cachedInput.x));
        } else {
            cachedRotationValue = turningPowerCurve.Evaluate(Mathf.Abs(cachedInput.x));
        }

        transform.Rotate(transform.up, cachedRotationValue * rotationSpeed * Time.deltaTime);
        transform.Translate(transform.forward * cachedInput.z * moveSpeed * Time.deltaTime, Space.World);

    }
}
