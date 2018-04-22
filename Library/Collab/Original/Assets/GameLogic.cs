using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {
	public PostProcessingProfile profile;
	public Light directionalSun;

	public Image titleImage;
	public Text numberText;

	// Save initial game state so that we can reset it
	private Vector3 initialPlayerPosition;
	private Quaternion initialPlayerRotation;
	private Vector3 initialPlayerScale; // this should not change

	private float startTime; // start time of this scene

	private int iter = 0; // Which runthrough are they on?

	const float len = 120.0f; // total length of a level (for now)
	const float costPerMeter = 0.3f;
	const float costPerSecond = 1.0f*0.0f;
	const float costPerCast = 13.5f;

	const float repPerDrink = 15.0f;

	public float totalCost =  0.0f;
	private Vector3 prevPosition;

	private float timeToEnd = -1.0f; // Once this is set to a positive value,
	// counts the amount of time until the game ends.
	private const float fadeOutLength = 5.0f;

	public bool canPlaySounds = true;

	private PlayerController playerController;

	// Use this for initialization
	void Start () {
		initialPlayerPosition = transform.localPosition;
		initialPlayerRotation = transform.localRotation;
		initialPlayerScale = transform.localScale;

		startTime = Time.time;
		totalCost = 0.0f;
		prevPosition = transform.position;

		playerController = GetComponent<PlayerController> ();
	}

	void ResetPlaythrough(){
		// Reset position
		transform.localPosition = initialPlayerPosition;
		transform.localRotation = initialPlayerRotation;
		transform.localScale = initialPlayerScale;
		startTime = Time.time;
		totalCost = 0.0f;
		prevPosition = transform.position;
	}

	public void Cast(){
		if (playerController.canCast) { // (this line is extraneous)
			totalCost += costPerCast;
		}
	}

	public void BeginEnding(){
		timeToEnd = fadeOutLength;
	}
	
	// Update is called once per frame
	void Update () {
		// How dead is the player?
		float t = Time.time - startTime; // Time in seconds since start

		// Compute cost
		if (playerController.canCast) { // not in the oasis
			totalCost += Time.deltaTime * costPerSecond;
			totalCost += costPerMeter * (transform.position - prevPosition).magnitude;
			prevPosition = transform.position;
		}

		// POST-PROCESSING EFFECTS
		// Set post-processing effects

		const float blackoutTime = 2.0f;

		float deadness = totalCost / len;
		//float deadness = Mathf.Max(0.0f, t-blackoutTime) / len;

		// Speed up deadness towards the end
		// deadness *= deadness; // actually don't

		// ramp up exposure a lot in the last second
		float dt = Mathf.Max (0.0f, (deadness - (len - 3.0f) / len) * len/3.0f);
		float t2 = deadness + 4 * dt * dt * dt * dt;

		// Color grading cues
		VignetteModel.Settings vm = profile.vignette.settings;
		vm.intensity = 0.297f * (deadness-dt*dt); //deadness;
		profile.vignette.settings = vm;

		GrainModel.Settings gs = profile.grain.settings;
		gs.intensity = 1 * deadness;
		profile.grain.settings = gs;

		ColorGradingModel.Settings cgs = profile.colorGrading.settings;

		// On the first iteration, fade in; on subsequent ones, blackout the first two seconds.
		if (t < blackoutTime) {
			if(iter==0){
				cgs.basic.postExposure = 0.0f + Mathf.Log(Mathf.Max(0.0f, t/2.0f))/Mathf.Log(2.0f);
			}else{
				cgs.basic.postExposure = -200.0f;
			}
			canPlaySounds = false;
		} else {
			// Normal behavior
			cgs.basic.postExposure = 0.0f + 1.19f * t2;
			canPlaySounds = true;
		}
		cgs.basic.temperature = 4.98f + 46.0f * deadness;
		profile.colorGrading.settings = cgs;

		directionalSun.intensity = 1.0f + 64.0f * dt * dt * dt * dt;///*1.0f +*/ Mathf.Pow(2.0f, 8.0f*dt * dt * dt);
		// END POST-PROCESSING EFFECTS

		// Title text
		if (iter == 0) {
			// disappear from seconds 2 to 4
			titleImage.color = new Color (1.0f, 1.0f, 1.0f, Mathf.Clamp (1.0f - (t - 2.0f) / 2.0f, 0.0f, 1.0f));
			if (t > 4.0f) {
				titleImage.enabled = false;
			} else {
				titleImage.enabled = true;
			}
		} else {
			titleImage.enabled = false;
		}


		// Number text
		numberText.enabled = ((iter>0) && (t<2.0f));
		if (iter > 0) {
			numberText.text = iter.ToString ();
		}

		// If we're ending, don't allow restarts
		if (timeToEnd < 0.0f) {
			// Are we at the end of this runthrough?
			if (deadness > 1.0f) {
				iter++;
				PlayerController pc = GetComponent<PlayerController> ();
				pc.audioSource.Stop ();
				pc.treasureSource.Stop ();
				ResetPlaythrough ();
			}
		} else {
			// Ending
			// What should the level of the visuals be?
			float et = timeToEnd/fadeOutLength;
			et = -2*et*et*et+3*et*et; // cubic Hermite spline
			cgs = profile.colorGrading.settings;
			cgs.basic.postExposure = Mathf.Lerp(cgs.basic.postExposure, Mathf.Log(t)/Mathf.Log(2.0f), Time.deltaTime);
			profile.colorGrading.settings = cgs;

			timeToEnd -= Time.deltaTime;

			Debug.Log (timeToEnd);

			if (timeToEnd > fadeOutLength) {
				EndGameController egc = GetComponent<EndGameController> ();
				egc.EndGame();
			}		
		}


	}
}
