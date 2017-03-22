using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
namespace ColorPickerUnity
{
    public class ValueSanitizer : MonoBehaviour
    {
        [SerializeField] private InputField input;
        [SerializeField] private ColorPickerSystem system;
        public enum Type { RGB, HSL, Alpha };
        [Tooltip("When the user hits a radio button, the type is set in the ColorPickerSystem")]
        [SerializeField] private Type type;
        [SerializeField] private int minValue = 0;
        [SerializeField] private int maxValue = 255;

        public void Start()
        {
            system = FindObjectOfType<ColorPickerSystem>();
        }

        public void ChangeInput(int value)
        {
            input.text = value.ToString();
        }

        public void EndEdit()
        {
            string requestInput = input.text;
            int requestInt = minValue;
            try
            {
                requestInt = Convert.ToInt32(requestInput);
                if (requestInt >= minValue && requestInt <= maxValue)
                {
                    input.text = requestInput;
                    //system call
                }
                else
                {
                    throw new Exception(string.Format("User input was wrong, must be between {0} and {1}", minValue, maxValue));
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                input.text = minValue.ToString();
            }
            switch (type)
            {
                case Type.RGB:
                    system.SetColorFromRGBInputs();
                    break;
                case Type.HSL:
                    system.SetColorFromHSLInputs();
                    break;
                case Type.Alpha:
                    system.SetAlphaFromInput();
                    break;
            }

        }
    }
}