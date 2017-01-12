﻿using UnityEngine;
using UnityEngine.UI;

namespace SonicRealms.UI
{
    public class ResolutionPickerScreenSize : MonoBehaviour
    {
        public ResolutionSettings.ScreenSize Value
        {
            get { return _value; }
            set
            {
                _value = value;
                UpdateValue();
            }
        }

        [SerializeField]
        private Text _text;

        private ResolutionSettings.ScreenSize _value;

        protected virtual void Reset()
        {
            _text = GetComponentInChildren<Text>();
        }

        protected virtual void UpdateValue()
        {
            _text.text = string.Format("{0}x{1}", _value.Width, _value.Height);
        }
    }
}