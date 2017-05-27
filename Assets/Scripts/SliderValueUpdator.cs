using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderValueUpdator : MonoBehaviour {

	[SerializeField] Text count;
	[SerializeField] Text speed;
	[SerializeField] Text compareTime;

	public void UpdateCount(float value){
		count.text = "Initial Object Count: "+value;
		SortingAlgorithms.count = (int)value;
	}
	public void UpdateSpeed(float value){
		speed.text = "Swap Speed: "+((float)(int)(value*100))/100;
		SortingAlgorithms.swapSpeed = value;
	}
	public void UpdateCompareTime(float value){
		compareTime.text = "Compare Time: " + ((float)(int)(value*100))/100;
		SortingAlgorithms.compareTime = value;
	}

}
