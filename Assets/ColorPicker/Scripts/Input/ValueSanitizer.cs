using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ValueSanitizer : MonoBehaviour
{
    public InputField input;
    public ColorPickerSystem system;
    public enum Type { RGB, HSL, Alpha};
    public Type type;
    public int MinValue = 0;
    public int MaxValue = 255;

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
        int requestInt = MinValue;
        try
        {
            requestInt = Convert.ToInt32(requestInput);
            if(requestInt >= MinValue && requestInt <= MaxValue)
            {
                input.text = requestInput;
                //system call
            }
            else
            {
                throw new Exception(string.Format("User input was wrong, must be between {0} and {1}", MinValue, MaxValue));
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
            input.text = MinValue.ToString();
        }
        switch(type)
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
