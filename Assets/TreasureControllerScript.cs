using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureControllerScript : MonoBehaviour {
    private Animator rise;
	// Use this for initialization
	void Start () {
       // rise = GetComponent<Animator>();
        StartCoroutine(CountDown());
	}
	
	// Update is called once per frame
	void Update () {
        //rise.SetTrigger("GotTreasure");
	}
    IEnumerator CountDown(){
        yield return new WaitForSeconds(23f);
        Destroy(this.gameObject);
    }
}
