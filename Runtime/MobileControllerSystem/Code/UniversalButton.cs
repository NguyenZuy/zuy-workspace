using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

namespace Suriyun.MCS {
    public class UniversalButton : MonoBehaviour,
    IPointerDownHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerUpHandler {
        #region :: Config ::
        public bool debugLog = true;
        public CanvasScaler scaler;
        public bool isAimable = false;
        public RectTransform btn;
        public RectTransform aimer;
        public RectTransform pointer;
        public RectTransform skillCanceller;
        protected SkillCanceller cachedSkillCanceller;
        public bool hasText;
        public TextMeshProUGUI text;
        public Image img;
        public bool resetOutputValueOnRelease;
        #endregion
        #region :: Parameters ::
        public ButtonState state;
        public bool isActive;
        public float btnRadius;
        public float aimerRadius;
        public bool isManualAimOverride = false;
        public bool isFingerDown = false;
        public bool isPointerUpOutOfBound = false;
        public Vector3 initialFingerPosition;
        public int fingerId = -99;
        public Vector3 fingerPosition;
        public Vector3 directionUnmodded;
        public Vector3 direction;
        public Vector3 directionXZ;
        public Vector3 rawDir;
        public float cancellerRadius;
        public AnimationCurve deadzoneCurve;
        [SerializeField]
        public float horizontal {
            get { return direction.x; }
        }
        [SerializeField]
        public float vertical {
            get { return direction.y; }
        }
        #endregion
        #region Cosmetics
        protected Vector3 refScale;
        protected Vector3 onPressedScale;
        public Color colorActive;
        public Color colorInactive;
        public Color colorPressed;
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

        public enum ButtonState {
            Active,
            Inactive,
            Pressed
        }

        protected virtual void Awake() {
            if (isAimable) {
                aimer.gameObject.SetActive(false);
                pointer.gameObject.SetActive(false);
            }
            btn = GetComponent<RectTransform>();

            this.UpdateBound();

            refScale = GetComponent<RectTransform>().localScale;
            onPressedScale = refScale * 0.75f;

            img = GetComponent<Image>();
            colorActive = img.color;

            this.UpdateButtonState();
        }

        public virtual void OnPointerDown(PointerEventData eventData) {
            if (state == ButtonState.Active) {
                if (debugLog) {
                    Debug.Log("[MCS] " + "[" + gameObject.name + "] " + "OnPointerDown - FingerID : " + eventData.pointerId);
                }
                isFingerDown = true;
                fingerId = eventData.pointerId;
                initialFingerPosition = eventData.position;
                fingerPosition = initialFingerPosition;
                isPointerUpOutOfBound = false;

                if (isAimable) {
                    aimer.gameObject.SetActive(true);
                    pointer.gameObject.SetActive(true);
                    pointer.position = aimer.position;
                }

                if (skillCanceller != null) {
                    this.UpdateSkillCancellerState();
                }

                state = ButtonState.Pressed;
                this.UpdateColor();

                if (onPointerDown != null) {
                    onPointerDown.Invoke(btnIndex);
                }
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData) {
            if (isAimable
                && state == ButtonState.Pressed) {
                if (debugLog) {
                    Debug.Log("[MCS] " + "[" + gameObject.name + "] " + "OnBeginDrag - FingerID : " + eventData.pointerId);
                }
                isManualAimOverride = true;

                this.UpdateAiming(eventData);

                if (onBeginDrag != null) {
                    onBeginDrag.Invoke(btnIndex);
                }
            }
        }

        protected bool canActivateSkill = false;
        public virtual void OnDrag(PointerEventData eventData) {
            if (isAimable
                && eventData.pointerId == fingerId
                && state == ButtonState.Pressed) {
                this.UpdateAiming(eventData);

                if (debugLog) {
                    Debug.Log("[MCS] " + "[" + gameObject.name + "] " + "OnDrag - FingerID : " + eventData.pointerId);
                }

                if (skillCanceller != null) {
                    this.UpdateSkillCancellerState();
                }

                if (onDrag != null) {
                    onDrag.Invoke(btnIndex);
                }
            }
        }


        public virtual void OnPointerUp(PointerEventData eventData) {
            if (state == ButtonState.Pressed) {

                if (debugLog) {
                    Debug.Log("[MCS] " + "[" + gameObject.name + "] " + "OnPointerUp - FingerID : " + eventData.pointerId);
                }

                isFingerDown = false;
                fingerId = -99;

                if (Vector3.Distance(initialFingerPosition, fingerPosition) > btnRadius) {
                    isPointerUpOutOfBound = true;
                    if (debugLog) {
                        Debug.Log("[MCS] " + "isPointerUpOutOfBound : " + isPointerUpOutOfBound.ToString());
                    }
                }

                if (isAimable) {
                    aimer.gameObject.SetActive(false);
                    pointer.gameObject.SetActive(false);

                    if (skillCanceller != null) {
                        this.UpdateSkillCancellerState();
                    }

                    if (resetOutputValueOnRelease) {
                        ResetOutputValue();
                    }
                }

                

                state = ButtonState.Active;
                this.UpdateButtonState();
                if (canActivateSkill && onActivateSkill != null) {

                    onActivateSkill.Invoke(btnIndex);
                } else if (onCancelSkill != null) {
                    onCancelSkill.Invoke(btnIndex);
                }

                if (onPointerUp != null) {
                    onPointerUp.Invoke(btnIndex);
                }

                this.UpdateColor();
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData) {
            if (isAimable) {
                if (debugLog) {
                    Debug.Log("[MCS] " + "[" + gameObject.name + "] " + "OnEndDrag - FingerID : " + eventData.pointerId);
                }

                isManualAimOverride = false;

                if (onEndDrag != null) {
                    onEndDrag.Invoke(btnIndex);
                }
            }
        }

        protected virtual void UpdateAiming(PointerEventData eventData) {
            fingerPosition.x = eventData.position.x;
            fingerPosition.y = eventData.position.y;
            rawDir = fingerPosition - aimer.position;
            rawDir = Vector3.ClampMagnitude(rawDir, aimerRadius);
            pointer.position = aimer.position + rawDir;

            this.UpdateDirection();
            if (debugLog) {
                Debug.Log("[MCS] " + "Aim Value : " + direction + " \t\t Magnitude : " + direction.magnitude);
            }
        }

        protected virtual void ResetOutputValue() {
            Debug.Log("Output value reset!");
            rawDir = Vector3.zero;
            this.UpdateDirection();
        }

        protected virtual void UpdateDirection() {
            directionUnmodded = rawDir / aimerRadius;
            direction = directionUnmodded * deadzoneCurve.Evaluate(directionUnmodded.magnitude);
            directionXZ.x = direction.x;
            directionXZ.y = 0f;
            directionXZ.z = direction.y;
        }

        public virtual void SetActiveState(bool active) {
            isActive = active;
            this.UpdateButtonState();
        }

        protected virtual void UpdateButtonState() {
            if (isActive) {
                state = ButtonState.Active;
            } else {
                state = ButtonState.Inactive;

            }
            this.UpdateColor();
        }

        protected virtual void UpdateColor() {
            switch (state) {
                case ButtonState.Active:
                    img.color = colorActive;
                    break;
                case ButtonState.Inactive:
                    img.color = colorInactive;
                    break;
                case ButtonState.Pressed:
                    img.color = colorPressed;
                    break;
            }
        }

        protected void UpdateSkillCancellerState() {
            if (cachedSkillCanceller == null) {
                cachedSkillCanceller = skillCanceller.GetComponent<SkillCanceller>();
            }

            if (IsFingerOverSkillCancellerButton()) {
                canActivateSkill = false;
                cachedSkillCanceller.state = ButtonState.Pressed;
            } else {
                canActivateSkill = true;
                cachedSkillCanceller.state = ButtonState.Active;
            }
        }
        protected bool IsFingerOverSkillCancellerButton() {
            return Vector3.Distance(fingerPosition, skillCanceller.position) < cancellerRadius;
        }

        public virtual void SetText(string t) {
            if (text != null) {
                text.text = t;
            }
        }

        public virtual void UpdateBound() {
            btnRadius = btn.rect.width / 2f * scaler.scaleFactor;
            if (debugLog) {
                Debug.Log("[MCS] " + this.gameObject.name + " >> " + btnRadius + " = " + btn.sizeDelta.x / 2f);
            }
            if (isAimable) {
                aimerRadius = aimer.rect.width / 2f * scaler.scaleFactor;
            }
            if (skillCanceller != null) {
                cancellerRadius = skillCanceller.rect.width / 2f * scaler.scaleFactor;
            }
        }
    }

    [System.Serializable]
    public class UnityEventInt : UnityEvent<int> {

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UniversalButton)), CanEditMultipleObjects]
    public class UniversalBtnInspector : Editor {
        protected SerializedProperty debugLog;
        protected SerializedProperty scaler;
        protected SerializedProperty isAimable;
        protected SerializedProperty aimer;
        protected SerializedProperty pointer;
        protected SerializedProperty skillCanceller;
        protected SerializedProperty isActive;
        protected SerializedProperty isFingerDown;

