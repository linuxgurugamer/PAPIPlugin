#region Usings

using System;
using PAPIPlugin.Interfaces;

#endregion

namespace PAPIPlugin.Arrays
{
    public abstract class AbstractLightArray : ILightArray
    {
        private bool _enabled;

        protected ILightGroup ParentGroup { get; private set; }

        protected ILightArrayManager ParentManager { get; private set; }

        protected AbstractLightArray()
        {
            Enabled = true;
        }

        #region ILightArray Members

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (Equals(_enabled, value))
                {
                    return;
                }

                _enabled = value;

                OnEnabledChanged();
            }
        }

        public virtual void Initialize(ILightGroup group)
        {
            ParentGroup = group;
        }

        public abstract void Update();

        public virtual void Destroy()
        {
            ParentManager = null;
        }

        public virtual void InitializeDisplay(ILightArrayManager arrayManager)
        {
            ParentManager = arrayManager;
        }

        #endregion

        protected event EventHandler EnabledChanged;

        protected virtual void OnEnabledChanged()
        {
            var handler = EnabledChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
