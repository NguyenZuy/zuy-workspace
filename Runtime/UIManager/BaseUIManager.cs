using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Zuy.Workspace.UI
{
    /// <summary>
    /// UIManager is a singleton class responsible for managing the UI elements in the Unity project.
    /// It handles the initialization, display, and disposal of various UI views, floating texts, dialog boxes, and circle loaders.
    /// The class also manages event subscriptions and provides methods to show/hide specific UI elements.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class BaseUIManager : Base.BaseSingleton<BaseUIManager>
    {
        #region Variables
        protected Dictionary<string, UIView> m_AllViews = new Dictionary<string, UIView>();

        private UIDocument _mainUIDocument;
        public UIDocument MainUIDocument => _mainUIDocument;

        protected bool m_IsUsingFloatingText;
        protected bool m_IsShowCircleLoader;

        // Samples views
        private UIView _sampleView1;
        private UIView _sampleView2;

        // Samples viewnames
        private const string _sampleView1Name = "sample_view_1";
        private const string _sampleView2Name = "sample_view_2";

        // Floating text
        private const string _floatingTextName = "floating_text";
        private VisualElement _floatingText;
        private Label _floatingTextMessageLbl;

        // Dialog box 
        private const string _dialogBoxName = "dialog_box";
        private VisualElement _dialogBox;
        private Label _dialogBoxTitleLbl;
        private Label _dialogBoxMessageLbl;
        private Button _dialogBoxOKBtn;
        private Button _dialogBoxYesBtn;
        private Button _dialogBoxNoBtn;
        private Action _dialogBoxPositiveCallback;
        private Action _dialogBoxNegativeCallback;

        // Circle loader
        private float _rotationSpeed = 100f;
        private const string _circleLoaderName = "circle_loader";
        private VisualElement _circleLoader;
        private VisualElement _circleLoaderImg;
        #endregion

        #region Unity Lifecircle Methods
        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes the main UIDocument component.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _mainUIDocument = GetComponent<UIDocument>();
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// Sets up views and subscribes to events.
        /// </summary>
        protected virtual void OnEnable()
        {
            SetupViews();
            SubscribeToEvents();
        }

        /// <summary>
        /// Called when the object becomes disabled or inactive.
        /// Disposes all views and unsubscribes from events.
        /// </summary>
        protected virtual void OnDisable()
        {
            DisposalAllViews();
            UnsubscribeFromEvents();
        }
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Sets up the views by querying the root visual element and initializing UI components.
        /// </summary>
        protected virtual void SetupViews()
        {
            VisualElement root = _mainUIDocument.rootVisualElement;

            _sampleView1 = new SampleView1(root.Q<VisualElement>(_sampleView1Name), TransitionType.Fade);
            _sampleView2 = new SampleView2(root.Q<VisualElement>(_sampleView2Name), TransitionType.Fade);

            m_AllViews.Add(_sampleView1Name, _sampleView1);
            m_AllViews.Add(_sampleView2Name, _sampleView2);

            _floatingText = root.Q<VisualElement>(_floatingTextName);
            _floatingTextMessageLbl = _floatingText.Q<Label>("message");

            _dialogBox = root.Q<VisualElement>(_dialogBoxName);
            _dialogBoxTitleLbl = _dialogBox.Q<Label>("title");
            _dialogBoxMessageLbl = _dialogBox.Q<Label>("message");
            _dialogBoxOKBtn = _dialogBox.Q<Button>("btn_ok");
            _dialogBoxYesBtn = _dialogBox.Q<Button>("btn_yes");
            _dialogBoxNoBtn = _dialogBox.Q<Button>("btn_no");
            RegisterDialogBoxButtonCallback(_dialogBoxOKBtn, _dialogBoxYesBtn, _dialogBoxNoBtn);

            _circleLoader = root.Q<VisualElement>(_circleLoaderName);
            _circleLoaderImg = _circleLoader.Q<VisualElement>("container").Q<VisualElement>("loader_image");
        }

        /// <summary>
        /// Disposes all views by calling their Dispose method and clears the dictionary.
        /// </summary>
        protected virtual void DisposalAllViews()
        {
            foreach (UIView view in m_AllViews.Values)
            {
                view.Dispose();
            }
            m_AllViews.Clear();
        }

        /// <summary>
        /// Subscribes to necessary events. (Currently empty, to be implemented as needed)
        /// </summary>
        protected virtual void SubscribeToEvents()
        {

        }

        /// <summary>
        /// Unsubscribes from events. (Currently empty, to be implemented as needed)
        /// </summary>
        protected virtual void UnsubscribeFromEvents()
        {

        }

        /// <summary>
        /// Shows the specified view by its name.
        /// </summary>
        /// <param name="viewName">The name of the view to show.</param>
        protected virtual void ShowView(string viewName)
        {
            if (m_AllViews.TryGetValue(viewName, out var view))
            {
                view.Show();
            }
        }

        /// <summary>
        /// Hides the specified view by its name.
        /// </summary>
        /// <param name="viewName">The name of the view to hide.</param>
        protected virtual void HideView(string viewName)
        {
            if (m_AllViews.TryGetValue(viewName, out var view))
            {
                view.Hide();
            }
        }

        /// <summary>
        /// Shows a floating text with the specified message.
        /// </summary>
        /// <param name="message">The message to display in the floating text.</param>
        protected virtual void ShowFloatingText(string message)
        {
            if (m_IsUsingFloatingText)
                return;

            m_IsUsingFloatingText = true;
            _floatingTextMessageLbl.text = message;
            _floatingText.style.display = DisplayStyle.Flex;
            _floatingText.AddToClassList("floating-text__container--visible-fade");
            _floatingText.AddToClassList("floating-text__container--visible-move");
            StartCoroutine(HideFloatingText());
        }

        /// <summary>
        /// Shows a dialog box with the specified type, title, message, and optional callbacks.
        /// </summary>
        /// <param name="dialogBoxType">The type of the dialog box (OK or YesNo).</param>
        /// <param name="title">The title of the dialog box.</param>
        /// <param name="message">The message of the dialog box.</param>
        /// <param name="positiveCallback">The callback for the positive button (OK or Yes).</param>
        /// <param name="negativeCallback">The callback for the negative button (No).</param>
        protected virtual void ShowDialogBox(DialogBoxType dialogBoxType, string title, string message, EventCallback<ClickEvent> positiveCallback = null, EventCallback<ClickEvent> negativeCallback = null)
        {
            EventCallback<ClickEvent> noOpCallback = evt => { };

            positiveCallback ??= noOpCallback;
            negativeCallback ??= noOpCallback;

            switch (dialogBoxType)
            {
                case DialogBoxType.OK:
                    _dialogBoxOKBtn.style.display = DisplayStyle.Flex;
                    _dialogBoxYesBtn.style.display = DisplayStyle.None;
                    _dialogBoxNoBtn.style.display = DisplayStyle.None;
                    _dialogBoxOKBtn.RegisterCallbackOnce(positiveCallback);
                    break;
                case DialogBoxType.YesNo:
                    _dialogBoxOKBtn.style.display = DisplayStyle.None;
                    _dialogBoxYesBtn.style.display = DisplayStyle.Flex;
                    _dialogBoxNoBtn.style.display = DisplayStyle.Flex;
                    _dialogBoxYesBtn.RegisterCallbackOnce(positiveCallback);
                    _dialogBoxNoBtn.RegisterCallbackOnce(negativeCallback);
                    break;
                default:
                    break;
            }

            _dialogBox.style.display = DisplayStyle.Flex;
            _dialogBox.AddToClassList("dialog-box__fade");
            _dialogBox.AddToClassList("dialog-box__fade--invisible");
            _dialogBox.AddToClassList("dialog-box__fade--visible");
        }

        /// <summary>
        /// Shows the circle loader.
        /// </summary>
        protected virtual void ShowCircleLoader()
        {
            _circleLoader.style.display = DisplayStyle.Flex;
            m_IsShowCircleLoader = true;
        }

        /// <summary>
        /// Hides the circle loader.
        /// </summary>
        protected virtual void HideCircleLoader()
        {
            _circleLoader.style.display = DisplayStyle.None;
            m_IsShowCircleLoader = false;
        }
        #endregion

        #region Required Methods
        /// <summary>
        /// Registers callbacks for dialog box buttons to handle pointer events.
        /// </summary>
        /// <param name="buttons">The buttons to register callbacks for.</param>
        void RegisterDialogBoxButtonCallback(params Button[] buttons)
        {
            void OnPointerEvent(Button button, bool isPressed)
            {
                if (isPressed)
                {
                    button.AddToClassList("dialog-box__button--pressed");
                }
                else
                {
                    button.RemoveFromClassList("dialog-box__button--pressed");
                }

                _dialogBox.RemoveFromClassList("dialog-box__fade");
                _dialogBox.RemoveFromClassList("dialog-box__fade--visible");
                _dialogBox.AddToClassList("view__fade__fade--invisible");
            }

            foreach (var button in buttons)
            {
                button.RegisterCallback<PointerDownEvent>(evt => OnPointerEvent(button, true), TrickleDown.TrickleDown);
                button.RegisterCallback<PointerUpEvent>(evt => OnPointerEvent(button, false), TrickleDown.TrickleDown);
            }
        }

        /// <summary>
        /// Coroutine to hide the floating text after a delay.
        /// </summary>
        /// <returns>IEnumerator for the coroutine.</returns>
        IEnumerator HideFloatingText()
        {
            yield return new WaitForSeconds(1f);
            _floatingText.RemoveFromClassList("floating-text__container--trans-combined");
            _floatingText.AddToClassList("floating-text__container--trans-fade");
            _floatingText.RemoveFromClassList("floating-text__container--visible-fade");
            yield return new WaitForSeconds(0.5f);
            _floatingText.RemoveFromClassList("floating-text__container--visible-move");
            _floatingText.RemoveFromClassList("floating-text__container--trans-fade");
            _floatingText.style.display = DisplayStyle.None;
            yield return null;
            _floatingText.AddToClassList("floating-text__container--trans-combined");
            m_IsUsingFloatingText = false;
        }
        #endregion
    }
}
