using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour {

	private PlayerController1 thePlayer;
	public float damageAmount = 5f; // Damage amount (có thể điều chỉnh trong Inspector)

	// Use this for initialization
	void Start () {
		thePlayer = FindObjectOfType<PlayerController1> ();
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player"){
			if (thePlayer != null) {
				thePlayer.TakeDamage(damageAmount);
				Debug.Log($"Fish hit player! Damage: {damageAmount}");
			}
		}
	}
}
