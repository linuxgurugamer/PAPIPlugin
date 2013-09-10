#region Usings

using System;
using PAPIPlugin.Impl;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using UnityEngine;

#endregion

namespace PAPIPlugin.Arrays
{
    public class PAPIArray : AbstractMultiArray<IStaticLightArray>
    {
        public const float DefaultLightRadius = 10.0f;

        public const double DefaultTargetGlidePath = 6;

        /// <summary>
        ///     If the difference of the gliepath from the target is more than this the whole array will show either red or white.
        /// </summary>
        public const double DefaultGlideslopeTolerance = 1.5;

        private const string ModelUrl = "PAPIPlugin/Models/papi/model";

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
            // We need to return an object which does not rely on rotations
            var retObject = new GameObject();
            retObject.transform.localPosition = Vector3.zero;
            retObject.transform.localEulerAngles = Vector3.zero;

            var papiModel = SetupPapiModel();
            papiModel.transform.parent = retObject.transform;

            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            var material = new Material(Util.GetFileContents("FlareShader.shader"));
            obj.renderer.sharedMaterial = material;

            obj.renderer.receiveShadows = false;
            obj.renderer.castShadows = false;

            obj.transform.parent = retObject.transform;
            obj.transform.localPosition = Vector3.forward * 3;
            obj.transform.localScale = new Vector3(2, 2, 2);

            var sphereCollider = obj.GetComponent<SphereCollider>();
            sphereCollider.enabled = false;

            return retObject;
        }

        private static GameObject SetupPapiModel()
        {
            var containerObject = new GameObject();

            var papiModel = GameDatabase.Instance.GetModel(ModelUrl);
            papiModel.SetActive(true);

            // Set correct layer for kerbal collision
            papiModel.SetLayerRecursively(15);

            papiModel.transform.parent = containerObject.transform;
            papiModel.transform.localRotation = Quaternion.LookRotation(Vector3.up);
            papiModel.transform.localPosition = Vector3.zero;

            SetupPapiLods(containerObject, papiModel);

            return containerObject;
        }

        private static void SetupPapiLods(GameObject containerObject, GameObject papiModel)
        {
            var lodGroup = containerObject.AddComponent<LODGroup>();

            var papiRenderer = papiModel.renderer;

            var lods = new LOD[1];
            lods[0] = new LOD(0.001f, new[] {papiRenderer});

            lodGroup.SetLODS(lods);
            lodGroup.RecalculateBounds();
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

            var currentCamera = FlightCamera.fetch.mainCamera;

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
                _lightGameObjects[index].GetComponentsInChildren<Renderer>().ForEach(renderer => renderer.material.SetColor("_Color", color));
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
