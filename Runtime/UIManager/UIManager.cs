using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Zuy.Workspace.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {
        #region Variables
        // Samples Views
        private UIView _sampleView1;
        private UIView _sampleView2;

        // Samples Viewnames
        private const string _sampleView1Name = "sample_view_1";
        private const string _sampleView2Name = "sample_view_2";

        private UIDocument _mainUIDocument;
        private Dictionary<string, UIView> _dictAllViews;

        public UIDocument MainUIDocument => _mainUIDocument;
        #endregion

        #region Unity Lifecircle Methods
        protected virtual void Awake()
        {
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
        }
        #endregion

        #region Virtual Methods
        protected virtual void SetupViews()
        {
            VisualElement root = _mainUIDocument.rootVisualElement;

            _sampleView1 = new SampleView1(root.Q<VisualElement>(_sampleView1Name), TransitionType.Fade);
            _sampleView2 = new SampleView2(root.Q<VisualElement>(_sampleView2Name), TransitionType.Fade);

            _dictAllViews = new Dictionary<string, UIView>
            {
                { _sampleView1Name, _sampleView1 },
                { _sampleView2Name, _sampleView2 }
            };
        }

        protected virtual void DisposalAllViews()
        {
            foreach (UIView view in _dictAllViews.Values)
            {
                view.Dispose();
            }
        }

        protected virtual void SubscribeToEvents()
        {

        }

        protected virtual void UnsubscribeFromEvents()
        {

        }
        #endregion
    }
}