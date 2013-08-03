#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using PAPIPlugin.Arrays;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using PAPIPlugin.UI;
using UnityEngine;

#endregion

namespace PAPIPlugin.Impl
{
    public class PAPITypeManager : ILightTypeManager
    {
        private readonly IList<PAPIArray> _papiArrays = new List<PAPIArray>();

        private EditableGUIField<double> _glideslopeField;

        private EditableGUIField<double> _glideslopeToleranceField;

        private bool _guiInitialized = false;

        #region ILightTypeManager Members

        public void Initialize(ILightGroup group)
        {
            Util.LogInfo(group);

            foreach (var lightArray in group.LightArrays.OfType<PAPIArray>())
            {
                Util.LogInfo(lightArray);
                _papiArrays.Add(lightArray);
            }

            group.LightArrayAdded += (sender, arguments) =>
            {
                var papi = arguments.Array as PAPIArray;
                if (papi != null)
                {
                    _papiArrays.Add(papi);
                }
            };
        }

        public void OnGui(int windowID)
        {
            if (!_guiInitialized)
            {
                _glideslopeField = new EditableGUIField<double>(PAPIArray.DefaultTargetGlidePath, DoubleConvertDelegate);
                _glideslopeToleranceField = new EditableGUIField<double>(PAPIArray.DefaultGlideslopeTolerance, DoubleConvertDelegate);

                _guiInitialized = true;
            }

            DoDegreeField("Glideslope", _glideslopeField);
            DoDegreeField("Glideslope tolerance", _glideslopeToleranceField);

            ApplyValues();
        }

        #endregion

        private void DoDegreeField<T>(string name, EditableGUIField<T> field)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(name + ":");
                field.LayoutTextField();
                GUILayout.Label("°");
            }
            GUILayout.EndHorizontal();
        }

        private static string DoubleConvertDelegate(string input, out double val)
        {
            val = 0;

            try
            {
                val = double.Parse(input);
                return null;
            }
            catch (FormatException e)
            {
                return string.Format("Failed to convert \"{0}\" to double: {1}", input, e.Message);
            }
        }

        private void ApplyValues()
        {
            if (!_glideslopeField.InvalidInput)
            {
                foreach (var papiArray in _papiArrays)
                {
                    papiArray.TargetGlideslope = _glideslopeField.Value;
                }
            }

            if (!_glideslopeToleranceField.InvalidInput)
            {
                foreach (var papiArray in _papiArrays)
                {
                    papiArray.GlideslopeTolerance = _glideslopeToleranceField.Value;
                }
            }
        }
    }
}
