namespace PAPIPlugin.Interfaces
{
    /// <summary>
    ///     Represents an abstract object which contains some lights of unspecified function. This object will live as long as
    ///     the game is running, <see cref="InitializeDisplay" />.
    /// </summary>
    public interface ILightArray
    {
        /// <summary>
        ///     Gets or sets the enabled status of the array.
        /// </summary>
        /// <remarks>
        ///     Implementing classes should use this to reduce the performence impact
        ///     when the array is disabled.
        /// </remarks>
        bool Enabled { get; set; }

        /// <summary>
        ///     Initializes the array with the given parent group.
        /// </summary>
        /// <remarks>
        ///     This function called when the parent group gets created and initialized it will only be called once for a given
        ///     light array.
        /// </remarks>
        /// <param name="group">The group that contains this array.</param>
        void Initialize(ILightGroup group);

        /// <summary>
        ///     Called each frame to update the display of the array.
        /// </summary>
        void Update();

        /// <summary>
        ///     Destroyes the resources used by the light array.
        /// </summary>
        /// <remarks>
        ///     Upon invocation the implementation should release any resources that were created in the invocation of
        ///     <see cref="InitializeDisplay" /> and return to a state in which <see cref="InitializeDisplay" /> can be called
        ///     again.
        /// </remarks>
        void Destroy();

        /// <summary>
        ///     Initializes the display within the given array manager.
        /// </summary>
        /// <remarks>
        ///     The implementation should use this function to initialize objects and resources which are used for displaying the
        ///     light array. This function may be called multiple times but only when <see cref="Destroy" /> has been called
        ///     before.
        /// </remarks>
        /// <param name="arrayManager">The array manager used for this display.</param>
        void InitializeDisplay(ILightArrayManager arrayManager);
    }
}
