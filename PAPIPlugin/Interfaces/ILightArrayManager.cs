#region Usings

using System;

#endregion

namespace PAPIPlugin.Interfaces
{
    public interface ILightArrayManager : IDisposable
    {
        event EventHandler ParsingFinished;

        ILightArrayConfig LightConfig { get; set; }

        void LoadConfig();

        void Update();
    }
}
