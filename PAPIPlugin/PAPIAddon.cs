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

            _arrayManager.AddArray(new PAPIArray(-0.0468, -74.708889, 73, 270));
            _arrayManager.AddArray(new PAPIArray(-0.036, -74.50963, 73, 90));
        }

        public void Update()
        {
            _arrayManager.Update();
        }
    }
}
