using UnityEngine;
using System.Collections;

public class ColliderSize : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		transform.GetComponent<BoxCollider> ().size = new Vector3 (Screen.height / 5.3f, Screen.height / 5.3f, 1);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
