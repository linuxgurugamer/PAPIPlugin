#region Usings

using System;
using System.ComponentModel;
using UnityEngine;

#endregion

namespace PAPIPlugin.UI
{
    public class EditableGUIField<T>
    {
        #region Delegates

        public delegate string TryConvertFromString(string input, out T val);

        #endregion

        private readonly TryConvertFromString _convertDelegate;

        public EditableGUIField(T initial = default(T), TryConvertFromString convertDelegate = null)
        {
            _convertDelegate = convertDelegate;
            Value = initial;

            // Null compare is intentional to guard against NullRefs...
// ReSharper disable once CompareNonConstrainedGenericWithNull
            StringValue = initial == null ? string.Empty : initial.ToString();

            if (_convertDelegate == null)
            {
                _convertDelegate = DefaultConvertDelegate;
            }

            InitializeStyles();
        }

        public T Value { get; set; }

        public string StringValue { get; private set; }

        public bool InvalidInput { get; private set; }

        public string LastErrorText { get; private set; }

        public GUIStyle ValidStyle { get; set; }

        public GUIStyle InvalidStyle { get; set; }

        private void InitializeStyles()
        {
            ValidStyle = new GUIStyle(GUI.skin.textField);

            InvalidStyle = new GUIStyle(ValidStyle)
            {
                onHover = {textColor = Color.red},
                normal = {textColor = Color.red}
            };
        }

        public void LayoutTextField(params GUILayoutOption[] options)
        {
            StringValue = GUILayout.TextField(StringValue, InvalidInput ? InvalidStyle : ValidStyle, options);

            T outVal;
            LastErrorText = _convertDelegate(StringValue, out outVal);

            InvalidInput = LastErrorText != null;
            if (LastErrorText == null)
            {
                Value = outVal;
            }
        }

        private string DefaultConvertDelegate(string input, out T val)
        {
            val = default(T);

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            try
            {
                val = (T) typeConverter.ConvertFromInvariantString(input);

                return null;
            }
            catch (NotSupportedException e)
            {
                return string.Format("The input \"{0}\" cannot be converted to type \"{1}\":\n{2}.", input, typeof(T).FullName, e.Message);
            }
        }
    }
}
