using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleXToScreen : MonoBehaviour {


	void Start () {
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width, 100);
	}

}
