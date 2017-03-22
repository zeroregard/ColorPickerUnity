using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace ColorPickerUnity
{
    public class ColorPickerSlider : MonoBehaviour
    {
        [SerializeField] public Slider sliderColorPicker;
        [SerializeField] public ColorPickerSystem colorPickerSystem;
        [SerializeField] public Material colorPickerSliderMaterial;
        [HideInInspector] public Texture2D colorPickerSliderTexture;
        [SerializeField] private Image sliderImage;

        private ColorPickerSystem.ColorViewMode currentMode = ColorPickerSystem.ColorViewMode.NONE;

        public void ValueChanged()
        {
            colorPickerSystem.RGBHSL_SliderValueChanged(sliderColorPicker.value);
        }

        public void ChangeSliderTexture(ColorPickerSystem.ColorViewMode mode, RGBHSL selectedRGBHSL)
        {
            if (colorPickerSliderTexture != null)
                Destroy(colorPickerSliderTexture);

            colorPickerSliderTexture = new Texture2D(1, 256);
            Color[] ColorArray = new Color[256];
            switch (mode)
            {
                case ColorPickerSystem.ColorViewMode.Hue:
                    ColorArray = HueMode();
                    break;
                case ColorPickerSystem.ColorViewMode.Saturation:
                    ColorArray = SaturationMode(selectedRGBHSL);
                    break;
                case ColorPickerSystem.ColorViewMode.Lightness:
                    ColorArray = LightnessMode(selectedRGBHSL);
                    break;
                case ColorPickerSystem.ColorViewMode.Red:
                    ColorArray = RedMode(selectedRGBHSL.G, selectedRGBHSL.B);
                    break;
                case ColorPickerSystem.ColorViewMode.Green:
                    ColorArray = GreenMode(selectedRGBHSL.R, selectedRGBHSL.B);
                    break;
                case ColorPickerSystem.ColorViewMode.Blue:
                    ColorArray = BlueMode(selectedRGBHSL.R, selectedRGBHSL.G);
                    break;
                default:
                    break;
            }
            for (int i = 0; i < 256; i++)
            {
                colorPickerSliderTexture.SetPixel(0, i, ColorArray[i]);
            }
            colorPickerSliderTexture.Apply();
            colorPickerSliderMaterial.mainTexture = colorPickerSliderTexture;
            sliderImage.SetMaterialDirty();

            currentMode = mode;
        }

        public void ChangeSliderValue(ColorPickerSystem.ColorViewMode mode, RGBHSL selectedRGBHSL)
        {
            ;
            switch (mode)
            {
                case ColorPickerSystem.ColorViewMode.Hue:
                    sliderColorPicker.value = selectedRGBHSL.H / 359f;
                    break;
                case ColorPickerSystem.ColorViewMode.Saturation:
                    sliderColorPicker.value = selectedRGBHSL.S;
                    break;
                case ColorPickerSystem.ColorViewMode.Lightness:
                    sliderColorPicker.value = selectedRGBHSL.L;
                    break;
                case ColorPickerSystem.ColorViewMode.Red:
                    sliderColorPicker.value = selectedRGBHSL.R / 255f;
                    break;
                case ColorPickerSystem.ColorViewMode.Green:
                    sliderColorPicker.value = selectedRGBHSL.G / 255f;
                    break;
                case ColorPickerSystem.ColorViewMode.Blue:
                    sliderColorPicker.value = selectedRGBHSL.B / 255f;
                    break;
                default:
                    break;
            }
        }

        public Color[] HueMode()
        {
            Color[] ColorArray = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                Color c = ColorConverter.ConvertRGBHSL(ColorConverter.ConvertHSL((i * 360) / 255f, 1, 1));
                ColorArray[i] = c;
            }
            return ColorArray;
        }

        public Color[] SaturationMode(RGBHSL selectedColor)
        {
            Color[] ColorArray = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                Color c = ColorConverter.ConvertRGBHSL(ColorConverter.ConvertHSL(selectedColor.H, i / 255f, selectedColor.L));
                ColorArray[i] = c;
            }
            return ColorArray;
        }

        public Color[] LightnessMode(RGBHSL selectedColor)
        {
            Color[] ColorArray = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                Color c = ColorConverter.ConvertRGBHSL(ColorConverter.ConvertHSL(selectedColor.H, selectedColor.S, i / 255f));
                ColorArray[i] = c;
            }
            return ColorArray;
        }

        public Color[] RedMode(float green, float blue)
        {
            Color[] ColorArray = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                Color c = new Color(i / 255f, green / 255f, blue / 255f);
                ColorArray[i] = c;
            }
            return ColorArray;
        }

        public Color[] GreenMode(float red, float blue)
        {
            Color[] ColorArray = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                Color c = new Color(red / 255f, i / 255f, blue / 255f);
                ColorArray[i] = c;
            }
            return ColorArray;
        }

        public Color[] BlueMode(float red, float green)
        {
            Color[] ColorArray = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                Color c = new Color(red / 255f, green / 255f, i / 255f);
                ColorArray[i] = c;
            }
            return ColorArray;
        }
    }
}