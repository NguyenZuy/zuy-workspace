using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Zuy.Workspace.UI
{
    public abstract class UIView : IDisposable
    {
        #region Variables

        protected VisualElement m_TopElement;
        protected TransitionType m_TrasitionType;

        private const float transitionDuration = 0.2f; // Duration of the transition in seconds

        public bool IsHidden => m_TopElement.style.display == DisplayStyle.None;
        public bool IsShowing => m_TopElement.style.display == DisplayStyle.Flex;
        #endregion

        // Constructor
        /// <summary>
        /// Initializes a new instance of the UIView class.
        /// </summary>
        /// <param name="topElement">The topmost VisualElement in the UXML hierarchy.</param>
        public UIView(VisualElement topElement, TransitionType transitionType = TransitionType.None)
        {
            m_TopElement = topElement ?? throw new ArgumentNullException(nameof(topElement));
            m_TrasitionType = transitionType;
            Initialize();
        }

        #region Common Methods
        public void Show()
        {
            OnShowing();
            m_TopElement.style.display = DisplayStyle.Flex;

            if (m_TrasitionType == TransitionType.Fade)
            {
                m_TopElement.AddToClassList("view__fade");
                m_TopElement.AddToClassList("view__fade--visible");
                m_TopElement.RemoveFromClassList("view__fade--invisible");
            }
            OnShown();
        }

        public void Hide()
        {
            OnHiding();
            m_TopElement.style.display = DisplayStyle.None;

            if (m_TrasitionType == TransitionType.Fade)
            {
                m_TopElement.RemoveFromClassList("view__fade");
                m_TopElement.RemoveFromClassList("view__fade--visible");
                m_TopElement.AddToClassList("view__fade--invisible");
            }
            OnHidden();
        }
        #endregion

        #region Virtual Methods
        public virtual void Dispose()
        {
            UnRegisterEventCallbacks();
            UnRegisterButtonCallbacks();
        }

        public virtual void Initialize()
        {
            SetVisualElements();
            RegisterButtonCallbacks();
        }

        protected virtual void OnShowing()
        {
            // Empty body
        }

        protected virtual void OnShown()
        {
            // Empty body
        }

        protected virtual void OnHiding()
        {
            // Empty body
        }

        protected virtual void OnHidden()
        {
            // Empty body
        }

        protected virtual void SetVisualElements()
        {
            // Empty body
        }

        protected virtual void RegisterEventCallbacks()
        {
            // Empty body
        }

        protected virtual void RegisterButtonCallbacks()
        {
            // Empty body
        }

        protected virtual void UnRegisterEventCallbacks()
        {
            // Empty body
        }

        protected virtual void UnRegisterButtonCallbacks()
        {
            // Empty body
        }
        #endregion
    }
}
