using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour {

    public float health;
    public float maxVelocity;
    public float acceleration;
    public int money;
    public Text displayMoney;
    private Rigidbody rb;
    private Camera cam;
    public GameObject treasure;
    public GameObject rod;
    public bool canCast = true;
    public Animator cast;
    private float probability = 0.5f;
	// Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
		audioSource = gameObject.GetComponent<AudioSource> ();
        Cursor.lockState = CursorLockMode.Locked;
		treasureSource = gameObject.AddComponent<AudioSource> ();
		treasureSource.clip = treasureChestSound;
        displayMoney.text = "$"+0;

		gameLogic = GetComponent<GameLogic> ();
	}
    public float speedH = 2.0f;
    public float speedV = 2.0f;


	public AudioClip step1;
	public AudioClip step2;
	public AudioClip treasureChestSound;
	public AudioSource audioSource; // Don't set these please
	public AudioSource treasureSource;

	private GameLogic gameLogic;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    public float speed = 10.0f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    private bool grounded = false;

	private float timeToAudioStep = 0.0f;
    IEnumerator IncrementMoney(int inc){
        int m = money;
        while(money<inc+m){
            money += Random.Range(4,7) +2;
            yield return new WaitForSecondsRealtime(1f/inc);
        }
    }

    void FixedUpdate()
    {
		if (grounded) {
			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			probability += targetVelocity.magnitude * 0.001f;
			if (probability > 1f)
				probability = 1f;
			if (probability < 0f)
				probability = 0f;
			targetVelocity = transform.TransformDirection (targetVelocity);
			targetVelocity *= speed;

			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = rb.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			rb.AddForce (velocityChange, ForceMode.VelocityChange);

			// Are we moving?
			if (targetVelocity.magnitude > 0.0000001f) {
				timeToAudioStep -= Time.deltaTime;
				if (timeToAudioStep <= 0.0f) {
					PlayStep ();
					timeToAudioStep = 0.65f;
				}
			} else {
				timeToAudioStep = 0.0f;
			}

			// Jump
			if (canJump && Input.GetButton ("Jump")) {
				rb.velocity = new Vector3 (velocity.x, CalculateJumpVerticalSpeed (), velocity.z);
				PlayStep ();
				timeToAudioStep = 0.0f;
			}
		} else {
			// In the air
			timeToAudioStep = 0.0f;
		}

        // We apply gravity manually for more tuning control
        rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));

        grounded = false;
    }

	void PlayStep(){
		if (gameLogic.canPlaySounds) {
			audioSource.pitch = Random.value * 0.2f + 0.9f;
			audioSource.PlayOneShot (Random.value > 0.5f ? step1 : step2, volumeScale: 0.25f * 0.8f);
		}
	}

    void OnCollisionStay()
    {
        grounded = true;
    }
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Safe") {
			canCast = false;
			// End the game
			gameLogic.BeginEnding();
		}
	}

	float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
	void Update()
    {
        displayMoney.text = "$" + money;
        //Debug.Log("Probability of pulling a chest is :" + probability);
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        if (pitch > 80)
            pitch = 80;
        if (pitch < -60)
            pitch = -60;
        cam.transform.localEulerAngles = new Vector3(pitch, 0, 0.0f);

        this.transform.eulerAngles = new Vector3(0, yaw, 0.0f);

        if(Input.GetButtonDown("Fire1") && canCast){
            cast.SetTrigger("Cast");
            probability -= 0.05f;
            Cast();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;


    }
    public void Cast(){
        if (Random.Range(0f, 1f) < probability)
        {
            probability -= 0.45f;
            StartCoroutine(IncrementMoney(Random.Range(700, 1500)));
            Instantiate(treasure, 4 * cam.transform.forward + 2.5f * Vector3.down + this.transform.position, Quaternion.identity);
			treasureSource.PlayDelayed (1.0f);

        }
		gameLogic.Cast ();
        }
    public void ResetMoney(){
        StopAllCoroutines();
        money = 0;
    }
}
