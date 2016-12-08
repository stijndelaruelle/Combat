using UnityEngine;
using System.Collections;

public class punchRadius : MonoBehaviour {

	// Use this for initialization

	private const float mag = 2.54f;
	private float lastOffset;
	private DynamicLight2D.DynamicLight dl;

	void Start () {
				dl = GetComponent<DynamicLight2D.DynamicLight>();
		lastOffset = dl.LightRadius;
		StartCoroutine(updateLoop());

	}

	IEnumerator updateLoop(){

		while(true){
			yield return new WaitForSeconds(.081f);
			float rnd = Random.Range(-1f,1f) * mag;
			yield return null;
			dl.LightRadius = lastOffset + rnd;
			yield return new WaitForEndOfFrame();
		}
	}

}
