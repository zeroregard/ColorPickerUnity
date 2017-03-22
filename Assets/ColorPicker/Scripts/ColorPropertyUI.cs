using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ColorPickerUnity
{
    /// <summary>
    /// Hue, Saturation, Lightness, Red, Green, Blue
    /// </summary>
    public class ColorPropertyUI : MonoBehaviour
    {
        public InputField inputField;
        public Toggle radioButton;
        [SerializeField] private ColorPickerSystem colorPickerSystem;
        public ColorPickerSystem.ColorViewMode colorViewType;

        public void Start()
        {
            colorPickerSystem = FindObjectOfType<ColorPickerSystem>();
        }

        public void SetValueFromSystem(RGBHSL hc)
        {
            Vector3 colorText;
            Color c = ColorConverter.ConvertRGBHSL(hc);
            switch (colorViewType)
            {
                case ColorPickerSystem.ColorViewMode.Hue:
                    colorText = ColorConverter.ConvertRGB(c);
                    inputField.text = Mathf.FloorToInt(colorText.x).ToString();
                    break;
                case ColorPickerSystem.ColorViewMode.Saturation:
                    colorText = ColorConverter.ConvertRGB(c);
                    inputField.text = Mathf.RoundToInt(colorText.y * 100).ToString();
                    break;
                case ColorPickerSystem.ColorViewMode.Lightness:
                    colorText = ColorConverter.ConvertRGB(c);
                    inputField.text = Mathf.RoundToInt(colorText.z * 100).ToString();
                    break;
                case ColorPickerSystem.ColorViewMode.Red:
                    colorText = ColorConverter.ConvertNormalized(c);
                    inputField.text = colorText.x.ToString();
                    break;
                case ColorPickerSystem.ColorViewMode.Green:
                    colorText = ColorConverter.ConvertNormalized(c);
                    inputField.text = colorText.y.ToString();
                    break;
                case ColorPickerSystem.ColorViewMode.Blue:
                    colorText = ColorConverter.ConvertNormalized(c);
                    inputField.text = colorText.z.ToString();
                    break;
                default:
                    break;
            }
        }

        public void SelectThisMode()
        {
            colorPickerSystem.SetMode(this);
        }

        public void CheckUserInput()
        {
            float inputFloat = float.Parse(inputField.text);
            inputField.text = EvaluateValue(inputFloat).ToString();
            //something something system
        }

        public float EvaluateValue(float value)
        {
            switch (colorViewType)
            {
                case ColorPickerSystem.ColorViewMode.Blue:
                case ColorPickerSystem.ColorViewMode.Green:
                case ColorPickerSystem.ColorViewMode.Red:
                    return ColorConverter.Clamp(0, 255, value);
                case ColorPickerSystem.ColorViewMode.Hue:
                    return ColorConverter.ClampExclusive(0, 360, value);
                case ColorPickerSystem.ColorViewMode.Lightness:
                case ColorPickerSystem.ColorViewMode.Saturation:
                    return ColorConverter.Clamp(0, 100, value);
                default:
                    return 0;
            }
        }
    }
}