        protected SerializedProperty fingerId;
        protected SerializedProperty isManualAimOverride;
        protected SerializedProperty direction;
        protected SerializedProperty state;
        protected SerializedProperty text;

        protected SerializedProperty deadzoneCurve;
        protected SerializedProperty resetOutputValueOnRelease;

        protected bool showRawButtonEvents = false;
        protected SerializedProperty onPointerDown;
        protected SerializedProperty onBeginDrag;
        protected SerializedProperty onDrag;
        protected SerializedProperty onPointerUp;
        protected SerializedProperty onEndDrag;

        protected bool showGameLogicEvents = true;
        protected SerializedProperty btnIndex;
        protected SerializedProperty onActivateSkill;
        protected SerializedProperty onCancelSkill;

        protected SerializedProperty colorActive;
        protected SerializedProperty colorInactive;
        protected SerializedProperty colorPressed;

        protected virtual void OnEnable() {
            debugLog = serializedObject.FindProperty("debugLog");
            scaler = serializedObject.FindProperty("scaler");
            isAimable = serializedObject.FindProperty("isAimable");
            aimer = serializedObject.FindProperty("aimer");
            pointer = serializedObject.FindProperty("pointer");
            skillCanceller = serializedObject.FindProperty("skillCanceller");
            isActive = serializedObject.FindProperty("isActive");
            isFingerDown = serializedObject.FindProperty("isFingerDown");
            fingerId = serializedObject.FindProperty("fingerId");
            isManualAimOverride = serializedObject.FindProperty("isManualAimOverride");
            direction = serializedObject.FindProperty("direction");
            state = serializedObject.FindProperty("state");
            text = serializedObject.FindProperty("text");

            deadzoneCurve = serializedObject.FindProperty("deadzoneCurve");
            resetOutputValueOnRelease = serializedObject.FindProperty("resetOutputValueOnRelease");

            btnIndex = serializedObject.FindProperty("btnIndex");
            onActivateSkill = serializedObject.FindProperty("onActivateSkill");
            onCancelSkill = serializedObject.FindProperty("onCancelSkill");

            onPointerDown = serializedObject.FindProperty("onPointerDown");
            onBeginDrag = serializedObject.FindProperty("onBeginDrag");
            onDrag = serializedObject.FindProperty("onDrag");
            onPointerUp = serializedObject.FindProperty("onPointerUp");
            onEndDrag = serializedObject.FindProperty("onEndDrag");

            colorActive = serializedObject.FindProperty("colorActive");
            colorInactive = serializedObject.FindProperty("colorInactive");
            colorPressed = serializedObject.FindProperty("colorPressed");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.TextArea("-----[ Config ]---------------", GUIStyle.none);
            EditorGUILayout.PropertyField(debugLog);
            EditorGUILayout.PropertyField(scaler);
            EditorGUILayout.PropertyField(isAimable);
            if (isAimable.boolValue) {
                EditorGUILayout.PropertyField(aimer);
                EditorGUILayout.PropertyField(pointer);
                EditorGUILayout.PropertyField(skillCanceller);
                EditorGUILayout.PropertyField(deadzoneCurve);
                EditorGUILayout.PropertyField(resetOutputValueOnRelease);
            }
            EditorGUILayout.PropertyField(text);
            EditorGUILayout.PropertyField(btnIndex);
            EditorGUILayout.TextArea("-----[ Parameters ]---------------", GUIStyle.none);
            EditorGUILayout.PropertyField(isActive);
            EditorGUILayout.PropertyField(isFingerDown);
            EditorGUILayout.PropertyField(fingerId);
            if (isAimable.boolValue) {
                EditorGUILayout.PropertyField(isManualAimOverride);
                EditorGUILayout.PropertyField(direction);
            }
            EditorGUILayout.PropertyField(state);
            EditorGUILayout.PropertyField(colorInactive);
            EditorGUILayout.PropertyField(colorPressed);
            EditorGUILayout.TextArea("-----[ Events ]---------------", GUIStyle.none);
            showGameLogicEvents = EditorGUILayout.Toggle("showGameLogicEvents", showGameLogicEvents);
            showRawButtonEvents = EditorGUILayout.Toggle("showRawButtonEvents", showRawButtonEvents);
            if (showGameLogicEvents) {
                EditorGUILayout.PropertyField(onActivateSkill);
                EditorGUILayout.PropertyField(onCancelSkill);
            }
            if (showRawButtonEvents) {
                EditorGUILayout.PropertyField(onPointerDown);
                EditorGUILayout.PropertyField(onBeginDrag);
                EditorGUILayout.PropertyField(onDrag);
                EditorGUILayout.PropertyField(onPointerUp);
                EditorGUILayout.PropertyField(onEndDrag);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}