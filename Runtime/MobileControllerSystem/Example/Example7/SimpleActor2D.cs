using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suriyun.MCS;
using System;

public class SimpleActor2D : MonoBehaviour {

    // This script prioritize simplicity for educational purpose over performance.

    public UniversalButton inputMove;
    public UniversalButton inputAttack;

    public float moveSpeed;

    protected Vector3 cachedInput;
    protected SpriteRenderer sprite;

    protected virtual void Start() {
        sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update() {
        if (inputMove.isFingerDown) {
            cachedInput = inputMove.direction;

            if (cachedInput.x > 0) {
                sprite.flipX = true;
            }

            if (cachedInput.x < 0) {
                sprite.flipX = false;
            }
        } else {
            cachedInput = Vector3.zero;
        }

        transform.Translate(cachedInput * moveSpeed * Time.deltaTime);
    }

    #region === Event Handler ===
    public virtual void OnEnable() {
        inputAttack.onPointerDown.AddListener(Attack);
    }

    public virtual void OnDisable() {
        inputAttack.onPointerDown.RemoveListener(Attack);
    }

    public virtual void Attack(int btnId) {
        Debug.Log("[logic] Attack : " + btnId);
    }
    #endregion
}