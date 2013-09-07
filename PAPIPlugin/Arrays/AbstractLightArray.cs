#region Usings

using System;
using PAPIPlugin.Interfaces;
using UnityEngine;

#endregion

namespace PAPIPlugin.Arrays
{
    public abstract class AbstractLightArray : ILightArray
    {
        protected AbstractLightArray()
        {
        }

        protected ILightGroup ParentGroup { get; private set; }
        protected GameObject ParentObject { get; private set; }

        #region ILightArray Members

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
