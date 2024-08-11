using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suriyun.MCS;
using System;

/// <summary>
/// This is an example of moving your character with built-in Unity physics.
/// Untiy Physics reference : https://docs.unity3d.com/ScriptReference/Rigidbody.html
/// </summary>

public class SimpleActor : MonoBehaviour {

    public UniversalButton inputMove;
    public UniversalButton inputAtk;
    public UniversalButton inputJump;

    public bool lerpStopping = false;
    public Transform dirMarker;
    public float moveSpeed;

    public float jumpPower;
    public Vector3 addGravity;

    #region === Core ===
    protected virtual void Start() {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
        }
        StartCoroutine(SlowUpdate());
    }
    protected Vector3 cachedInput;
    protected virtual void Update() {
        if (inputMove.isFingerDown) {
            cachedInput = inputMove.directionXZ;
            transform.forward = cachedInput;
        } else {
            if (lerpStopping) {
                cachedInput = Vector3.Lerp(cachedInput, Vector3.zero, moveSpeed * Time.deltaTime);
            } else {
                cachedInput = Vector3.zero;
            }
        }
        dirMarker.position = transform.position + cachedInput;

        UpdateCharge(Time.deltaTime);
    }

    protected virtual void FixedUpdate() {
        if (IsGrounded()) {
            rb.AddForce(cachedInput * moveSpeed, ForceMode.Acceleration);
        } else {
            rb.AddForce(cachedInput * moveSpeed / 2f, ForceMode.Acceleration);
        }
        rb.AddForce(addGravity, ForceMode.Acceleration);
    }


    protected IEnumerator SlowUpdate() {
        bool done = false;
        while (!done) {
            if (isCharging) {
                Debug.Log("[logic] Charge Time : " + chargeTime);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    #endregion
    #region === Event Handler ===
    public virtual void OnEnable() {
        inputAtk.onPointerDown.AddListener(ChargeAtk);
        inputAtk.onPointerUp.AddListener(ReleaseCharge);
        inputJump.onPointerDown.AddListener(Jump);
    }

    public virtual void OnDisable() {
        inputAtk.onPointerDown.RemoveListener(ChargeAtk);
        inputAtk.onPointerUp.RemoveListener(ReleaseCharge);
        inputJump.onPointerDown.RemoveListener(Jump);
    }

    protected ChargeBall genki;
    public bool isCharging;
    public float chargeTime;
    public Vector3 genkiPosition => transform.position + Vector3.up * 2f;
    public virtual void ChargeAtk(int arg0) {
        chargeTime = 0;
        isCharging = true;
        Debug.Log("[logic] Charge Atk : " + Time.realtimeSinceStartup);

        genki = new ChargeBall();
        genki.CreateAt(genkiPosition);
    }

    public virtual void UpdateCharge(float deltaTime) {
        if (isCharging) {
            chargeTime += deltaTime;
            genki.Update(genkiPosition + genki.size * Vector3.up, chargeTime * 2f);
        }
    }

    public virtual void ReleaseCharge(int arg0) {
        isCharging = false;
        genki.Shoot(transform.forward);
        Debug.Log("[logic] Release charge : " + chargeTime);
    }

    protected Rigidbody rb;

    public virtual void Jump(int btn_id) {
        if (IsGrounded()) {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            Debug.Log("[logic] Jump");
        }
    }

    protected bool IsGrounded() {
        if (Physics.Raycast(transform.position, Vector3.down, 1.2f)) {
            return true;
        } else {
            return false;
        }
    }
    #endregion
}

public class ChargeBall {

    public GameObject gameObject;
    public Rigidbody rb;
    public float size;

    public ChargeBall CreateAt(Vector3 position) {
        gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rb = gameObject.AddComponent<Rigidbody>();
        return this;
    }

    public void Update(Vector3 position, float size) {
        gameObject.transform.position = position;
        this.size = size;
        gameObject.transform.localScale = Vector3.one * size;
    }

    public void Shoot(Vector3 dir, float power = 6f) {
        rb.AddForce(dir * power * size, ForceMode.Impulse);
        GameObject.Destroy(gameObject, 45f);
    }
}
