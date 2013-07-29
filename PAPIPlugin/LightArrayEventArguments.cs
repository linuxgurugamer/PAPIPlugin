using System;
using PAPIPlugin.Interfaces;

namespace PAPIPlugin
{
    public class LightArrayEventArguments : EventArgs
    {
        public LightArrayEventArguments(ILightArray array)
        {
            Array = array;
        }

        public ILightArray Array { get; private set; }
    }
}