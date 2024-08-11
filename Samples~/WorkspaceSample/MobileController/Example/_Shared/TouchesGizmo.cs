using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchesGizmo : MonoBehaviour {

    public RectTransform indicatorPrefabs;
    public List<RectTransform> indies;
    protected RectTransform rect;

    protected void Start() {
        rect = GetComponent<RectTransform>();
    }

    protected void Update() {

        while (indies.Count < Input.touchCount) {
            indies.Add(Instantiate(indicatorPrefabs) as RectTransform);
            indies[indies.Count - 1].SetParent(rect, false);
        }

        while (indies.Count > Input.touchCount) {
            Destroy(indies[0].gameObject);
            indies.RemoveAt(0);
        }

        for (int i = 0; i < Input.touchCount; i++) {
            indies[i].position = Input.touches[i].position;
        }

    }
}
