using UnityEngine;
using System.Collections;

public class RandomWander : MonoBehaviour
{

	//public AudioClip AlienScream;
	public float minSpeed;
	// minimum range of speed to move
	public float maxSpeed;
	// maximum range of speed to move
	float speed;
	// speed is a constantly changing value from the random range of minSpeed and maxSpeed

	public string[] collisionTags;
	//  What are the GO tags that will act as colliders that trigger a
	//  direction change? Tags like for walls, room objects, etc.
	public AudioClip collisionSound;

	float step = Mathf.PI / 60;
	float timeVar = 0;
	float rotationRange = 360;
	//  How far should the object rotate to find a new direction?
	float baseDirection = 0;

	float counter = 0;

	Vector3 randomDirection;
	// Random, constantly changing direction from a narrow range for natural motion



	void Update ()
	{

		if (counter++ > 250) {
			print ("CHANGE " + counter);
			baseDirection = baseDirection + Random.Range (-50, 50);
			counter = 0;
		}
		randomDirection = new Vector3 (0, Mathf.Sin (timeVar) * (rotationRange / 2) + baseDirection, 0); //   Moving at random angles 
		timeVar += step;
		speed = Random.Range (minSpeed, maxSpeed);              //      Change this range of numbers to change speed
		GetComponent<Rigidbody> ().AddForce (transform.forward * speed);
		transform.Rotate (randomDirection * Time.deltaTime * 80.0f);        
	}

	void OnCollisionEnter (Collision col)
	{

		if (col.gameObject.tag == collisionTags [0]) {                   //  Tag it with a wall or other object
			print("COLLISION");
			baseDirection = baseDirection + Random.Range (-30, 30);   // Switch to a new direction on collision

			// use the above code as a template for all the collisionTags
			// add here.. and on.. and on..
		}
	}
}
