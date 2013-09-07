using System;
using System.Collections.Generic;

namespace PAPIPlugin.Interfaces
{
    public interface ILightGroup
    {
        event EventHandler<LightArrayEventArguments> LightArrayAdded;

        string Name { get; }

        IEnumerable<ILightArray> LightArrays { get; }

        T GetOrAddTypeManager<T>() where T : ILightTypeManager, new();

        T GetOrAddTypeManager<T>(Func<T> creatorFunc) where T : ILightTypeManager;

        void OnGui(int windowId);

        void AddArray(ILightArray array);

        bool RemoveArray(ILightArray array);
    }
}