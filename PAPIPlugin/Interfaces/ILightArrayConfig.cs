#region Usings

using System.Collections.Generic;

#endregion

namespace PAPIPlugin.Interfaces
{
    /// <summary>
    ///     Represents a configuration of light arrays. This is used to group all known <see cref="ILightGroup" /> instances
    ///     together.
    /// </summary>
    /// <remarks>
    ///     It is up to the implementation to actually chose a way of creating the light groups.
    /// </remarks>
    public interface ILightArrayConfig
    {
        /// <summary>
        ///     The light groups in this config object.
        /// </summary>
        IEnumerable<ILightGroup> LightArrayGroups { get; }

        /// <summary>
        ///     Gets or sets the debug mode of the config object.
        /// </summary>
        bool DebugMode { get; set; }
    }
}
