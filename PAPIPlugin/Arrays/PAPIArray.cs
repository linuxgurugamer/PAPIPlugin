#region Usings

using System;
using PAPIPlugin.Impl;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace PAPIPlugin.Arrays
{
    public class PAPIArray : AbstractLightArray, IConfigNode
    {
        private const int PAPIPartCount = 4;

        private const float PAPILightRadius = 10.0f;

        private const double DefaultTargetGlidePath = 6;

        /// <summary>
        ///     If the difference of the gliepath from the target is more than this the whole array will show either red or white.
        /// </summary>
        private const double DefaultBadGlidepathVariance = 1.5;

        private static readonly Vector3 PAPILightDifference = Vector3.right * PAPILightRadius * 1.5f;

        private GameObject _papiGameObject;

        private GameObject[] _partObjects;

        private Vector3d _relativeSurfacePosition;

        public PAPIArray()
        {
            TargetGlidePath = DefaultTargetGlidePath;
            BadGlidepathVariance = DefaultBadGlidepathVariance;

            EnabledChanged += (sender, args) =>
            {
                if (Enabled)
                {
                    return;
                }

                foreach (var partObject in _partObjects)
                {
                    partObject.SetActive(false);
                }
            };
        }

        public double BadGlidepathVariance { get; set; }

        public double TargetGlidePath { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public double Heading { get; set; }

        #region IConfigNode Members

        public void Load(ConfigNode node)
        {
            BadGlidepathVariance = node.ConvertValue("BadGlidepath", DefaultBadGlidepathVariance);
            TargetGlidePath = node.ConvertValue("TargetGlidepath", DefaultTargetGlidePath);

            try
            {
                Longitude = node.ConvertValueWithException<double>("Longitude").ClampAndLog(-180, 180);
                Latitude = node.ConvertValueWithException<double>("Latitude").ClampAndLog(-90, 90);

                var headingDeg = node.ConvertValueWithException<double>("Heading").ClampAndLog(0, 360);
                Heading = (headingDeg / 180) * Math.PI;
            }
            catch (FormatException e)
            {
                Util.LogWarning(e.Message);
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override void Destroy()
        {
            foreach (var partObject in _partObjects)
            {
                Object.Destroy(partObject);
            }

            Object.Destroy(_papiGameObject);

            base.Destroy();
        }

        public override void Update()
        {
            if (!Enabled)
            {
                return;
            }

            var currentCamera = Camera.main;

            var relativePosition = _papiGameObject.transform.InverseTransformPoint(currentCamera.transform.position);

            var normalizedPosition = relativePosition.normalized;
            // As the local normal is (0, 1, 0), y is the result of normal * normalizedPosition.
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

        public override void InitializeDisplay(ILightArrayManager arrayManager)
        {
            base.InitializeDisplay(arrayManager);

            InitializePAPIParts(Latitude, Longitude, Heading);
        }

        public override void Initialize(ILightGroup @group)
        {
            base.Initialize(@group);

            @group.GetOrAddTypeManager<PAPITypeManager>();
        }

        /// <summary>
        ///     Initializes the whole array at the given latitude and longitude with the given altitude. The heading is the
        ///     direction to array looks to and should be in the range [0, 2 * PI).
        /// </summary>
        /// <param name="lat">The latitude</param>
        /// <param name="lon">The longitude</param>
        /// <param name="heading">The heading in radians.</param>
        private void InitializePAPIParts(double lat, double lon, double heading)
        {
            var parentBody = ParentGroup.ParentBody;

            var pqs = parentBody.pqsController;

            var surfaceNormal = parentBody.transform.InverseTransformDirection(parentBody.GetSurfaceNVector(lat, lon));
            var zeroAltSurface = pqs.transform.InverseTransformPoint(parentBody.GetWorldSurfacePosition(lat, lon, 0));

            var north = Vector3.up * (float) pqs.radius; // We are in local space so up * radius is the north pole

            var directionToNorth = (north - zeroAltSurface).normalized;

            var orthogonalNorthDir = Orthonormalise(directionToNorth, surfaceNormal);

            var anotherVector = Vector3d.Cross(surfaceNormal, orthogonalNorthDir);

            var headingVector = orthogonalNorthDir * Math.Cos(heading) + anotherVector * Math.Sin(heading);

            _papiGameObject = new GameObject();
            _papiGameObject.transform.parent = parentBody.transform;
            _papiGameObject.transform.localPosition = zeroAltSurface;
            _papiGameObject.transform.localRotation = Quaternion.LookRotation(headingVector, surfaceNormal);

            var maxHeight = double.MinValue;
            _partObjects = new GameObject[PAPIPartCount];
            for (var i = 0; i < PAPIPartCount; i++)
            {
                var obj = new GameObject();

                AddPAPIPart(obj);

                obj.transform.parent = _papiGameObject.transform;
                obj.transform.localPosition = (i - (PAPIPartCount / 2)) * PAPILightDifference;

                maxHeight = Math.Max(maxHeight, parentBody.GetSurfaceHeight(Latitude, Longitude));

                _partObjects[i] = obj;
            }

            maxHeight = Math.Max(0, maxHeight);
            _relativeSurfacePosition =
                parentBody.transform.InverseTransformPoint(parentBody.GetWorldSurfacePosition(lat, lon, maxHeight + PAPILightRadius * 0.5));
            _papiGameObject.transform.localPosition = _relativeSurfacePosition;
        }

        private static Vector3d Orthonormalise(Vector3d direction, Vector3d firstVector)
        {
            // This is basically the first step of a Gram–Schmidt process
            // See http://en.wikipedia.org/wiki/Gram%E2%80%93Schmidt_process

            return direction - Vector3d.Dot(firstVector, direction) * firstVector;
        }

        private static void AddPAPIPart(GameObject obj)
        {
            var lineRenderer = obj.AddComponent<LineRenderer>();

            lineRenderer.useWorldSpace = false;
            lineRenderer.transform.parent = obj.transform;
            lineRenderer.transform.localPosition = Vector3.zero;
            lineRenderer.transform.eulerAngles = Vector3.zero;

            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.SetColors(Color.red, Color.red);
            lineRenderer.SetWidth(PAPILightRadius, PAPILightRadius);
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.up * PAPILightRadius);
        }

        private void UpdatePAPIPart(int index, double difference, float alpha)
        {
            var gameObj = _partObjects[index];

            var lineRenderer = gameObj.GetComponent<LineRenderer>();

            var color = GetArrayPartColor(index, difference);
            color.a = alpha;
            lineRenderer.SetColors(color, color);
        }

        private Color GetArrayPartColor(int index, double difference)
        {
            if (difference < -BadGlidepathVariance)
            {
                return Color.red;
            }
            if (difference > BadGlidepathVariance)
            {
                return Color.white;
            }

            // This should map temp into [-1, 1]
            double temp = index - (PAPIPartCount / 2);
            temp = temp / (PAPIPartCount / 2);

            return temp > difference ? Color.red : Color.white;
        }
    }
}
