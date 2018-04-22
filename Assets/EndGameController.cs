using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameController : MonoBehaviour
{

    public Image black;
    public Text quote;
    public string[] quotes = {
        "One fish\ntwo fish\nred fish\nblue fish\n\n-Dr. Seuss",
            "Never forget that only dead\nfish swim with the stream\n\n-Malcolm Muggeridge",
        "Many men go fishing all of their lives \nwithout knowing that it is \nnot fish they are after\n\n-Henry David Thoreau"
    };

	// Use this for initialization
	void Start () {
        quote.text = quotes[Random.Range(0, quotes.Length)];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void EndGame(){
        black.color = new Color(0, 0, 0, 1);
        StartCoroutine(Load());

    }
    IEnumerator Load(){
        yield return new WaitForSeconds(1);
        quote.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(9);
        SceneManager.LoadSceneAsync("Menu");
    }
}
