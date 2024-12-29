using UnityEngine;
using System.Collections;

public class TextureScroller : MonoBehaviour {

	///***********************************************************************
	/// This class scrolls the background texture of the main game.
	///***********************************************************************

	private float offset;
	private float damper = 0.04f;
	public float coef = 1;

	void LateUpdate (){
		offset +=  damper * Time.deltaTime * coef;
		GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", new Vector2(offset, 0));
	}
}