#region Usings

using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using UnityEngine;

#endregion

namespace PAPIPlugin.Arrays
{
    public class StaticLightArray : AbstractMultiArray<IStaticLightArray>, IStaticLightArray
    {
        private Color _lightColor;

        protected float LightRadius { get; set; }

        #region IStaticLightArray Members

        public Color LightColor
        {
            private get { return _lightColor; }
            set
            {
                _lightColor = value;
                if (_lightArrays != null)
                {
                    foreach (var staticLightArray in _lightArrays)
                    {
                        staticLightArray.LightColor = value;
                    }
                }
                else if (_lightGameObjects != null)
                {
                    foreach (var lightGameObject in _lightGameObjects)
                    {
                        lightGameObject.renderer.material.SetColor("_TintColor", value);
                    }
                }
            }
        }

        #endregion

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

        protected override IStaticLightArray CreateChildArray()
        {
            return new StaticLightArray();
        }

        public override void Load(ConfigNode node)
        {
            LightColor = node.ConvertValue("LightColor", Color.white);
            LightRadius = node.ConvertValue("LightRadius", 1.0f).ClampAndLog(0.0f, float.MaxValue);

            base.Load(node);
        }
    }
}
