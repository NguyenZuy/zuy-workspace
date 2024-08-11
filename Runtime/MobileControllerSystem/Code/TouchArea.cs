using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;
using static Suriyun.MCS.UniversalButton;

namespace Suriyun.MCS {
    public class TouchArea : MonoBehaviour,
    IPointerDownHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerUpHandler {


        #region :: Config ::
        public bool debugLog = true;
        public CanvasScaler scaler;
        #endregion

        #region :: Parameters ::
        public ButtonState state;
        public bool isActive;
        public float btnRadius;
        public float aimerRadius;
        public bool isFingerDown = false;
        public bool isPointerUpOutOfBound = false;

        public Vector3 initialFingerPosition;
        protected Vector3 lastKnownFingerPosition;
        public Vector3 fingerPosition;
        public Vector3 deltaFingerPositionRaw;
        public Vector3 deltaFingerPositionInches;

        public Vector3 deltaFingerPositionRawYX;
        public Vector3 deltaFingerPositionInchesYX;



        public int fingerId = -99;

        public float totalDragDistance;

        public AnimationCurve deadzoneCurve;

        #endregion

        #region Events
        public int btnIndex;
        public UnityEventInt onPointerDown;
        public UnityEventInt onBeginDrag;
        public UnityEventInt onDrag;
        public UnityEventInt onPointerUp;
        public UnityEventInt onEndDrag;
        public UnityEventInt onActivateSkill;
        public UnityEventInt onCancelSkill;
        #endregion

        protected virtual void Awake() {

        }

        public virtual void OnPointerDown(PointerEventData eventData) {
            if (state == ButtonState.Active) {
                if (debugLog) {
                    Debug.Log("[MCS] " + "[" + gameObject.name + "] " + "OnPointerDown - FingerID : " + eventData.pointerId);
                }

                isFingerDown = true;
                fingerId = eventData.pointerId;

                isPointerUpOutOfBound = false;

                // TODO // Uupdate Drag
                initialFingerPosition = eventData.position;
                fingerPosition = initialFingerPosition;
                lastKnownFingerPosition = fingerPosition;
                deltaFingerPositionRaw = Vector3.zero;


                state = ButtonState.Pressed;

                if (onPointerDown != null) {
                    onPointerDown.Invoke(btnIndex);
                }
            }
        }


        public virtual void OnBeginDrag(PointerEventData eventData) {
            if (eventData.pointerId == fingerId && state == ButtonState.Pressed) {
                if (debugLog) {
                    Debug.Log("[MCS] " + "[" + gameObject.name + "] " + "OnBeginDrag - FingerID : " + eventData.pointerId);
                }
                // TODO // Uupdate Drag
                totalDragDistance = 0f;
                this.UpdateDrag(eventData);

                if (onBeginDrag != null) {
                    onBeginDrag.Invoke(btnIndex);
                }
            }
        }

        public virtual void OnDrag(PointerEventData eventData) {
            if (eventData.pointerId == fingerId && state == ButtonState.Pressed) {
                if (debugLog) {
                    Debug.Log("[MCS] " + "[" + gameObject.name + "] " + "OnDrag - FingerID : " + eventData.pointerId);
                }

                // TODO // Uupdate Drag
                this.UpdateDrag(eventData);
                totalDragDistance += deltaFingerPositionInches.magnitude;

                if (debugLog) {
                    Debug.Log("[MCS] " + "OnDrag : " + this.deltaFingerPositionInches.ToString("F7"));
                }

                if (onDrag != null) {
                    onDrag.Invoke(btnIndex);
                }
                LogDrag();
            }
        }

        public virtual void OnPointerUp(PointerEventData eventData) {
            if (eventData.pointerId == fingerId && state == ButtonState.Pressed) {
                if (debugLog) {
                    Debug.Log("[MCS] " + "[" + gameObject.name + "] " + "OnPointerUp - FingerID : " + eventData.pointerId);
                }

                isFingerDown = false;
                fingerId = -99;

                // TODO // Uupdate Drag
                this.UpdateDrag(eventData);


                state = ButtonState.Active;

                if (onPointerUp != null) {
                    onPointerUp.Invoke(btnIndex);
                }
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData) {
            if (eventData.pointerId == fingerId) {
                if (debugLog) {
                    Debug.Log("[MCS] " + "[" + gameObject.name + "] " + "OnEndDrag - FingerID : " + eventData.pointerId);
                }

                // TODO // Uupdate Drag
                this.UpdateDrag(eventData);

                if (onEndDrag != null) {
                    onEndDrag.Invoke(btnIndex);
                }
            }



        }

        protected virtual void UpdateDrag(PointerEventData eventData) {
            fingerPosition = eventData.position;

            deltaFingerPositionRaw = fingerPosition - lastKnownFingerPosition;
            deltaFingerPositionInches = deltaFingerPositionRaw / Screen.dpi;

            // Adjusted with deadzone curve
            deltaFingerPositionInches = deltaFingerPositionInches * deadzoneCurve.Evaluate(deltaFingerPositionInches.magnitude);

            lastKnownFingerPosition = fingerPosition;

            deltaFingerPositionRawYX.x = deltaFingerPositionRaw.y;
            deltaFingerPositionRawYX.y = deltaFingerPositionRaw.x;

            deltaFingerPositionInchesYX.x = deltaFingerPositionInches.y;
            deltaFingerPositionInchesYX.y = deltaFingerPositionInches.x;
        }

        protected string tmpS;
        protected virtual void LogDrag() {
            tmpS = "";
            tmpS += "initialFingerPosition : " + initialFingerPosition + " | ";
            tmpS += "fingerPosition : " + fingerPosition + "\n";
            tmpS += "deltaFingerPosition : " + deltaFingerPositionRaw.x + " " + deltaFingerPositionRaw.y + " ";
            tmpS += "totalDragDistance : " + totalDragDistance;


            Debug.Log(tmpS);
        }

    } // Class : TouchArea

} // Namespace