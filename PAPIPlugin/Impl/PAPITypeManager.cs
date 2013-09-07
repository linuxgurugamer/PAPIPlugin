#region Usings

using System;
using System.Linq;
using PAPIPlugin.Arrays;
using PAPIPlugin.Interfaces;
using PAPIPlugin.UI;
using UnityEngine;

#endregion

namespace PAPIPlugin.Impl
{
    public class PAPITypeManager : ILightTypeManager
    {
        private EditableGUIField<double> _glideslopeField;

        private EditableGUIField<double> _glideslopeToleranceField;

        private bool _guiInitialized;

        private double _initialGlideslopeValue = PAPIArray.DefaultTargetGlidePath;

        private double _initialTargetGlideslopeValue = PAPIArray.DefaultGlideslopeTolerance;

        private ILightGroup _lightGroup;

        #region ILightTypeManager Members

        public void Initialize(ILightGroup group)
        {
            foreach (var lightArray in group.LightArrays.OfType<PAPIArray>())
            {
                _initialTargetGlideslopeValue = lightArray.GlideslopeTolerance;
                _initialGlideslopeValue = lightArray.TargetGlideslope;
            }

            group.LightArrayAdded += (sender, arguments) =>
                {
                    var papi = arguments.Array as PAPIArray;
                    if (papi == null)
                    {
                        return;
                    }

                    _initialTargetGlideslopeValue = papi.GlideslopeTolerance;
                    _initialGlideslopeValue = papi.TargetGlideslope;
                };

            _lightGroup = group;
        }

        public void OnGui(int windowID)
        {
            if (!_guiInitialized)
            {
                _glideslopeField = new EditableGUIField<double>(_initialGlideslopeValue, DoubleConvertDelegate);
                _glideslopeToleranceField = new EditableGUIField<double>(_initialTargetGlideslopeValue, DoubleConvertDelegate);

                _guiInitialized = true;
            }

            DoDegreeField("Glideslope", _glideslopeField);
            DoDegreeField("Glideslope tolerance", _glideslopeToleranceField);

            ApplyValues();
        }

        #endregion

        private static void DoDegreeField<T>(string name, EditableGUIField<T> field)
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
            foreach (var papiArray in _lightGroup.LightArrays.OfType<PAPIArray>())
            {
                if (!_glideslopeField.InvalidInput)
                {
                    papiArray.TargetGlideslope = _glideslopeField.Value;
                }

                if (!_glideslopeToleranceField.InvalidInput)
                {
                    papiArray.GlideslopeTolerance = _glideslopeToleranceField.Value;
                }
            }
        }
    }
}
