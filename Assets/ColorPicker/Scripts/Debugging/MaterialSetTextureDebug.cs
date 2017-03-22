using UnityEngine;
using System.Collections;

public class MaterialSetTextureDebug : MonoBehaviour {

    public Material m;
    public Texture t;
	// Use this for initialization
	void Update ()
    {
        m.SetTexture(0, t);
	}
}
