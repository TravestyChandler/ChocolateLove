using UnityEngine;
using System.Collections;

public class Couple : MonoBehaviour {
	public GameObject guy, girl;
	public float speed = 0.01f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(guy == null){
			if(girl == null){
				Destroy (this.gameObject);
			}
			else{
				girl.GetComponent<EnemyScripts>().fleeing = true;
			 }
		}
		if(girl == null){
			if(guy == null){
				Destroy (this.gameObject);
			}
			else{
				guy.GetComponent<EnemyScripts>().fleeing = true;
			}
		}
	}
}
