using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHUD : MonoBehaviour {

	[SerializeField]
	Vector2 showPos = new Vector2 (0, 0);
	[SerializeField]
	Vector2 hidePos = new Vector2 (0, -70);

	RectTransform rect;
	bool shouldHide = true;


	void Start () {
		rect = GetComponent<RectTransform> ();
		HideHud ();
	}

	public void HideHud(UnityEngine.EventSystems.BaseEventData e){
		HideHud ();
	}	

	public void HideHud(){
		shouldHide = true;
		StartCoroutine (Hide ());
	}	
	IEnumerator Hide(){
		while (shouldHide && Vector2.Distance(rect.anchoredPosition,hidePos)>0.1f) {
			rect.anchoredPosition= Vector2.Lerp (rect.anchoredPosition, hidePos, 0.05f);
			yield return null;
		}
		rect.anchoredPosition= hidePos;
	}

	public void ShowHud(UnityEngine.EventSystems.BaseEventData e){
		ShowHud ();
	}

	public void ShowHud(){
		shouldHide = false;
		StartCoroutine (Show ());
	}
	IEnumerator Show(){
		while (!shouldHide && Vector2.Distance(rect.anchoredPosition,showPos)>0.1f) {
			rect.anchoredPosition= Vector2.Lerp (rect.anchoredPosition, showPos, 0.05f);
			yield return null;
		}
		rect.anchoredPosition = showPos;
	}
	


}
