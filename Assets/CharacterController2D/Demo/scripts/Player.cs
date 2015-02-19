using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
	public float eatingTimer = 0.0f;
	public float size = 1f;
	public int health = 3;
	public List<Image> hearts;
	public Sprite fullHeart;
	public Sprite emptyHeart;
	public bool invul = false;
	public float invulTimer = 0f;
	public float invulLength = 1f;
	public GameObject gameOver;
	public GameObject victory;
	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	public AudioClip eatSound;
	public AudioClip hitSound;
	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;

	public ParticleSystem part;
	public bool eating = false;


	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
		gameOver.gameObject.SetActive(false);
		part.renderer.sortingLayerName = "Player";
		victory.gameObject.SetActive(false);
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		if(hit.collider.tag =="Pickup"){
			eating = true;

			hit.collider.gameObject.SendMessage("Death");

		}
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent( Collider2D col )
	{	
		if(col.tag =="Pickup"){
			eating = true;
			col.gameObject.SendMessage("Death");
		}
		if(col.tag == "Baker"){
			col.gameObject.SendMessage("Death");
		}
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{

		if(invul){
			if(invulTimer > invulLength){
				invul = false;
				_animator.Play( Animator.StringToHash( "ChocolateRightWalk" ) );
				invulTimer = 0.0f;
			}
			else{
				invulTimer += Time.deltaTime;
			}
		}
		setHealth();
		if(eating){
			if(eatingTimer > 0.66f){
				eating = false;
				_animator.Play( Animator.StringToHash( "ChocolateRightWalk" ) );
				eatingTimer = 0.0f;
			}
			else{
				eatingTimer += Time.deltaTime;
			}
		}
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

		if( _controller.isGrounded )
			_velocity.y = 0;

		if( Input.GetKey( KeyCode.RightArrow ) )
		{
			normalizedHorizontalSpeed = 1;
			part.emissionRate = 6;
			this.transform.localScale = new Vector3(Mathf.Abs (this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
			part.transform.eulerAngles = new Vector3(180f, 90f, 0);
			if(eating){
				_animator.Play( Animator.StringToHash( "ChocolateRightEating" ) );
			}
			else{
				if( _controller.isGrounded ){
					_animator.Play( Animator.StringToHash( "ChocolateRightWalk" ) );
				}
			}
		}
		else if( Input.GetKey( KeyCode.LeftArrow ) )
		{
			normalizedHorizontalSpeed = -1;
			part.emissionRate = 6;
			this.transform.localScale = new Vector3(-Mathf.Abs (this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
			part.transform.eulerAngles = new Vector3(0f, 90f, 0);
			if(eating){
				_animator.Play( Animator.StringToHash( "ChocolateRightEating" ) );
			}
			else{
				if( _controller.isGrounded ){
					_animator.Play( Animator.StringToHash( "ChocolateRightWalk" ) );
				}
			}
		}
		else
		{
			normalizedHorizontalSpeed = 0;
			part.emissionRate = 0;
//			if( _controller.isGrounded )
//				_animator.Play( Animator.StringToHash( "Idle" ) );
		}


		// we can only jump whilst grounded
		if( _controller.isGrounded && Input.GetKeyDown( KeyCode.UpArrow ) )
		{
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
		//	_animator.Play( Animator.StringToHash( "Jump" ) );
		}


		// apply horizontal speed smoothing it
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		_controller.move( _velocity * Time.deltaTime );
	}

	public void setHealth(){
		if(health == 3){
			foreach(Image i in hearts){
				i.sprite = fullHeart;
			}
		}
		else if(health == 2){
			hearts[2].sprite = emptyHeart;
			hearts[1].sprite = fullHeart;
			hearts[0].sprite = fullHeart;
		}
		else if(health == 0){
			hearts[2].sprite = emptyHeart;
			hearts[1].sprite = emptyHeart;
			hearts[0].sprite = fullHeart;
		}
		else if(health <= 0){
			hearts[2].sprite = emptyHeart;
			hearts[1].sprite = emptyHeart;
			hearts[0].sprite = emptyHeart;
			playerDeath ();
		}
	}

	public void playerDeath(){
		gameOver.SetActive(true);
		Time.timeScale = 0.0f;
	}

	public void GameWin(){
		victory.SetActive(true);
		Time.timeScale = 0.0f;
	}
}
