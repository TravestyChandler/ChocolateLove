using UnityEngine;
using System.Collections;

public class Baker : MonoBehaviour {
	public int direction = 1;
	public float speed = 1.0f;
	public GameObject player;
	public bool eaten = false;
	public float size  = 6f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(direction == 1){
			this.transform.Translate( Vector3.right * speed * Time.deltaTime);
			this.GetComponent<Animator>().Play (Animator.StringToHash( "WalkRight" ) );
		}
		else{
			this.transform.Translate( -Vector3.right * speed * Time.deltaTime);
			this.GetComponent<Animator>().Play (Animator.StringToHash( "WalkLeft" ) );
		}
	}

	public void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.tag == "Wall"){
			direction = -direction;
		}
	}
	
	public void Death(){
		if(player.GetComponent<Player>().size >= this.size){
			if(!eaten){
				eaten = true;
				player.audio.PlayOneShot(player.GetComponent<Player>().eatSound);
				Destroy(this.gameObject);
				player.GetComponent<Player>().GameWin();
			}
		}
		else{
			if(!player.GetComponent<Player>().invul){
				player.audio.PlayOneShot(player.GetComponent<Player>().hitSound);
				player.GetComponent<Player>().health --;
				player.GetComponent<Player>().invul = true;
			}
		}
	}
}
