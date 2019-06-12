using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    Camera.main.depthTextureMode |= DepthTextureMode.Depth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
