using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linda;
using UniRx;

public class PhilosopherCoroutines : AbstractLinda {

	public int idPhilosopher;
	public int numPhilosophers;

	public Material thinkMaterial;
	public Material eatMaterial;
	public Material waitMaterial;


	// Use this for initialization
	void Start () {
		StartCoroutine (Think ());
		//DoThings ();
	}

	// Update is called once per frame
	void Update () {

	}

	void DoThings ()
	{
		while (true) {
			//print ("1");
			StartCoroutine (Think());
			//print ("2");
			StartCoroutine (GetChops ());
			//print ("3");
			StartCoroutine (Eat());
			//print ("4");
			ReleaseChops ();
		}

	}

	private IEnumerator Think ()
	{
		print (idPhilosopher + " : THINKING");
		GetComponent<Renderer> ().material.color = thinkMaterial.color;
		yield return new WaitForSeconds (Random.Range (5, 10));
		StartCoroutine (GetChops());
	}

	private IEnumerator GetChops ()
	{
		GetComponent<Renderer> ().material.color = waitMaterial.color;
		print (idPhilosopher + " : awaiting chop(" + idPhilosopher + ")");
		yield return (LindaLibrary.Linda_IN_SUSP_Coroutine (string.Format ("chop({0})", idPhilosopher), this));
		//StartCoroutine (LindaLibraryCoroutines.Linda_IN_SUSP (string.Format ("chop({0})", idPhilosopher), this));
		print (idPhilosopher + " : awaiting chop(" + (idPhilosopher+1)%(numPhilosophers) + ")");
		yield return (LindaLibrary.Linda_IN_SUSP_Coroutine (string.Format ("chop({0})", (idPhilosopher+1)%(numPhilosophers)),this));
		print (idPhilosopher+" DONE AWAITING");
		StartCoroutine (Eat());
	}

	private IEnumerator Eat ()
	{
		print (idPhilosopher + " : EATING");
		GetComponent<Renderer> ().material.color = eatMaterial.color;
		yield return new WaitForSeconds (Random.Range (5, 10));
		ReleaseChops ();
	}

	private void ReleaseChops ()
	{
		print (idPhilosopher + " : RELEASING chop(" + idPhilosopher + ")");
		LindaLibrary.Linda_OUT (string.Format ("chop({0})",idPhilosopher));
		print (idPhilosopher + " : RELEASING chop(" + (idPhilosopher+1)%(numPhilosophers) + ")");
		LindaLibrary.Linda_OUT (string.Format ("chop({0})", (idPhilosopher+1)%(numPhilosophers)));
		print (idPhilosopher + " : DONE OUT");
		StartCoroutine (Think ());
	}
}
