#region Usings

using UnityEngine;

#endregion

namespace PAPIPlugin.Interfaces
{
    public interface IStaticLightArray : ILightArray
    {
        Color LightColor { set; }
    }
}
