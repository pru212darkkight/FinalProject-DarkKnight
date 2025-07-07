using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWater : MonoBehaviour {

	private PlayerController thePlayer;
	public GameObject death;

	public float speed = 0.3f;

	private float turnTimer;
	public float timeTrigger;

	private Rigidbody2D myRigidbody;



 

	// Use this for initialization
	void Start () {
		thePlayer = FindObjectOfType<PlayerController> ();	
		myRigidbody = GetComponent<Rigidbody2D> ();

		turnTimer = 0;
		timeTrigger = 3f;
		 
	}

	// Update is called once per frame
	void Update (){
		myRigidbody.linearVelocity = new Vector3 (myRigidbody.transform.localScale.x * speed, myRigidbody.linearVelocity.y, 0f);

		turnTimer += Time.deltaTime;
		if(turnTimer >= timeTrigger){
			turnAround ();
			turnTimer = 0;
		}



	}


	void OnTriggerEnter2D(Collider2D other){

		if(other.tag == "Player" && thePlayer.rushing){
			Instantiate (death, gameObject.transform.position, gameObject.transform.rotation);
			Destroy (gameObject);
		}

	}

	void turnAround(){
		// Giữ nguyên scale hiện tại, chỉ đổi hướng
		Vector3 currentScale = transform.localScale;
		if (currentScale.x > 0) {
			transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
		} else {
			transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
		}
	}
}
