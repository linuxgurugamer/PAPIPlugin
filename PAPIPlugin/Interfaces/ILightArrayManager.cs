#region Usings

using System;

#endregion

namespace PAPIPlugin.Interfaces
{
    public interface ILightArrayManager : IDisposable
    {
        event EventHandler ParsingFinished;

        event EventHandler AllLightConfigReloaded;

        ILightArrayConfig LightConfig { get; set; }

        ILightArrayConfig LoadConfig();

        void InitializeButton();

        void Update();

        void OnGUI();
    }
}
