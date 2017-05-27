using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortableObject : MonoBehaviour {
	public float value;
	MeshRenderer meshRenderer;
	[SerializeField]
	GameObject complete;
	bool alreadyComplete;

	void Awake(){
		alreadyComplete = false;
		meshRenderer = GetComponent<MeshRenderer> ();
		Unhighlight ();
		value = Random.Range (1f, 20f);
		transform.localScale = new Vector3 (1, value, 1);
	}

	public void Highlight(){
		if (alreadyComplete) {
			complete.SetActive (false);
		}
		meshRenderer.enabled = true;
	}
	public void Unhighlight(){
		if (alreadyComplete) {
			complete.SetActive (true);
		}
		meshRenderer.enabled = false;
	}
	public void Complete(){
		complete.SetActive (true);
		alreadyComplete = true;
	}
	public void Uncomplete(){
		complete.SetActive(false);
		alreadyComplete=false;
	}
}
