using System;
using System.Collections.Generic;

namespace PAPIPlugin
{
    public class LightArrayManager
    {
        private readonly IList<ILightArray> _lightArrays = new List<ILightArray>();

        public void AddArray(ILightArray array)
        {
            if (array==null)throw new ArgumentException("array");

            _lightArrays.Add(array);
        }

        public bool RemoveArray(ILightArray array)
        {
            return _lightArrays.Remove(array);
        }

        public void Update()
        {
            foreach (var lightArray in _lightArrays)
            {
                lightArray.Update();
            }
        }
    }
}