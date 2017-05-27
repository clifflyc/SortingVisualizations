using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingAlgorithms : MonoBehaviour {

	public enum Algorithm{
		Bubble,
		Selection,
		Insertion,
		Quick,
		Merge,
		Tree
	}

	[SerializeField]
	Algorithm algorithm = Algorithm.Bubble;

	public static int count = 50;
	static int lastCount = 50;
	public static float swapSpeed = 2f;
	public static float compareTime = 0.03f;
	[SerializeField]
	Vector3 forkVector = new Vector3 (0, 0, -5);
	[SerializeField] GameObject prefab;

	SortableObject[] pieces;

	void Start () {
		Setup ();
	}

	float GetXOfIndex(int i){
		return -lastCount + i * 2;
	}
	void Setup(){
		lastCount = count;
		pieces = new SortableObject[lastCount];
		for (int i = 0; i < lastCount; i++) {
			GameObject g = (GameObject)Instantiate (prefab, new Vector3 (GetXOfIndex (i), 0, 0), Quaternion.identity);
			pieces [i] = g.GetComponent<SortableObject> ();
		}
		StartCoroutine (LerpCamera ());
		switch (algorithm) {
		case Algorithm.Bubble:
			StartCoroutine (BubbleSort ());
			break;
		case Algorithm.Selection:
			StartCoroutine (SelectionSort ());
			break;
		case Algorithm.Insertion:
			StartCoroutine (InsertionSort ());
			break;
		case Algorithm.Quick:
			StartCoroutine (QuickSort ());
			break;
		case Algorithm.Merge:
			StartCoroutine (MergeSort ());
			break;
		case Algorithm.Tree:
			StartCoroutine (TreeSort ());
			break;
		}
	}

	public void SetBubble(){
		SetAlgorithm (Algorithm.Bubble);
	}
	public void SetSelection(){
		SetAlgorithm (Algorithm.Selection);
	}
	public void SetInsertion(){
		SetAlgorithm (Algorithm.Insertion);
	}
	public void SetQuick(){
		SetAlgorithm (Algorithm.Quick);
	}
	public void SetMerge(){
		SetAlgorithm (Algorithm.Merge);
	}
	public void SetTree(){
		SetAlgorithm (Algorithm.Tree);
	}
	public void SetAlgorithm(Algorithm algorithm){
		StopAllCoroutines ();
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Sortable")) {
			Destroy (gameObject);
		}

		this.algorithm = algorithm;
		Setup ();
	}



	IEnumerator LerpCamera(){
		Vector3 targetPos = Camera.main.transform.position;
		targetPos.z = -count - 20;
		while (Vector3.Distance (Camera.main.transform.position, targetPos) > 0.1f) {
			Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, targetPos, 0.02f);
			yield return null;
		}
		Camera.main.transform.position = targetPos;
	}
	IEnumerator BubbleSort(){
		yield return BubbleSort (0, pieces.Length - 1);
	}

	IEnumerator BubbleSort(int start, int end){
		for (int i = end; i > start; i--) {
			for (int j = start; j < i; j++) {
				pieces [j].Highlight ();
				pieces [j + 1].Highlight ();
				if (pieces [j].value > pieces [j + 1].value) {
					yield return Swap (j, j + 1);
				}
				yield return new WaitForSeconds (compareTime);
				pieces [j].Unhighlight ();
				pieces [j + 1].Unhighlight ();
			}
			pieces [i].Complete ();
		}
		pieces [start].Complete ();
		yield return null;
	}
	IEnumerator SelectionSort(){
		for (int i =0; i <pieces.Length-1; i++) {
			int p = i;
			for (int j = i+1; j < pieces.Length; j++) {
				pieces [j].Highlight ();
				yield return new WaitForSeconds (compareTime);
				if (pieces [p].value > pieces [j].value) {
					pieces [p].Unhighlight ();
					p = j;
				} else {
					pieces [j].Unhighlight ();
				}
			}
			yield return Swap (i, p);
			pieces [i].Complete ();
		}
		pieces [pieces.Length-1].Complete ();
		yield return null;
	}


	IEnumerator InsertionSort(){
		yield return InsertionSort (0, pieces.Length - 1);
	}

	IEnumerator InsertionSort(int start, int end){
		pieces [start].Complete ();

		for (int i = start+1; i <= end; i++) {
			SortableObject t = pieces [i];
			t.Highlight ();
			pieces [i - 1].Highlight (); //for visual purposes, highlight the comparison piece before comparing. so if no comparison happens, comparison visual is still noticeable
			yield return Fork (i);

			int index = i-1;
			while (index >= start) {
				pieces [index].Highlight ();
				yield return new WaitForSeconds (compareTime);
				if (t.value < pieces [index].value) {
					yield return ShiftHigher (index, t);
				} else {
					pieces [index].Unhighlight ();
					break;
				}
				pieces [index].Unhighlight ();
				index--;
			}
			pieces [index + 1] = t;
			t.Complete ();
			t.Unhighlight ();
			yield return Unfork (index + 1);
		}
	}

	IEnumerator QuickSort(){
		yield return QuickSort (0,pieces.Length-1);
	}

	IEnumerator QuickSort (int start, int end){
		if (end - start < 4) {
			yield return BubbleSort (start, end);
		} else {
			SortableObject pivot = pieces [start];
			pivot.Highlight ();
			yield return Fork (start);
			int a = start;
			int b = end + 1;

			bool moveA = true;
			while (a != b) {
				if (moveA) {
					if (a > start)
						pieces [a].Unhighlight ();
					if (a == end)
						break;
					a++;
					pieces [a].Highlight ();
					yield return new WaitForSeconds (compareTime);
					if (pieces [a].value > pivot.value) {
						moveA = false;
					}
				} else {
					if (b <= end)
						pieces [b].Unhighlight ();
					b--;
					pieces [b].Highlight ();
					yield return new WaitForSeconds (compareTime);
					if (pieces [b].value < pivot.value) {
						moveA = true;
						yield return Swap (a, b);
					}
				}
			}
			pieces [a].Unhighlight ();

			if (pieces [a].value > pivot.value) {
				a--;
			}
			yield return Swap (a, start);
			pivot.Complete ();
			pivot.Unhighlight ();
			yield return Unfork (a);
			if (a != start)
				yield return QuickSort (start, a - 1);
			if (a != end)
				yield return QuickSort (a + 1, end);
		}
	}

	IEnumerator MergeSort(){
		yield return MergeSort (0, pieces.Length - 1);
	}

	IEnumerator MergeSort(int start, int end){
		if (end - start <4) {
			yield return BubbleSort (start, end);
		} else {
			int mid = (start + end) / 2;
			int a = start;
			int b = mid + 1;
			yield return MergeSort (a, mid);
			yield return MergeSort (b, end);

			for (int w = start; w <= end; w++) {
				pieces [w].Uncomplete ();
				pieces [w].Unhighlight ();
				//yield return null;
			}

			SortableObject[] newPieces = new SortableObject[end - start + 1];
			int i = 0;

			while (a <= mid && b <= end) {
				pieces [a].Highlight ();
				pieces [b].Highlight ();
				yield return new WaitForSeconds (compareTime);
				if (pieces [a].value < pieces [b].value) {
					yield return Fork_Merge (a,start+i);
					pieces [a].Complete ();
					newPieces [i] = pieces [a];
					a++;
				} else {
					yield return Fork_Merge (b,start+i);
					pieces [b].Complete ();
					newPieces [i] = pieces [b];
					b++;
				}
				i++;
			}
			while (a <= mid) {
				pieces [a].Highlight ();
				yield return Fork_Merge (a,start+i);
				pieces [a].Complete ();
				newPieces [i] = pieces [a];
				a++;
				i++;
			}
			while (b <= end) {
				pieces [b].Highlight ();
				yield return Fork_Merge (b,start+i);
				pieces [b].Complete ();
				newPieces [i] = pieces [b];
				b++;
				i++;
			}
			//unfork all very quickly, this starts the unfork animation for everything in newPieces except for the last.
			//it does one unfork per frame (does not wait until animation completes
			//then it unforks the last item in newPieces, it waits for this animation to complete.
			for (int j = 0; j < end-start; j++) {
				pieces [start + j] = newPieces [j];
				pieces [start + j].Complete ();
				pieces [start + j].Unhighlight();
				StartCoroutine (Unfork (start + j));
				yield return null;
			}
			pieces [end] = newPieces [end - start];
			pieces [end].Complete ();
			pieces [end].Unhighlight ();
			yield return Unfork (end);
		}

	}

	IEnumerator TreeSort(){
		yield return null;
	}
	IEnumerator Swap(int a, int b){
		Vector3 posA = pieces [b].transform.position;
		Vector3 posB = pieces [a].transform.position;
		posA.x = GetXOfIndex (a);
		posB.x = GetXOfIndex (b);
		float timeElapsed = 0;
		while (Vector3.Distance(pieces[a].transform.position,posB)>0.02f) {
			pieces [a].transform.position = Vector3.Lerp (pieces [a].transform.position, posB, swapSpeed * timeElapsed);
			pieces [b].transform.position = Vector3.Lerp (pieces [b].transform.position, posA, swapSpeed * timeElapsed);
			timeElapsed += Time.deltaTime;
			yield return null;
		}

		pieces [a].transform.position = posB;
		pieces [b].transform.position = posA;
		SortableObject temp = pieces [b];
		pieces [b] = pieces [a];
		pieces [a] = temp;
	}


	IEnumerator Fork(int p){
		float timeElapsed = 0;
		Vector3 targetPos = pieces [p].transform.position + forkVector;
		while (Vector3.Distance (pieces [p].transform.position, targetPos) > 0.02f) {
			pieces [p].transform.position = Vector3.Lerp (pieces [p].transform.position, targetPos, swapSpeed * timeElapsed);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		pieces [p].transform.position = targetPos;
	}

	IEnumerator Unfork(int p){
		float timeElapsed = 0;
		Vector3 targetPos = pieces [p].transform.position - forkVector;
		while (Vector3.Distance (pieces [p].transform.position, targetPos) > 0.02f) {
			pieces [p].transform.position = Vector3.Lerp (pieces [p].transform.position, targetPos, swapSpeed * timeElapsed);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		pieces [p].transform.position = targetPos;
	}

	IEnumerator Fork_Merge(int p,int targetIndex){
		float timeElapsed = 0;
		Vector3 targetPos = pieces [p].transform.position + forkVector;
		targetPos.x = GetXOfIndex (targetIndex);
		while (Vector3.Distance (pieces [p].transform.position, targetPos) > 0.02f) {
			pieces [p].transform.position = Vector3.Lerp (pieces [p].transform.position, targetPos, swapSpeed * timeElapsed);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		pieces [p].transform.position = targetPos;
	}

	IEnumerator MoveToIndexPos(int o, int i){
		float timeElapsed = 0;
		Vector3 targetPos = pieces [o].transform.position;
		targetPos.x = GetXOfIndex (i);
		while (Vector3.Distance (pieces [o].transform.position, targetPos) > 0.02f) {
			pieces [o].transform.position = Vector3.Lerp (pieces [o].transform.position, targetPos, swapSpeed * timeElapsed);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		pieces [o].transform.position = targetPos;

	}

	IEnumerator ShiftHigher(int a, SortableObject t){
		Vector3 posA = pieces [a].transform.position + forkVector;
		Vector3 posB = t.transform.position - forkVector;
		float timeElapsed = swapSpeed/20;
		while (Vector3.Distance(pieces[a].transform.position,posB)>0.02f) {
			pieces [a].transform.position = Vector3.Lerp (pieces [a].transform.position, posB, swapSpeed * timeElapsed);
			t.transform.position = Vector3.Lerp (t.transform.position, posA, swapSpeed * timeElapsed);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		pieces [a].transform.position = posB;
		t.transform.position = posA;
		pieces [a + 1] = pieces [a];
	}

}
