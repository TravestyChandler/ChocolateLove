using UnityEngine;
using System.Collections;


public class EnemyScripts : MonoBehaviour {
	public bool fleeing = false;
	public float speed = 1.0f;
	public int direction = 1;
	public GameObject player;
	public float size = 1f;
	public bool eaten = false;
	// Use this for initialization
	void Start () {
		player = GameObject.Find("Character");
	}
	
	// Update is called once per frame
	void Update () {
		if(fleeing){
			if(direction == 1){
				this.transform.Translate( Vector3.right * speed * Time.deltaTime);
				this.GetComponent<Animator>().Play (Animator.StringToHash( "RunningRight" ) );
			}
			else{
				this.transform.Translate( -Vector3.right * speed * Time.deltaTime);
				this.GetComponent<Animator>().Play (Animator.StringToHash( "RunningLeft" ) );
			}
		}
	}

	public void Death(){
		if(player.GetComponent<Player>().size >= this.size){
			if(!eaten){
				eaten = true;
				player.audio.PlayOneShot(player.GetComponent<Player>().eatSound);
				player.GetComponent<Player>().size += 0.5f;
				if(player.transform.localScale.x > 0){
					player.transform.localScale = new Vector3(1f + (player.GetComponent<Player>().size * 0.25f), 1f + (player.GetComponent<Player>().size * 0.25f), 1f);
				}
				else{
					player.transform.localScale = new Vector3(-1f - (player.GetComponent<Player>().size * 0.25f), 1f + (player.GetComponent<Player>().size * 0.25f), 1f);
				}
				Destroy(this.gameObject);
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

	public void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.tag == "Wall"){
			direction = -direction;
		}
	}
}
