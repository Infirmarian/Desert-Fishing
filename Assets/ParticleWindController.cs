using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleWindController : MonoBehaviour {

	public ParticleSystem childParticleSystem;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// fix the rotation of the box; keep the translation (and scaling)
		childParticleSystem.transform.rotation = Quaternion.identity;
	}
}
