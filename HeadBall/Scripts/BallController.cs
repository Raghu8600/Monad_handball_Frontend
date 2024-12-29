using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

	/// <summary>
	/// This is the main ball controller. It handles all ball status including force management,
	/// movements, collisions, sounds, visual effects, etc.
	/// </summary>

	private GameObject gc;						//Reference to GameController Object
	private float ballMaxSpeed = 10.0f;			//Ball should not move faster than this value

	public AudioClip ballHitGround;				//Audio when ball hits floor
	public AudioClip[] ballHitHead;				//Audio when ball hits the heads

	public Vector3 ballStartingPosition;		//initial position of the ball

	public GameObject ballSpeedDebug;			//debug object to show ball's speed at all times
	public GameObject hitEffect;				//visual effect for contact points
	public GameObject ballShadow;				//shadow object that follows the ball



	void Awake () {
		//find and cache references to important gameobjects
		gc			 = GameObject.FindGameObjectWithTag("GameController");
	}


	void Start () {
		//set ball starting position
		transform.position = ballStartingPosition;
	}
	

	void FixedUpdate () {

		//never allow the ball to get stuck at the two ends of the screen.
		escapeLimits();

		//move ball's shadow object
		manageBallShadow();

		//debug - show ball's speed
		if(ballSpeedDebug)
			ballSpeedDebug.GetComponent<TextMesh>().text = "Speed: " + GetComponent<Rigidbody>().linearVelocity.magnitude.ToString();

		//limit ball's maximum spped
		if(GetComponent<Rigidbody>().linearVelocity.magnitude > ballMaxSpeed)
			GetComponent<Rigidbody>().linearVelocity = GetComponent<Rigidbody>().linearVelocity.normalized * ballMaxSpeed;
	}


	/// <summary>
	/// Make shadow object follow ball's movements
	/// </summary>
	void manageBallShadow() {
		if(!ballShadow)
			return;

		ballShadow.transform.position = new Vector3(transform.position.x, -1.8f, 0.1f);
		ballShadow.transform.localScale = new Vector3(1.5f, 0.75f, 0.001f);
	}


	/// <summary>
	/// never allow the ball to get stuck at the two ends of the screen.
	/// we do this by applying a small force to move the ball in opposite direction.
	/// Screen boundaries are hardcoded and must be updated to match your own design.
	/// </summary>
	void escapeLimits() {
		if(transform.position.x <= -2.5f)
			GetComponent<Rigidbody>().AddForce(new Vector3(3, 0, 0), ForceMode.Impulse);
		if(transform.position.x >= 2.5f)
			GetComponent<Rigidbody>().AddForce(new Vector3(-3, 0, 0), ForceMode.Impulse);
	}


	/// <summary>
	/// Manages collition events
	/// </summary>
	void OnCollisionEnter (Collision other) {
		
		if(other.gameObject.tag == "Field") {
			playSfx(ballHitGround);
			createHitGfx();
			checkGoal();
		}

		//no more collision checking with player
		if (GameController.gameIsFinished)
			return;

		if(other.gameObject.tag == "PlayerHead") {
			playSfx(ballHitHead[Random.Range(0, ballHitHead.Length)]);
			StartCoroutine (other.gameObject.GetComponent<PlayerController>().changeFaceStatus());
			GameController.playerScore++;
			createHitGfx();
		}
	}


	/// <summary>
	/// Creates a small visual object to show the contact point between ball and other objects
	/// </summary>
	void createHitGfx() {
		GameObject hitGfx = Instantiate(hitEffect, 
					                    transform.position + new Vector3(0, -0.4f, -1), 
					                    Quaternion.Euler(0, 180, 0)) as GameObject;
		hitGfx.name = "hitGfx";
	}


	/// <summary>
	/// Check if ball falls on ground.
	/// </summary>
	void checkGoal() {

		if(GameController.gameIsFinished)
			return;

		StartCoroutine(gc.GetComponent<GameController>().manageFallEvent());
	}


	/// <summary>
	/// Move the ball to the starting position after a goal happened
	/// </summary>
	public IEnumerator resetBallPosition(int _posIndex) {

		//give the ball to the player or the cpu if they have received a goal
		transform.position = ballStartingPosition;


		//freeze the ball for a while
		GetComponent<Rigidbody>().Sleep();
		GetComponent<Rigidbody>().isKinematic = true;

		yield return new WaitForSeconds(1.0f);

		//unfreeze the ball
		//GetComponent<Rigidbody>().isKinematic = false;
	}


	//*****************************************************************************
	// Play sound clips
	//*****************************************************************************
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}
}