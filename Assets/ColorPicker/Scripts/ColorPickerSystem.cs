using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[Serializable]
public class ColorProperties //I mainly have this class just to be able to collapse the properties in the inspector
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
    public bool UsesAlpha = true;
    public ColorProperties colorProperties = new ColorProperties();
    private List<ColorPropertyUI> colorPropertiesList; //Used to foreach through the properties
    public HexSanitizer HexInput;
    public ColorPickerSlider RBGHSL_Slider;
    public Slider Alpha_Slider;
    public InputField AlphaInput;
    public ColorPickerTexturizer TwoDColorPicker;


    //public Color CurrentColor;
    [HideInInspector]
    public RGBHSL CurrentRGBHSL;
    public Image CurrentColorVisualizer;


    public void Start()
    {
        colorPropertiesList = new List<ColorPropertyUI> { colorProperties.Red, colorProperties.Green, colorProperties.Blue, colorProperties.Hue, colorProperties.Saturation, colorProperties.Lightness };
        RGBHSL_SliderValueChanged(0);
        if(!UsesAlpha)
        {
            Alpha_Slider.gameObject.SetActive(false);
        }
        else
        {
            Alpha_SliderValueChanged(Alpha_Slider);
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
            property.RadioButton.isOn = false;
        }
        colorProperty.RadioButton.isOn = true;
        CurrentMode = colorProperty.type;
        RBGHSL_Slider.ChangeSliderTexture(CurrentMode, CurrentRGBHSL);
        RBGHSL_Slider.ChangeSliderValue(CurrentMode, CurrentRGBHSL);
        UpdatePicker(RBGHSL_Slider.slider.value);
        TwoDColorPicker.Cursor.rectTransform.anchoredPosition = GetSpecificColorLocation(CurrentRGBHSL);
    }
    #region ColorSetting
    public void SetColorFromHexInput(RGBHSL color)
    {
        UpdateCurrentColor(color);
        RBGHSL_Slider.ChangeSliderTexture(CurrentMode, color);
        RBGHSL_Slider.ChangeSliderValue(CurrentMode, color);
        Alpha_Slider.value = color.A;
        AlphaInput.text = color.A.ToString();
        UpdatePicker(RBGHSL_Slider.slider.value);
        TwoDColorPicker.Cursor.rectTransform.anchoredPosition = GetSpecificColorLocation(CurrentRGBHSL);
    }

    public void SetColorFromRGBInputs()
    {
        byte r = Convert.ToByte(colorProperties.Red.Input.text);
        byte g = Convert.ToByte(colorProperties.Green.Input.text);
        byte b = Convert.ToByte(colorProperties.Blue.Input.text);
        byte a = CurrentRGBHSL.A;

        RGBHSL c = new RGBHSL(r, g, b, a);
        UpdateCurrentColor(c);
        RBGHSL_Slider.ChangeSliderTexture(CurrentMode, c);
        RBGHSL_Slider.ChangeSliderValue(CurrentMode, c);
        UpdatePicker(RBGHSL_Slider.slider.value);
        TwoDColorPicker.Cursor.rectTransform.anchoredPosition = GetSpecificColorLocation(CurrentRGBHSL);
        colorProperties.Hue.SetValueFromSystem(c);
        colorProperties.Saturation.SetValueFromSystem(c);
        colorProperties.Lightness.SetValueFromSystem(c);
        SetHexInput(c);
    }

    public void SetColorFromHSLInputs()
    {
        int h = Convert.ToInt32(colorProperties.Hue.Input.text);
        int s = Convert.ToInt32(colorProperties.Saturation.Input.text);
        int l = Convert.ToInt32(colorProperties.Lightness.Input.text);

        RGBHSL c = ColorConverter.ConvertHSL(h, s / 100f, l / 100f);
        c.A = CurrentRGBHSL.A;
        UpdateCurrentColor(c);
        RBGHSL_Slider.ChangeSliderTexture(CurrentMode, c);
        RBGHSL_Slider.ChangeSliderValue(CurrentMode, c);
        UpdatePicker(RBGHSL_Slider.slider.value);
        TwoDColorPicker.Cursor.rectTransform.anchoredPosition = GetSpecificColorLocation(CurrentRGBHSL);
        colorProperties.Red.SetValueFromSystem(c);
        colorProperties.Green.SetValueFromSystem(c);
        colorProperties.Blue.SetValueFromSystem(c);
        SetHexInput(c);
    }

    public void SetColorFromXYPicker(RGBHSL color)
    {
        
        foreach (var property in colorPropertiesList)
        {
            property.SetValueFromSystem(color);
        }
        
        UpdateCurrentColor(color);
        SetHexInput(color);
        RBGHSL_Slider.ChangeSliderTexture(CurrentMode, color);
    }
    #endregion
    /// <summary>
    /// Updates the stored color (CurrentHexColor) and the visualization of the color
    /// </summary>
    /// <param name="color"></param>
    public void UpdateCurrentColor(RGBHSL color)
    {
        CurrentRGBHSL = color;
        CurrentColorVisualizer.color = ColorConverter.ConvertRGBHSL(color);
    }


    public void SetHexInput(RGBHSL c)
    {
        if (!UsesAlpha)
        {
            HexInput.ChangeInput(c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2"));
        }
        else
        {
            HexInput.ChangeInput(c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2") + c.A.ToString("X2"));
        }
    }

    public void GetCurrentCursorColor()
    {
        Vector2 pos = TwoDColorPicker.Cursor.rectTransform.anchoredPosition;
        Color c = TwoDColorPicker.ColorArray[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)];
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
                TwoDColorPicker.GenerateTexture(CurrentMode, value * 360f);
                break;
            case ColorViewMode.Saturation:
                TwoDColorPicker.GenerateTexture(CurrentMode, value);
                break;
            case ColorViewMode.Lightness:
                TwoDColorPicker.GenerateTexture(CurrentMode, value);
                break;
            case ColorViewMode.Red:
            case ColorViewMode.Green:
            case ColorViewMode.Blue:
                TwoDColorPicker.GenerateTexture(CurrentMode, value);
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
        byte input = Convert.ToByte(AlphaInput.text);
        CurrentRGBHSL.A = input;
        Alpha_Slider.value = input;
        SetColorFromXYPicker(CurrentRGBHSL);
    }

    public void Alpha_SliderValueChanged(Slider s)
    {
        CurrentRGBHSL.A = (byte)s.value;
        AlphaInput.text = s.value.ToString();
        SetColorFromXYPicker(CurrentRGBHSL);
    }



}
