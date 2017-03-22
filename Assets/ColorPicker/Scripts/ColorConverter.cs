using UnityEngine;
using System.Collections;

namespace ColorPickerUnity
{
    /// <summary>
    /// Unity's RGBHSL does not have HSL fields, so we'll use our own
    /// </summary>
    [System.Serializable]
    public class RGBHSL
    {
        public byte R;  // Red,         0-255
        public byte G;  // Green,       0-255
        public byte B;  // Blue,        0-255
        public byte A;  // Alpha,       0-255 
        public float H; // Hue,         0-359 degrees
        public float S; // Saturation,  0-100 %
        public float L; // Lightness,   0-100 %

        public RGBHSL(byte R, byte G, byte B, byte A = 255)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;

            Vector3 HSL = ColorConverter.ConvertRGB(ColorConverter.ConvertRGBHSL(this));
            H = HSL.x;
            S = HSL.y;
            L = HSL.z;
        }

        public RGBHSL(byte R, byte G, byte B, float H, float S, float L, byte A = 255)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;

            this.H = H;
            this.S = S;
            this.L = L;
        }

        public RGBHSL(float H, float S, float L, byte A = 255)
        {
            this.H = H;
            this.S = S;
            this.L = L;
            this.A = A;

            RGBHSL c = ColorConverter.ConvertHSL(H, S, L);
            R = c.R;
            G = c.G;
            B = c.B;
        }
    }

    /// <summary>
    /// Conversion is based on this:
    /// http://www.rapidtables.com/convert/color/hsv-to-rgb.htm
    /// http://www.rapidtables.com/convert/color/rgb-to-hsv.htm
    /// </summary>
    public class ColorConverter : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hue">0..359</param>
        /// <param name="saturation">0..1</param>
        /// <param name="lightness">0..1</param>
        /// <returns></returns>
        public static RGBHSL ConvertHSL(float hue, float saturation, float lightness)
        {
            float C = lightness * saturation;
            float X = C * (1 - Mathf.Abs((hue / 60) % 2 - 1));
            float M = lightness - C;
            Vector3 rgbTemp = new Vector3(0, 0, 0);

            hue = ClampExclusive(0, 360, hue);

            if (0 <= hue && hue < 60)
                rgbTemp = new Vector3(C, X, 0);
            else if (60 <= hue && hue < 120)
                rgbTemp = new Vector3(X, C, 0);
            else if (120 <= hue && hue < 180)
                rgbTemp = new Vector3(0, C, X);
            else if (180 <= hue && hue < 240)
                rgbTemp = new Vector3(0, X, C);
            else if (240 <= hue && hue < 300)
                rgbTemp = new Vector3(X, 0, C);
            else if (300 <= hue && hue < 360)
                rgbTemp = new Vector3(C, 0, X);

            byte r = (byte)Mathf.RoundToInt((rgbTemp.x + M) * 255f);
            byte g = (byte)Mathf.RoundToInt((rgbTemp.y + M) * 255f);
            byte b = (byte)Mathf.RoundToInt((rgbTemp.z + M) * 255f);

            RGBHSL c = new RGBHSL(r, g, b, hue, saturation, lightness);
            return c;
        }

        public static Vector3 ConvertRGB(Color c)
        {
            float CMax = Mathf.Max(c.r, c.g, c.b);
            float CMin = Mathf.Min(c.r, c.g, c.b);
            float diff = CMax - CMin;

            float hue = GetHue(c, CMax, diff);
            float saturation = GetSaturation(diff, CMax);
            float lightness = CMax;
            return new Vector3(hue, saturation, lightness);
        }

        public static RGBHSL ConvertColor(Color c)
        {
            byte R = (byte)Mathf.RoundToInt(c.r * 255);
            byte G = (byte)Mathf.RoundToInt(c.g * 255);
            byte B = (byte)Mathf.RoundToInt(c.b * 255);
            byte A = (byte)Mathf.RoundToInt(c.a * 255);
            return new RGBHSL(R, G, B, A);
        }

        public static Color ConvertRGBHSL(RGBHSL c)
        {
            float R = c.R / 255f;
            float G = c.G / 255f;
            float B = c.B / 255f;
            float A = c.A / 255f;
            return new Color(R, G, B, A);

        }

        public static float GetHue(Color c, float CMax, float diff)
        {
            float temp = 0;
            if (diff == 0)
            {
                return 0;
            }
            else if (c.r == CMax)
            {
                temp = 60 * (((c.g - c.b) / diff) % 6);
            }
            else if (c.g == CMax)
            {
                temp = 60 * (((c.b - c.r) / diff) + 2);
            }
            else if (c.b == CMax)
            {
                temp = 60 * (((c.r - c.g) / diff) + 4);
            }
            if (temp < 0)
            {
                temp = 359 + temp;
            }
            return temp;
        }

        public static float GetSaturation(float diff, float CMax)
        {
            if (CMax == 0)
            {
                return 0;
            }
            else
            {
                return (diff / CMax);
            }
        }

        public static float ClampExclusive(float min, float maxEx, float value)
        {
            if (value < min)
            {
                return min;
            }
            else if (value >= maxEx)
            {
                return min;
            }
            else
            {
                return value;
            }

        }

        public static float Clamp(float min, float max, float value)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static Vector3 ConvertNormalized(Color c)
        {
            int r = Mathf.RoundToInt(c.r * 255f);
            int g = Mathf.RoundToInt(c.g * 255f);
            int b = Mathf.RoundToInt(c.b * 255f);

            return new Vector3(r, g, b);
        }
    }
}