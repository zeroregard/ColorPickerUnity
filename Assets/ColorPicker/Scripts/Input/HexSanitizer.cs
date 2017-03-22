using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class HexSanitizer : MonoBehaviour
{
    public InputField input;
    public string FallBackHex = "000000";
    public ColorPickerSystem system;

    public void Start()
    {
        input.characterLimit = 6;
        if (system.UsesAlpha)
        {
            input.characterLimit += 2;
        }
    }

    public void ChangeInput(string hex)
    {
        FallBackHex = hex;
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
            if (system.UsesAlpha)
            {
                byte a = (byte)int.Parse(requestInput.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                system.SetColorFromHexInput(new RGBHSL(r, g, b, a));
            }
            else
            {
                system.SetColorFromHexInput(new RGBHSL(r, g, b, 255));
            }

        }
        catch
        {
            input.text = FallBackHex;
        }
    }
}
