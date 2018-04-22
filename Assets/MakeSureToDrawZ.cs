using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSureToDrawZ : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<MeshRenderer> ().material.SetInt ("_ZWrite", 1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
