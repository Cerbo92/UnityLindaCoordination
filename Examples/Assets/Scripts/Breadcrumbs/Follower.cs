using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using LindaCoordination;
using System;

public class Follower : AbstractLinda {

	private NavMeshAgent agent;
	//private int currentDestPoint;
	//private Transform[] points;
	private int counter;
	private Collider destroyable;
	private bool once = true;

	// Use this for initialization
	void Start () {
		agent = gameObject.AddComponent<NavMeshAgent> ();
		agent.autoBraking = true;
		//GoToNextPoint();
	}
	
	// Update is called once per frame
	void Update () {
		//print (agent.remainingDistance);
//		if (!agent.pathPending && agent.remainingDistance < 0.8f && agent.remainingDistance > 0f) {
//			print ("DESTROYING");
//			Destroy (destroyable.gameObject);
//		}
	}

//	void GoToNextPoint ()
//	{
//		if (points.Length == 0) {
//			return;
//		}
//		agent.destination = points [currentDestPoint].position;
//		currentDestPoint = (currentDestPoint + 1) % points.Length;
//	}

	async void OnTriggerEnter(Collider coll) {
		if (base.enabledOrSuspensionCheck (coll)) {
			return;
		} 
		print("FOLLOWER TRIGGEREDDDD, CHECKING... " + coll);
		if (coll.CompareTag ("Region") && coll.name.Equals ("Caution")) {
			print ("REGION: SHOULD I STOP?");
			if (LindaCoordinationUtilities.RetrieveMessageFromSituatedObjectTriggered ("stop(_)",coll).Equals (ReturnTypeKB.True)) {
				LindaCoordinationUtilities.AttachBagOfTuplesToObject ("LindaAgent");
				print ("STOPPING AND AWAITING A GOOD MESSAGE");
				NavMeshPath navpath = agent.path;
				agent.ResetPath ();
				await LindaCoordinationUtilities.RetrieveMessageSuspendedFromSituatedObjectTriggered ("allGood", coll, this);
				print ("DONE AWAITING GOOD MESSAGE");
				agent.SetPath (navpath);
			} else {
				print ("SHOULD I STOP? NOPE");
			}

		}
		if (LindaCoordinationUtilities.AskMessageFromSituatedObjectTriggered ("iWasHere(lindaagent,_)",coll).Equals (ReturnTypeKB.True)) {
			print("FOLLOWER TRIGGEREDDDD, TRYING TO FOLLOW lindaagent ");
			bool res = LindaCoordinationUtilities.AskToObjectBag ("hello(world)", "LindaAgent");
			Debug.LogWarning ("HELLOWORLD ASK TO BAG " + res);
			if (res && once) {
				print("AQUIRING BAG: " + LindaCoordinationUtilities.AcquireBagOfTuples (gameObject, "LindaAgent"));
				print ("MUST BE TRUE: " + LindaCoordinationUtilities.AskToObjectBag ("hello(world)", gameObject));
				once = false;
			}

			var tupleCounter = Convert.ToInt32 (LindaCoordinationUtilities.AskParam ("X", "iWasHere(lindaagent,X)", coll.gameObject));
//			if (!first) {
//				Destroy (destroyable.gameObject);
//			}
//			first = false;
			destroyable = coll;
			Destroy (destroyable.gameObject,1.7f);
			//int tupleCounter = Int32.Parse (LindaCoordinationUtilities.AskParam ("X", "iWasHere(lindaagent,X)", coll.gameObject));
			if (tupleCounter > counter) {
				agent.destination = coll.transform.position;
				counter = tupleCounter;
			}
		}
	}
}
