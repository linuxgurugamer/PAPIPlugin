#region Usings

using System;
using PAPIPlugin.Impl;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using UnityEngine;

#endregion

namespace PAPIPlugin.Arrays
{
    public class PAPIArray : AbstractMultiArray<IStaticLightArray>, IConfigNode
    {
        public const float DefaultLightRadius = 10.0f;

        public const double DefaultTargetGlidePath = 6;

        /// <summary>
        ///     If the difference of the gliepath from the target is more than this the whole array will show either red or white.
        /// </summary>
        public const double DefaultGlideslopeTolerance = 1.5;

        public PAPIArray()
        {
            TargetGlideslope = DefaultTargetGlidePath;
            GlideslopeTolerance = DefaultGlideslopeTolerance;
        }

        protected float LightRadius { get; set; }

        public double GlideslopeTolerance { get; set; }

        public double TargetGlideslope { get; set; }

        protected override GameObject CreateLightDisplay()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            var material = new Material(Shader.Find("Particles/Additive"));
            obj.renderer.sharedMaterial = material;

            obj.transform.localScale = new Vector3(LightRadius, LightRadius, LightRadius);

            var sphereCollider = obj.GetComponent<SphereCollider>();
            sphereCollider.enabled = false;

            return obj;
        }

        public override void Initialize(ILightGroup @group, GameObject parentObj)
        {
            base.Initialize(@group, parentObj);

            if (group != null)
            {
                group.GetOrAddTypeManager<PAPITypeManager>();
            }
        }

        protected override IStaticLightArray CreateChildArray()
        {
            return new StaticLightArray();
        }

        public override void Load(ConfigNode node)
        {
            base.Load(node);

            GlideslopeTolerance = node.ConvertValue("GlideslopeTolerance", DefaultGlideslopeTolerance);
            TargetGlideslope = node.ConvertValue("TargetGlideslope", DefaultTargetGlidePath);
            LightRadius = node.ConvertValue("LightRadius", DefaultLightRadius).ClampAndLog(0.0f, float.MaxValue);
        }

        public override void Update()
        {
            base.Update();

            var currentCamera = Camera.main;

            if (currentCamera == null)
            {
                return;
            }

            var relativePosition = _arrayObject.transform.InverseTransformPoint(currentCamera.transform.position);

            var normalizedPosition = relativePosition.normalized;
            // As the local normal is (0, 1, 0), y is the result of normal * normalizedPosition.
            var normalDot = normalizedPosition.y;

            var directionDot = normalizedPosition.z;

            var angle = 90 - Math.Acos(normalDot) * (180 / Math.PI);

            var difference = angle - TargetGlideslope;

            for (var i = 0; i < LightCount; i++)
            {
                if (directionDot <= 0)
                {
                    SetColor(i, new Color(0, 0, 0, 0));
                }
                else
                {
                    // Use the direction dot for alpha to fade the lights out
                    var color = GetArrayPartColor(i, difference);
                    color.a = directionDot;
                    SetColor(i, color);
                }
            }
        }

        private void SetColor(int index, Color color)
        {
            if (_lightArrays == null)
            {
                _lightGameObjects[index].renderer.material.SetColor("_TintColor", color);
            }
            else
            {
                _lightArrays[index].LightColor = color;
            }
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
            double temp = index - (LightCount / 2);
            // ReSharper disable once PossibleLossOfFraction
            temp = temp / (LightCount / 2);

            return temp > difference ? Color.red : Color.white;
        }
    }
}
