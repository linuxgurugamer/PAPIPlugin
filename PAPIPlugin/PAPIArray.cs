#region Usings

using System;
using System.Linq;
using UnityEngine;

#endregion

namespace PAPIPlugin
{
    public class PAPIArray : ILightArray
    {
        private const int PAPIPartCount = 4;

        private const float PAPILightRadius = 10.0f;

        private const double TargetGlidePath = 6;

        /// <summary>
        ///     If the difference of the gliepath from the target is more than this the whole array will show either red or white.
        /// </summary>
        private const double BadGlidepathVariance = 1.5;

        private static readonly Vector3 PAPILightDifference = Vector3.right * PAPILightRadius * 1.5f;

        private readonly CelestialBody _kerbinBody;

        private GameObject _papiGameObject;

        private GameObject[] _partObjects;

        private Vector3d _relativeSurfacePosition;

        public PAPIArray(double lat, double lon, double alt, double heading)
        {
            _kerbinBody = FlightGlobals.Bodies.First(body => body.name == "Kerbin");

            InitializePAPIParts(lat, lon, alt, heading * Math.PI / 180);
        }

        /// <summary>
        ///     Initializes the whole array at the given latitude and longitude with the given altitude. The heading is the
        ///     direction to array looks to and should be in the range [0, 2 * PI).
        /// </summary>
        /// <param name="lat">The latitude</param>
        /// <param name="lon">The longitude</param>
        /// <param name="alt">The altitude</param>
        /// <param name="heading">The heading in radians.</param>
        private void InitializePAPIParts(double lat, double lon, double alt, double heading)
        {
            _relativeSurfacePosition = _kerbinBody.transform.InverseTransformPoint(_kerbinBody.GetWorldSurfacePosition(lat, lon, alt));

            var surfaceNormal = _kerbinBody.transform.InverseTransformDirection(_kerbinBody.GetSurfaceNVector(lat, lon));

            var pqs = _kerbinBody.pqsController;
            var north = Vector3.up * (float) pqs.radius; // We are in local space so up * radius is the north pole

            var directionToNorth = (north - surfaceNormal).normalized;

            var orthogonalNorthDir = Orthonormalise(directionToNorth, surfaceNormal);

            var anotherVector = Vector3d.Cross(surfaceNormal, orthogonalNorthDir);

            var headingVector = orthogonalNorthDir * Math.Cos(heading) + anotherVector * Math.Sin(heading);

            Util.LogWarning(_relativeSurfacePosition.ToString());

            _papiGameObject = new GameObject();
            _papiGameObject.transform.parent = _kerbinBody.transform;
            _papiGameObject.transform.localPosition = _relativeSurfacePosition;
            _papiGameObject.transform.localRotation = Quaternion.LookRotation(headingVector, surfaceNormal);

            _partObjects = new GameObject[PAPIPartCount];
            for (var i = 0; i < PAPIPartCount; i++)
            {
                var obj = new GameObject();

                var lineRenderer = AddPAPIPart(obj);
                lineRenderer.useWorldSpace = false;

                lineRenderer.transform.parent = _papiGameObject.transform;
                lineRenderer.transform.localPosition = (i - (PAPIPartCount / 2)) * PAPILightDifference;

                _partObjects[i] = obj;
            }
        }

        private static Vector3d Orthonormalise(Vector3d direction, Vector3d firstVector)
        {
            // This is basically the first step of a Gram–Schmidt process
            // See http://en.wikipedia.org/wiki/Gram%E2%80%93Schmidt_process

            return direction - Vector3d.Dot(firstVector, direction) * firstVector;
        }

        private static LineRenderer AddPAPIPart(GameObject obj)
        {
            var lineRenderer = obj.AddComponent<LineRenderer>();

            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.SetColors(Color.red, Color.red);
            lineRenderer.SetWidth(PAPILightRadius, PAPILightRadius);
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.up * PAPILightRadius);

            return lineRenderer;
        }

        public void Update()
        {
            var currentCamera = Camera.main;

            var relativePosition = _papiGameObject.transform.InverseTransformPoint(currentCamera.transform.position);

            var normalizedPosition = relativePosition.normalized;
            // As the local normal is (0, 1, 0), y is the dot of normal * normalizedPosition.
            var normalDot = normalizedPosition.y;

            var directionDot = normalizedPosition.z;

            var angle = 90 - Math.Acos(normalDot) * (180 / Math.PI);

            var difference = angle - TargetGlidePath;

            for (var i = 0; i < PAPIPartCount; i++)
            {
                if (directionDot <= 0)
                {
                    _partObjects[i].SetActive(false);
                }
                else
                {
                    _partObjects[i].SetActive(true);

                    // Use the direction dot for alpha to fade the lights out
                    UpdatePAPIPart(i, difference, directionDot);
                }
            }
        }

        private void UpdatePAPIPart(int index, double difference, float alpha)
        {
            var gameObj = _partObjects[index];

            var lineRenderer = gameObj.GetComponent<LineRenderer>();

            var color = GetArrayPartColor(index, difference);
            color.a = alpha;
            lineRenderer.SetColors(color, color);
        }

        private static Color GetArrayPartColor(int index, double difference)
        {
            if (difference < -BadGlidepathVariance) return Color.red;
            if (difference > BadGlidepathVariance) return Color.white;

            // This should map temp into [-1, 1]
            double temp = index - (PAPIPartCount / 2);
            temp = temp / (PAPIPartCount / 2);

            return temp > difference ? Color.red : Color.white;
        }
    }
}
