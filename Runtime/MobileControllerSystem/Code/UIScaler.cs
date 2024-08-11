using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Suriyun.MCS {
    [DefaultExecutionOrder(-1200)]
    public class UIScaler : MonoBehaviour {
        public CanvasScaler scaler;

        public float refDPI;
        public float refScaleFactor;
        public float refWidth;
        public float refHeight;

        public float refInches;
        public float refDiagonalInches;
        public float preferredScaleFactor;

        public UIScaleMode scaleMode;

        public AnimationCurve scaleByScreenSizeInches;

        public AnimationCurve scaleMultiplierByDpi;
        public AnimationCurve scaleMultiplierByAspectRatio;

        protected virtual void Awake() {
            // iPhone 11 Pro Max
            // DPI 458
            // Width 2688
            // Inches 5.868996
            // ScaleFactor 2.061925

            // Unity Editor
            //refDPI = 298;
            //refScaleFactor = 1.1f;
            //refWidth = 1434;
            //refInches = refWidth / (float)refDPI;

            refDPI = 458;
            refWidth = 2688;
            refHeight = 1242;
            refInches = refWidth / (float)refDPI;
            refDiagonalInches = 6.465209f;
            refScaleFactor = 2.061925f;

            this.UpdateScale();
        }

        public void UpdateScale() {
            switch (scaleMode) {
                case UIScaleMode.Variable:
                    preferredScaleFactor = refScaleFactor * Screen.width / refWidth
                        * scaleMultiplierByDpi.Evaluate(Screen.dpi)
                        * scaleMultiplierByAspectRatio.Evaluate(Screen.width / (float)Screen.height);
                    break;
            }
            scaler.scaleFactor = preferredScaleFactor;
            this.LogScaleInfo();
        }

        protected void LogScaleInfo() {
            Debug.Log("[MCS]\n" +
                      "DPI " + Screen.dpi + "\n" +
                      "Width " + Screen.width + "\n" +
                      "Height " + Screen.height + "\n" +
                      "Inches " + GetDiagonalInches(Screen.width, Screen.height, Screen.dpi) + "\n" +
                      "ScaleFactor " + scaler.scaleFactor
                      );
        }

        public float GetDiagonalPixel(int w, int h) {
            return Mathf.Sqrt(Mathf.Pow(w, 2) + Mathf.Pow(h, 2));
        }

        public float GetDiagonalInches(int w, int h, float dpi) {
            return GetDiagonalPixel(w, h) / dpi;
        }

        public enum UIScaleMode {
            Variable
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIScaler))]
    public class UIScalerInspector : Editor {
        protected UIScaler script;

        protected SerializedProperty scaler;
        protected SerializedProperty scaleMode;

        protected SerializedProperty scaleMultiplierByDpi;
        protected SerializedProperty scaleMultiplierByAspectRatio;

        protected float width;
        protected float height;
        protected float dpi;
        protected float aspectRatio;

        protected float tmpFloat;

        protected virtual void OnEnable() {
            script = target as UIScaler;
            scaler = serializedObject.FindProperty("scaler");
            scaleMode = serializedObject.FindProperty("scaleMode");
            scaleMultiplierByDpi = serializedObject.FindProperty("scaleMultiplierByDpi");
            scaleMultiplierByAspectRatio = serializedObject.FindProperty("scaleMultiplierByAspectRatio");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.TextArea("-----[ Config ]---------------", GUIStyle.none);
            EditorGUILayout.PropertyField(scaler);
            EditorGUILayout.PropertyField(scaleMode);
            EditorGUILayout.PropertyField(scaleMultiplierByDpi);
            EditorGUILayout.PropertyField(scaleMultiplierByAspectRatio);

            if (GUILayout.Button("Update scale")) {
                script.refDPI = 458;
                script.refWidth = 2688;
                script.refHeight = 1242;
                script.refInches = script.refWidth / (float)script.refDPI;
                script.refDiagonalInches = 6.465209f;
                script.refScaleFactor = 2.061925f;
                script.scaler.runInEditMode = true;
                script.UpdateScale();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}