using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Zuy.Workspace.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : Base.BaseSingleton<UIManager>
    {
        #region Variables
        protected Dictionary<string, UIView> m_AllViews = new Dictionary<string, UIView>();

        // Samples Views
        private UIView _sampleView1;
        private UIView _sampleView2;

        // Samples Viewnames
        private const string _sampleView1Name = "sample_view_1";
        private const string _sampleView2Name = "sample_view_2";

        private const string _floatingTextName = "floating_text";
        private VisualElement _floatingText;

        private UIDocument _mainUIDocument;

        private bool _isUsingFloatingText;

        public UIDocument MainUIDocument => _mainUIDocument;
        #endregion

        #region Unity Lifecircle Methods
        protected override void Awake()
        {
            base.Awake();
            _mainUIDocument = GetComponent<UIDocument>();
        }

        protected virtual void OnEnable()
        {
            SetupViews();
            SubscribeToEvents();
        }

        protected virtual void OnDisable()
        {
            DisposalAllViews();
            UnsubscribeFromEvents();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                _sampleView1.Show();
                _sampleView2.Hide();
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                _sampleView2.Show();
                _sampleView1.Hide();
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                ShowFloatingText("This is an error!!!");
            }
        }
        #endregion

        #region Virtual Methods
        protected virtual void SetupViews()
        {
            VisualElement root = _mainUIDocument.rootVisualElement;

            _sampleView1 = new SampleView1(root.Q<VisualElement>(_sampleView1Name), TransitionType.Fade);
            _sampleView2 = new SampleView2(root.Q<VisualElement>(_sampleView2Name), TransitionType.Fade);

            m_AllViews.Add(_sampleView1Name, _sampleView1);
            m_AllViews.Add(_sampleView2Name, _sampleView2);

            _floatingText = root.Q<VisualElement>(_floatingTextName);
        }

        protected virtual void DisposalAllViews()
        {
            foreach (UIView view in m_AllViews.Values)
            {
                view.Dispose();
            }
            m_AllViews.Clear();
        }

        protected virtual void SubscribeToEvents()
        {

        }

        protected virtual void UnsubscribeFromEvents()
        {

        }

        protected virtual void ShowView(string viewName)
        {
            if (m_AllViews.TryGetValue(viewName, out var view))
            {
                view.Show();
            }
        }

        protected virtual void HideView(string viewName)
        {
            if (m_AllViews.TryGetValue(viewName, out var view))
            {
                view.Hide();
            }
        }

        protected virtual void ShowFloatingText(string message)
        {
            if (_isUsingFloatingText)
                return;

            _isUsingFloatingText = true;
            _floatingText.Q<Label>("message").text = message;
            _floatingText.AddToClassList("floating-text__container--visible-fade");
            _floatingText.AddToClassList("floating-text__container--visible-move");
            StartCoroutine(HideFloatingText());
        }

        IEnumerator HideFloatingText()
        {
            yield return new WaitForSeconds(1f);
            _floatingText.RemoveFromClassList("floating-text__container--trans-combined");
            _floatingText.AddToClassList("floating-text__container--trans-fade");
            _floatingText.RemoveFromClassList("floating-text__container--visible-fade");
            yield return new WaitForSeconds(0.5f);
            _floatingText.RemoveFromClassList("floating-text__container--visible-move");
            _floatingText.RemoveFromClassList("floating-text__container--trans-fade");
            yield return null;
            _floatingText.AddToClassList("floating-text__container--trans-combined");
            _isUsingFloatingText = false;
        }
        #endregion
    }
}