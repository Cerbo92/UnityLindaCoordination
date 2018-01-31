using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using LindaCoordination;
using System;
using System.Threading.Tasks;

public class SpatialChair : AbstractLinda {

	private bool isAvailable;
	private int id;
	private int totalPhils;
	private GameObject[] objs;
	private GameObject table;

	private int chop1, chop2;
	private GameObject chop1go, chop2go;

	public Material availMaterial;
	public Material notAvailMaterial;

	public bool IsAvailable {
		get {
			return isAvailable;
		}
		set {
			isAvailable = value;
		}
	}

	public int Id {
		get {
			return id;
		}
		set {
			id = value;
		}
	}

	void Start(){
		table = GameObject.Find ("Table");
		CapsuleCollider c = gameObject.GetComponent<CapsuleCollider> ();
		objs = LindaCoordinationUtilities.GetSituatedObjectsFromArea (transform.localPosition, c.bounds.extents.x, 10);
		//print (name);
		//print (objs.Length + " , " + objs[0]);
		int count = 0;
		for (int i = 0; i < objs.Length; i++) {
			if (count == 2) {
				break;
			}
			if (objs[i].name.Contains("Chop")) {
				string thename = objs [i].name;
				//print ("CHOPPP" + thename);
				if (count == 0) {
					chop1 =  Convert.ToInt32(thename.Substring (thename.Length - 1));
					chop1go = objs [i];
					count++;
					continue;
				}
				if (count==1) {
					chop2 = Convert.ToInt32(thename.Substring (thename.Length - 1));
					chop2go = objs [i];
					count++;
				}

			}
		}
	}

	public async Task IWantToEat(SpatialPhilosopher phil){
		print (name + " : awaiting chop(" + chop1 + ")");
		await LindaCoordinationUtilities.RetrieveSuspend (string.Format ("chop({0})", chop1), table, this);
		chop1go.GetComponent<Renderer> ().material = Resources.Load ("Materials/Red") as Material;
		print (name + " : awaiting chop(" + chop2 + ")");
		await LindaCoordinationUtilities.RetrieveSuspend (string.Format ("chop({0})", chop2), table, this);
		chop2go.GetComponent<Renderer> ().material = Resources.Load ("Materials/Red") as Material;
		print (name + " DONE AWAITING");
	}

	public void DoneEating(){
		print (name + " : RELEASING chop(" + chop1 + ")");
		chop1go.GetComponent<Renderer> ().material = Resources.Load ("Materials/Green") as Material;
		LindaCoordinationUtilities.Tell (string.Format ("chop({0})", chop1), table);

		print (name + " : RELEASING chop(" + chop2 + ")");
		chop2go.GetComponent<Renderer> ().material = Resources.Load ("Materials/Green") as Material;
		LindaCoordinationUtilities.Tell (string.Format ("chop({0})", chop2), table);

		IsAvailable = true;
	}
}
