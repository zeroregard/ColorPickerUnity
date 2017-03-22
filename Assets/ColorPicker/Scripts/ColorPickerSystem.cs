using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
namespace ColorPickerUnity
{
    [Serializable]
    public class ColorProperty //I mainly have this class just to be able to collapse the properties in the inspector
    {
        public ColorPropertyUI Red;
        public ColorPropertyUI Green;
        public ColorPropertyUI Blue;
        public ColorPropertyUI Hue;
        public ColorPropertyUI Saturation;
        public ColorPropertyUI Lightness;
    }
    public class ColorPickerSystem : MonoBehaviour
    {
        public bool usesAlpha = true;
        public bool usesInputFields = true;

        [SerializeField]  private ColorProperty colorProperties = new ColorProperty(); //if !usesInputFields, hide this.
        [HideInInspector] private List<ColorPropertyUI> colorPropertiesList; //Used to iterate the properties
        [SerializeField]  private HexSanitizer hexInput;
        [SerializeField]  private ColorPickerSlider sliderRGBHSL;
        [SerializeField]  private Slider sliderAlpha;
        [SerializeField]  private InputField inputAlpha;
        [SerializeField]  private ColorPickerTexturizer colorPickerTwoD;

        private RGBHSL currentRGBHSL;
        [SerializeField] private Image imageCurrentColor;



        public void Start()
        {
            if (usesInputFields)
            {
                colorPropertiesList = new List<ColorPropertyUI> { colorProperties.Red, colorProperties.Green, colorProperties.Blue, colorProperties.Hue, colorProperties.Saturation, colorProperties.Lightness };
            }
            RGBHSL_SliderValueChanged(0);
            if (!usesAlpha)
            {
                sliderAlpha.gameObject.SetActive(false);
            }
            else
            {
                Alpha_SliderValueChanged(sliderAlpha);
            }
        }

        public enum ColorViewMode
        {
            Hue, Saturation, Lightness, Red, Green, Blue, NONE
        }

        public ColorViewMode CurrentMode = ColorViewMode.Hue;

        public void SetMode(ColorPropertyUI colorProperty)
        {
            foreach (var property in colorPropertiesList)
            {
                property.radioButton.isOn = false;
            }
            colorProperty.radioButton.isOn = true;
            CurrentMode = colorProperty.colorViewType;
            sliderRGBHSL.ChangeSliderTexture(CurrentMode, currentRGBHSL);
            sliderRGBHSL.ChangeSliderValue(CurrentMode, currentRGBHSL);
            UpdatePicker(sliderRGBHSL.sliderColorPicker.value);
            colorPickerTwoD.colorPickerCursor.rectTransform.anchoredPosition = GetSpecificColorLocation(currentRGBHSL);
        }
#region ColorSetting
        public void SetColorFromHexInput(RGBHSL color)
        {
            UpdateCurrentColor(color);
            sliderRGBHSL.ChangeSliderTexture(CurrentMode, color);
            sliderRGBHSL.ChangeSliderValue(CurrentMode, color);
            sliderAlpha.value = color.A;
            inputAlpha.text = color.A.ToString();
            UpdatePicker(sliderRGBHSL.sliderColorPicker.value);
            colorPickerTwoD.colorPickerCursor.rectTransform.anchoredPosition = GetSpecificColorLocation(currentRGBHSL);
        }

        public void SetColorFromRGBInputs()
        {
            byte r = Convert.ToByte(colorProperties.Red.inputField.text);
            byte g = Convert.ToByte(colorProperties.Green.inputField.text);
            byte b = Convert.ToByte(colorProperties.Blue.inputField.text);
            byte a = currentRGBHSL.A;

            RGBHSL c = new RGBHSL(r, g, b, a);
            UpdateCurrentColor(c);
            sliderRGBHSL.ChangeSliderTexture(CurrentMode, c);
            sliderRGBHSL.ChangeSliderValue(CurrentMode, c);
            UpdatePicker(sliderRGBHSL.sliderColorPicker.value);
            colorPickerTwoD.colorPickerCursor.rectTransform.anchoredPosition = GetSpecificColorLocation(currentRGBHSL);
            colorProperties.Hue.SetValueFromSystem(c);
            colorProperties.Saturation.SetValueFromSystem(c);
            colorProperties.Lightness.SetValueFromSystem(c);
            SetHexInput(c);
        }

