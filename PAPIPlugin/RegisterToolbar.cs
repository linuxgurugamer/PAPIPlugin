using UnityEngine;

using ToolbarControl_NS;

namespace PAPIPlugin.Impl
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(DefaultLightArrayManager.MODID, DefaultLightArrayManager.MODNAME);
        }
    }
}
