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
        public const int DefaultPartCount = 4;

        public const float DefaultLightRadius = 10.0f;

        public const double DefaultTargetGlidePath = 6;

        /// <summary>
        ///     If the difference of the gliepath from the target is more than this the whole array will show either red or white.
        /// </summary>
        public const double DefaultGlideslopeTolerance = 1.5;

        private GameObject _papiGameObject;

        private GameObject[] _partObjects;

        public PAPIArray()
        {
            TargetGlideslope = DefaultTargetGlidePath;
            GlideslopeTolerance = DefaultGlideslopeTolerance;

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

        public double GlideslopeTolerance { get; set; }

        public double TargetGlideslope { get; set; }

        public int PartCount { get; set; }

        public float LightRadius { get; set; }

        public float LightDistance { get; set; }

        public Vector3 LocalPosition { get; set; }

        public void Load(ConfigNode node)
        {
            GlideslopeTolerance = node.ConvertValue("GlideslopeTolerance", DefaultGlideslopeTolerance);
            TargetGlideslope = node.ConvertValue("TargetGlideslope", DefaultTargetGlidePath);
            PartCount = node.ConvertValue("PartCount", DefaultPartCount);
            LightRadius = node.ConvertValue("LightRadius", DefaultLightRadius);
            LightDistance = node.ConvertValue("LightDistance", LightRadius * 1.5f);
            LocalPosition = node.ConvertValue("LocalPosition", Vector3.zero);
        }

        public void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }

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

            if (currentCamera == null)
            {
                return;
            }

            var relativePosition = _papiGameObject.transform.InverseTransformPoint(currentCamera.transform.position);

            var normalizedPosition = relativePosition.normalized;
            // As the local normal is (0, 1, 0), y is the result of normal * normalizedPosition.
            var normalDot = normalizedPosition.y;

            var directionDot = normalizedPosition.z;

            var angle = 90 - Math.Acos(normalDot) * (180 / Math.PI);

            var difference = angle - TargetGlideslope;

            for (var i = 0; i < PartCount; i++)
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

        public override void Initialize(ILightGroup @group, GameObject parentObject)
        {
            Util.LogWarning("Initialize: " + group + " " + parentObject);

            base.Initialize(@group, parentObject);

            @group.GetOrAddTypeManager<PAPITypeManager>();

            InitializePAPIParts(parentObject);
        }

        private void InitializePAPIParts(GameObject parentObject)
        {
            _papiGameObject = new GameObject();
            _papiGameObject.transform.parent = parentObject.transform;
            _papiGameObject.transform.localPosition = LocalPosition;
            _papiGameObject.transform.localEulerAngles = Vector3.zero;

            _partObjects = new GameObject[PartCount];
            for (var i = 0; i < PartCount; i++)
            {
                var obj = AddPAPIPart();

                obj.transform.parent = _papiGameObject.transform;
                obj.transform.localPosition = GetLocalLighPosition(i);
                obj.transform.localEulerAngles = Vector3.zero;

                _partObjects[i] = obj;
            }

            _papiGameObject.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        ///     Gets the local position given a zero-based index.
        /// </summary>
        /// <param name="i">The index of the light, zero-based</param>
        /// <returns>A local position specifying the light position</returns>
        private Vector3 GetLocalLighPosition(int i)
        {
            return Vector3.right * LightDistance * i;
        }

        private GameObject AddPAPIPart()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            var material = new Material(Shader.Find("Particles/Additive"));
            obj.renderer.sharedMaterial = material;

            obj.transform.localScale = new Vector3(LightRadius, LightRadius, LightRadius);

            var sphereCollider = obj.GetComponent<SphereCollider>();
            sphereCollider.enabled = false;

            return obj;
        }

        private void UpdatePAPIPart(int index, double difference, float alpha)
        {
            var gameObj = _partObjects[index];

            var color = GetArrayPartColor(index, difference);
            color.a = alpha;

            gameObj.renderer.material.SetColor("_TintColor", color);
        }

        private Color GetArrayPartColor(int index, double difference)
        {
            if (difference < -GlideslopeTolerance)
            {
                return Color.red;
            }
            if (difference > GlideslopeTolerance)
            {
                return Color.white;
            }

            // This should map temp into [-1, 1]
            double temp = index - (PartCount / 2);
// ReSharper disable once PossibleLossOfFraction
            temp = temp / (PartCount / 2);

            return temp > difference ? Color.red : Color.white;
        }
    }
}
