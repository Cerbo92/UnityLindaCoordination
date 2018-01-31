using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linda;
using System.Threading.Tasks;

public class Chair : AbstractLinda {

	private bool isAvailable;
	private int id;
	private int totalPhils;

	public Material availMaterial;
	public Material notAvailMaterial;

	public bool IsAvailable {
		get {
			return isAvailable;
		}
		set {
//			if (value) {
//				GetComponent<Renderer> ().material.color = availMaterial.color;
//			} else {
//				GetComponent<Renderer> ().material.color = notAvailMaterial.color;
//			}
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

	public async Task IWantToEat(SituatedPhilosopher phil){
		Id = phil.IdPhilosopher;
		totalPhils = phil.NumPhilosophers;
		print (Id + " : awaiting chop(" + Id + ")");
		await LindaLibrary.Linda_IN_SUSP (string.Format ("chop({0})", Id), this);
		print (Id + " : awaiting chop(" + (Id+1)%(totalPhils) + ")");
		await LindaLibrary.Linda_IN_SUSP (string.Format ("chop({0})", (Id+1)%(totalPhils)), this);
		print (Id+" DONE AWAITING");
	}

	public void DoneEating(){
		print (Id + " : RELEASING chop(" + Id + ")");
		LindaLibrary.Linda_OUT (string.Format ("chop({0})",Id));
		print (Id + " : RELEASING chop(" + (Id+1)%(totalPhils) + ")");
		LindaLibrary.Linda_OUT (string.Format ("chop({0})", (Id+1)%(totalPhils)));
		print (Id + " : DONE OUT");
		IsAvailable = true;
	}
}
