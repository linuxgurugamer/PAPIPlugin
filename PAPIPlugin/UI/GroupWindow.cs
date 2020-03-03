﻿#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using Tac;
using UnityEngine;

#endregion

namespace PAPIPlugin.UI
{
    public class GroupWindow<T> : Window<T> where T : ILightArrayConfig
    {
        private readonly T _arrayConfig;

        private readonly IDictionary<string, bool> _toggleState = new Dictionary<string, bool>();

        private GUIStyle _toggleStyle;

        private GUIStyle _buttonStyle;

        public event EventHandler AllLightConfigSaved;

        //public event EventHandler AllLightConfigReloaded;

        public GroupWindow(T arrayConfig) : base("Group configuration", 400, 200)
        {
            _arrayConfig = arrayConfig;
        }

        protected override void ConfigureStyles()
        {
            base.ConfigureStyles();

            if (_toggleStyle == null)
            {
                _toggleStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Normal,
                    padding = {top = 4, bottom = 4},
                    stretchWidth = true,
                    stretchHeight = false
                };
            }

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Normal,
                    padding = { top = 4, bottom = 4 },
                    stretchWidth = true,
                    stretchHeight = false
                };
            }
        }

        protected override void DrawWindowContents(int windowId)
        {
            foreach (var lightGroup in _arrayConfig.LightArrayGroups.Where(lightGroup => lightGroup.Name != null))
            {
                if (!_toggleState.ContainsKey(lightGroup.Name))
                {
                    _toggleState[lightGroup.Name] = false;
                }

                var newVal = GUILayout.Toggle(_toggleState[lightGroup.Name], lightGroup.Name, _toggleStyle);

                if (newVal)
                {
                    lightGroup.OnGui(windowId);
                }

                _toggleState[lightGroup.Name] = newVal;
            }

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save Std", _buttonStyle))
                {
                    AllLightConfigSaved(this, new SaveAllBool(false));
                }
                if (GUILayout.Button("Save All", _buttonStyle))
                {
                    AllLightConfigSaved(this, new SaveAllBool(true));
                }
                if (GUILayout.Button("Reload PAPI.config", _buttonStyle))
                {
                    PAPIAddon.fetch._arrayManager.ReloadPAPIConfig();
                    //AllLightConfigReloaded(this, null);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
    internal class SaveAllBool: EventArgs
    {
        internal bool saveAll;
        internal SaveAllBool(bool b)
        {
            saveAll = b;
        }
    }
}
