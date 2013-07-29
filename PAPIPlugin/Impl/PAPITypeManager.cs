#region Usings

using System.Collections.Generic;
using System.Linq;
using PAPIPlugin.Arrays;
using PAPIPlugin.Interfaces;

#endregion

namespace PAPIPlugin.Impl
{
    public class PAPITypeManager : ILightTypeManager
    {
        private readonly IList<ILightArray> _papiArrays = new List<ILightArray>();

        #region ILightTypeManager Members

        public void Initialize(ILightGroup group)
        {
            foreach (var lightArray in group.LightArrays.Where(array => array is PAPIArray))
            {
                _papiArrays.Add(lightArray);
            }

            group.LightArrayAdded += (sender, arguments) =>
            {
                if (arguments.Array is PAPIArray)
                    _papiArrays.Add(arguments.Array);
            };
        }

        #endregion
    }
}
