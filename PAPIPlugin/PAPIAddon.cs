#region Usings

using UnityEngine;

#endregion

namespace PAPIPlugin
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class PAPIAddon : MonoBehaviour
    {
        private LightArrayManager _arrayManager;

        public void Awake()
        {
            _arrayManager = new LightArrayManager();

            _arrayManager.AddArray(new PAPIArray(-0.0468, -74.688889, 78, 270));
            _arrayManager.AddArray(new PAPIArray(-0.036, -74.52763, 78, 90));
        }

        public void Update()
        {
            _arrayManager.Update();
        }
    }
}
