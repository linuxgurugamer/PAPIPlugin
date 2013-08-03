namespace PAPIPlugin.Interfaces
{
    public interface ILightTypeManager
    {
        void Initialize(ILightGroup manager);

        void OnGui(int windowID);
    }
}
