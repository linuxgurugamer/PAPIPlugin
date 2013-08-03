using System;
using System.Collections.Generic;

namespace PAPIPlugin.Interfaces
{
    public interface ILightGroup
    {
        event EventHandler<LightArrayEventArguments> LightArrayAdded;

        string Name { get; }
 
        CelestialBody ParentBody { get; }

        IEnumerable<ILightArray> LightArrays { get; }

        void Update();

        T GetOrAddTypeManager<T>() where T : ILightTypeManager, new();

        T GetOrAddTypeManager<T>(Func<T> creatorFunc) where T : ILightTypeManager;

        void OnGui(int windowId);
    }
}