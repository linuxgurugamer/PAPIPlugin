#region Usings

using PAPIPlugin.Interfaces;
using UnityEngine;

#endregion

namespace PAPIPlugin.Arrays
{
    public abstract class AbstractLightArray : ILightArray
    {
        protected ILightGroup ParentGroup { get; private set; }

        protected GameObject ParentObject { get; private set; }

        #region ILightArray Members

        public virtual bool Enabled { protected get; set; }

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
    }
}
