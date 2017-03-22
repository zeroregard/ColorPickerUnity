using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
namespace ColorPickerUnity
{
    public class HexSanitizer : MonoBehaviour
    {
        [SerializeField] private InputField input;
        [SerializeField] private string fallBackHex = "000000";
        [SerializeField] private ColorPickerSystem colorPickerSystem;

        public void Start()
        {
            input.characterLimit = 6;
            if (colorPickerSystem.usesAlpha)
            {
                input.characterLimit += 2;
            }
        }

        public void ChangeInput(string hex)
        {
            fallBackHex = hex;
            input.text = hex;
        }

        public void EndEdit()
        {
            string requestInput = input.text;
            try
            {
                byte r = (byte)int.Parse(requestInput.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = (byte)int.Parse(requestInput.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = (byte)int.Parse(requestInput.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                if (colorPickerSystem.usesAlpha)
                {
                    byte a = (byte)int.Parse(requestInput.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                    colorPickerSystem.SetColorFromHexInput(new RGBHSL(r, g, b, a));
                }
                else
                {
                    colorPickerSystem.SetColorFromHexInput(new RGBHSL(r, g, b, 255));
                }

            }
            catch
            {
                input.text = fallBackHex;
            }
        }
    }
}