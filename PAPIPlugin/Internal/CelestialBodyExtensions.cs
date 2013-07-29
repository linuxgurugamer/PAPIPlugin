#region Usings

using UnityEngine;

#endregion

namespace PAPIPlugin.Internal
{
    public static class CelestialBodyExtensions
    {
        public static double GetSurfaceHeight(this CelestialBody body, double latitude, double longitude)
        {
            var height =
                body.pqsController.GetSurfaceHeight(QuaternionD.AngleAxis(longitude, Vector3d.down) *
                                                    QuaternionD.AngleAxis(latitude, Vector3d.forward) * Vector3d.right);

            return height - body.Radius;
        }
    }
}
