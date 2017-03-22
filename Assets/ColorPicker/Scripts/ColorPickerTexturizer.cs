using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ColorPickerUnity
{
    public class ColorPickerTexturizer : MonoBehaviour
    {
        [HideInInspector]   private Texture2D colorPickerTexture;
        [SerializeField]    private Material colorPickerMaterial;
                            public Image colorPickerCursor;
        [SerializeField]    private RectTransform rectTransform;
        [SerializeField]    private ColorPickerSystem colorPickerSystem;
        [HideInInspector]   public Color[,] texturePixelsArray;
        [SerializeField]    private Image colorPickerImage;

        public void GenerateTexture(ColorPickerSystem.ColorViewMode mode, float value)
        {
            if (colorPickerTexture != null)
                Destroy(colorPickerTexture);
            colorPickerTexture = new Texture2D(256, 256);
            texturePixelsArray = null;
            switch (mode)
            {
                case ColorPickerSystem.ColorViewMode.Hue:
                    texturePixelsArray = HueView(value);
                    break;
                case ColorPickerSystem.ColorViewMode.Saturation:
                    texturePixelsArray = SaturationView(value);
                    break;
                case ColorPickerSystem.ColorViewMode.Lightness:
                    texturePixelsArray = LightnessView(value);
                    break;
                case ColorPickerSystem.ColorViewMode.Red:
                    texturePixelsArray = RedView(value);
                    break;
                case ColorPickerSystem.ColorViewMode.Green:
                    texturePixelsArray = GreenView(value);
                    break;
                case ColorPickerSystem.ColorViewMode.Blue:
                    texturePixelsArray = BlueView(value);
                    break;
                default:
                    break;
            }
            SetTexturePixels(colorPickerTexture, texturePixelsArray);
        }

        void SetTexturePixels(Texture2D texture, Color[,] colorarray)
        {
            for (int height = 0; height < 256; height++)
            {
                for (int width = 0; width < 256; width++)
                {
                    texture.SetPixel(width, height, colorarray[width, height]);
                }
            }
            texture.Apply();
            colorPickerMaterial.mainTexture = texture;
            colorPickerImage.SetMaterialDirty();
        }

        Color[,] HueView(float hue)
        {
            Color[,] ColorArray = new Color[256, 256];
            for (int height = 0; height < 256; height++)
            {
                for (int width = 0; width < 256; width++)
                {
                    ColorArray[width, height] = ColorConverter.ConvertRGBHSL(ColorConverter.ConvertHSL(hue, width / 255f, height / 255f));
                }
            }
            return ColorArray;
        }

        Color[,] SaturationView(float saturation)
        {
            Color[,] ColorArray = new Color[256, 256];
            for (int height = 0; height < 256; height++)
            {
                for (int width = 0; width < 256; width++)
                {
                    ColorArray[width, height] = ColorConverter.ConvertRGBHSL(ColorConverter.ConvertHSL((width * 360) / 255f, saturation, height / 255f));
                }
            }
            return ColorArray;
        }

        Color[,] LightnessView(float lightness)
        {
            Color[,] ColorArray = new Color[256, 256];
            for (int height = 0; height < 256; height++)
            {
                for (int width = 0; width < 256; width++)
                {
                    ColorArray[width, height] = ColorConverter.ConvertRGBHSL(ColorConverter.ConvertHSL((width * 360) / 255f, height / 255f, lightness));
                }
            }
            return ColorArray;
        }

        Color[,] RedView(float red)
        {
            Color[,] ColorArray = new Color[256, 256];
            for (int height = 0; height < 256; height++)
            {
                for (int width = 0; width < 256; width++)
                {
                    ColorArray[width, height] = new Color(red, width / 255f, height / 255f);
                }
            }
            return ColorArray;
        }

        Color[,] GreenView(float green)
        {
            Color[,] ColorArray = new Color[256, 256];
            for (int height = 0; height < 256; height++)
            {
                for (int width = 0; width < 256; width++)
                {
                    ColorArray[width, height] = new Color(height / 255f, green, width / 255f);
                }
            }
            return ColorArray;
        }

        Color[,] BlueView(float blue)
        {
            Color[,] ColorArray = new Color[256, 256];
            for (int height = 0; height < 256; height++)
            {
                for (int width = 0; width < 256; width++)
                {
                    ColorArray[width, height] = new Color(width / 255f, height / 255f, blue);
                }
            }
            return ColorArray;
        }

        public void SelectColor()
        {
            Vector2 cursorPosition = ConvertToRectPosition(Input.mousePosition);
            MoveCursor(cursorPosition);
            Color c = texturePixelsArray[Mathf.FloorToInt(cursorPosition.x), Mathf.FloorToInt(cursorPosition.y)];
            RGBHSL hc = ColorConverter.ConvertColor(c);
            colorPickerSystem.SetColorFromXYPicker(hc);
        }

        public Vector2 ConvertToRectPosition(Vector2 mousePos)
        {
            //Mouse Position is normal cartesian coordinate system vector2
            Vector2 size = rectTransform.sizeDelta;
            var position = mousePos - new Vector2(rectTransform.transform.position.x, rectTransform.transform.position.y);

            if (position.x > size.x - 1)
                position.x = size.x - 1;
            else if (position.x < 0)
                position.x = 0;

            if (position.y > size.y - 1)
                position.y = size.y - 1;
            else if (position.y < 0)
                position.y = 0;
            return position;
        }

        void MoveCursor(Vector2 position)
        {
            colorPickerCursor.GetComponent<RectTransform>().anchoredPosition = position;
        }
    }
}