        public void SetColorFromHSLInputs()
        {
            int h = Convert.ToInt32(colorProperties.Hue.inputField.text);
            int s = Convert.ToInt32(colorProperties.Saturation.inputField.text);
            int l = Convert.ToInt32(colorProperties.Lightness.inputField.text);

            RGBHSL c = ColorConverter.ConvertHSL(h, s / 100f, l / 100f);
            c.A = currentRGBHSL.A;
            UpdateCurrentColor(c);
            sliderRGBHSL.ChangeSliderTexture(CurrentMode, c);
            sliderRGBHSL.ChangeSliderValue(CurrentMode, c);
            UpdatePicker(sliderRGBHSL.sliderColorPicker.value);
            colorPickerTwoD.colorPickerCursor.rectTransform.anchoredPosition = GetSpecificColorLocation(currentRGBHSL);
            colorProperties.Red.SetValueFromSystem(c);
            colorProperties.Green.SetValueFromSystem(c);
            colorProperties.Blue.SetValueFromSystem(c);
            SetHexInput(c);
        }

        public void SetColorFromXYPicker(RGBHSL color)
        {
            if (usesInputFields)
            {
                foreach (var property in colorPropertiesList)
                {
                    property.SetValueFromSystem(color);
                }
                SetHexInput(color);
            }
            UpdateCurrentColor(color);
            sliderRGBHSL.ChangeSliderTexture(CurrentMode, color);
        }
#endregion
        /// <summary>
        /// Updates the stored color (CurrentHexColor) and the visualization of the color
        /// </summary>
        /// <param name="color"></param>
        public void UpdateCurrentColor(RGBHSL color)
        {
            currentRGBHSL = color;
            imageCurrentColor.color = ColorConverter.ConvertRGBHSL(color);
        }


        public void SetHexInput(RGBHSL c)
        {
            if (!usesAlpha)
            {
                hexInput.ChangeInput(c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2"));
            }
            else
            {
                hexInput.ChangeInput(c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2") + c.A.ToString("X2"));
            }
        }

        public void GetCurrentCursorColor()
        {
            Vector2 pos = colorPickerTwoD.colorPickerCursor.rectTransform.anchoredPosition;
            Color c = colorPickerTwoD.texturePixelsArray[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)];
            RGBHSL hc = ColorConverter.ConvertColor(c);
            SetColorFromXYPicker(hc);
        }

        public Vector2 GetSpecificColorLocation(RGBHSL rgb)
        {
            Vector2 colorLocation = new Vector2();
            Vector3 hsl = ColorConverter.ConvertRGB(new Color(rgb.R / 255f, rgb.G / 255f, rgb.B / 255f));
            switch (CurrentMode)
            {
                case ColorViewMode.Hue:
                    colorLocation = new Vector2(hsl.y * 255f, hsl.z * 255f);
                    break;
                case ColorViewMode.Saturation:
                    colorLocation = new Vector2((hsl.x / 359f) * 255f, hsl.z * 255f);
                    break;
                case ColorViewMode.Lightness:
                    colorLocation = new Vector2((hsl.x / 359f) * 255f, hsl.y * 255f);
                    break;
                case ColorViewMode.Red:
                    colorLocation = new Vector2(rgb.G, rgb.B);
                    break;
                case ColorViewMode.Green:
                    colorLocation = new Vector2(rgb.B, rgb.R);
                    break;
                case ColorViewMode.Blue:
                    colorLocation = new Vector2(rgb.R, rgb.G);
                    break;
                default:
                    break;
            }
            return colorLocation;
        }

        public void UpdatePicker(float value)
        {
            switch (CurrentMode)
            {
                case ColorViewMode.Hue:
                    colorPickerTwoD.GenerateTexture(CurrentMode, value * 360f);
                    break;
                case ColorViewMode.Saturation:
                    colorPickerTwoD.GenerateTexture(CurrentMode, value);
                    break;
                case ColorViewMode.Lightness:
                    colorPickerTwoD.GenerateTexture(CurrentMode, value);
                    break;
                case ColorViewMode.Red:
                case ColorViewMode.Green:
                case ColorViewMode.Blue:
                    colorPickerTwoD.GenerateTexture(CurrentMode, value);
                    break;
                default:
                    break;
            }
        }

        public void RGBHSL_SliderValueChanged(float value)
        {
            UpdatePicker(value);
            GetCurrentCursorColor();
        }

        public void SetAlphaFromInput()
        {
            byte input = Convert.ToByte(inputAlpha.text);
            currentRGBHSL.A = input;
            sliderAlpha.value = input;
            SetColorFromXYPicker(currentRGBHSL);
        }

        public void Alpha_SliderValueChanged(Slider s)
        {
            currentRGBHSL.A = (byte)s.value;
            inputAlpha.text = s.value.ToString();
            SetColorFromXYPicker(currentRGBHSL);
        }
    }
}