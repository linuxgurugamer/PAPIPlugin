#region Usings

using System;
using PAPIPlugin.Interfaces;
using UnityEngine;

#endregion

namespace PAPIPlugin.Arrays
{
    public abstract class AbstractLightArray : ILightArray
    {
        private bool _enabled;

        protected AbstractLightArray()
        {
            Enabled = true;
        }

        protected ILightGroup ParentGroup { get; private set; }
        protected GameObject ParentObject { get; private set; }

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

        public virtual void Initialize(ILightGroup group, GameObject parentObj)
        {
            ParentGroup = group;
            ParentObject = parentObj;
        }

        public abstract void Update();

        public virtual void Destroy()
        {
            ParentGroup.RemoveArray(this);
            ParentGroup = null;
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
