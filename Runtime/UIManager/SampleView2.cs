using UnityEngine;
using UnityEngine.UIElements;

namespace Zuy.Workspace.UI
{
    public class SampleView2 : UIView
    {
        #region Varaibles
        public Label titleLabel;
        public Label authorNameLabel;

        private const string _titleLabelName = "title";
        private const string _authorLabelName = "author_name";
        #endregion

        public SampleView2(VisualElement topElement, TransitionType transitionType = TransitionType.None) : base(topElement, transitionType)
        {

        }

        #region Override Methods
        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnShowing()
        {
            base.OnShowing();
        }

        protected override void OnShown()
        {
            base.OnShown();
        }

        protected override void OnHiding()
        {
            base.OnHiding();
        }

        protected override void OnHidden()
        {
            base.OnHidden();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            titleLabel = m_TopElement.Q<Label>(_titleLabelName);
            authorNameLabel = m_TopElement.Q<Label>(_authorLabelName);
        }

        protected override void RegisterEventCallbacks()
        {
            base.RegisterEventCallbacks();
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();
        }

        protected override void UnRegisterEventCallbacks()
        {
            base.UnRegisterEventCallbacks();
        }

        protected override void UnRegisterButtonCallbacks()
        {
            base.UnRegisterButtonCallbacks();
        }
        #endregion
    }
}
