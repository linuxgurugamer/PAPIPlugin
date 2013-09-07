#region Usings

using System.Diagnostics;
using System.Linq;
using PAPIPlugin.Impl;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using UnityEngine;

#endregion

namespace PAPIPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class PAPIAddon : MonoBehaviour
    {
        public static ILightArrayConfig Config { get; private set; }

        public void Awake()
        {
            var defaultConfig = new DefaultLightArrayConfig();
            defaultConfig.LoadConfig();

            Config = defaultConfig;
        }
    }
}
