using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prolog;
using Linda;
using System.Threading.Tasks;

public class Philosopher : AbstractLinda {

	public int idPhilosopher;
	public int numPhilosophers;

	public Material thinkMaterial;
	public Material eatMaterial;
	public Material waitMaterial;


	// Use this for initialization
	void Start () {
		DoThings ();
		//qui ci vado lo stesso, anche se sono async, altrimenti si inchioda tutto e diventa estremamente lenta la simulazione
	}
	
	// Update is called once per frame
	void Update () {
		//DoThings ();
		//l'update è chiamato lo stesso!

	}

	async void DoThings ()
	{
		while (true) {
			//print ("1");
			await Think ();
			//print ("2");
			await GetChops ();
			//print ("3");
			await Eat ();
			//print ("4");
			ReleaseChops ();
		}

	}

	async Task Think ()
	{
		print (idPhilosopher + " : THINKING");
		GetComponent<Renderer> ().material.color = thinkMaterial.color;
		await Task.Delay (Random.Range (5,10)*1000);
	}

	async Task GetChops ()
	{
		GetComponent<Renderer> ().material.color = waitMaterial.color;
		print (idPhilosopher + " : awaiting chop(" + idPhilosopher + ")");
		await LindaLibrary.Linda_IN_SUSP (string.Format ("chop({0})", idPhilosopher), this);
		print (idPhilosopher + " : awaiting chop(" + (idPhilosopher+1)%(numPhilosophers) + ")");
		await LindaLibrary.Linda_IN_SUSP (string.Format ("chop({0})", (idPhilosopher+1)%(numPhilosophers)), this);
		print (idPhilosopher+" DONE AWAITING");
	}

	async Task Eat ()
	{
		print (idPhilosopher + " : EATING");
		GetComponent<Renderer> ().material.color = eatMaterial.color;
		await Task.Delay (Random.Range (5,10)*1000);
	}

	private void ReleaseChops ()
	{
		print (idPhilosopher + " : RELEASING chop(" + idPhilosopher + ")");
		LindaLibrary.Linda_OUT (string.Format ("chop({0})",idPhilosopher));
		print (idPhilosopher + " : RELEASING chop(" + (idPhilosopher+1)%(numPhilosophers) + ")");
		LindaLibrary.Linda_OUT (string.Format ("chop({0})", (idPhilosopher+1)%(numPhilosophers)));
		print (idPhilosopher + " : DONE OUT");
	}
		
}
