#region Usings

using PAPIPlugin.Interfaces;
using PAPIPlugin.UI;
using Tac;
using UnityEngine;

#endregion

namespace PAPIPlugin
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class LightGUI : MonoBehaviour
    {
        private GroupWindow<ILightArrayConfig> _groupWindow;

        private Icon<LightGUI> _groupWindowIcon;

        public void Awake()
        {
            _groupWindowIcon = new Icon<LightGUI>(new Rect(Screen.width * 0.8f, 0.0f, 80.0f, 20.0f), "icon.png", "Light groups",
                                                  "Opens the light group overview", OnIconClickHandler);
            _groupWindowIcon.SetVisible(true);
        }

        private void OnIconClickHandler()
        {
            if (_groupWindow == null)
            {
                _groupWindow = new GroupWindow<ILightArrayConfig>(PAPIAddon.Config);
                _groupWindow.SetVisible(true);
            }
            else
            {
                _groupWindow.ToggleVisible();
            }
        }

        public void OnDestroy()
        {
            _groupWindowIcon.SetVisible(false);

            if (_groupWindow != null)
            {
                _groupWindow.SetVisible(false);
            }
        }
    }
}
