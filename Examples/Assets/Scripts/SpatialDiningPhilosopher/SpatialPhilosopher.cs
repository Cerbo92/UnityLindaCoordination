using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LindaCoordination;
using System.Threading.Tasks;
using UnityEngine.AI;

public class SpatialPhilosopher : AbstractLinda {

	public int idPhilosopher;
	public int numPhilosophers;

	public Material thinkMaterial;
	public Material eatMaterial;
	public Material waitMaterial;

	private SpatialChair chair;
	private NavMeshAgent agent;

	public int IdPhilosopher {
		get {
			return idPhilosopher;
		}
		set {
			idPhilosopher = value;
		}
	}

	public int NumPhilosophers {
		get {
			return numPhilosophers;
		}
	}

	// Use this for initialization
	void Start () {
		GameObject obj = GameObject.Find ("Chair" + IdPhilosopher);
		chair = obj.GetComponent<SpatialChair> ();
		agent = GetComponent<NavMeshAgent> ();
		agent.autoBraking = true;
		agent.SetDestination (obj.transform.localPosition);
		StartCoroutine (ReachingDestination());
	}

	// Update is called once per frame
	void Update () {
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

	private IEnumerator ReachingDestination(){
		print (name + "," + agent.destination);
		while (agent.remainingDistance == 0f || agent.remainingDistance > 3f) {
			yield return null;
		}
		agent.ResetPath ();
		DoThings ();
	}

	public async Task Think ()
	{
		print (idPhilosopher + " : THINKING");
		GetComponent<Renderer> ().material.color = thinkMaterial.color;
		await Task.Delay (Random.Range (3,10)*1000);
	}

	async Task GetChops ()
	{
		GetComponent<Renderer> ().material.color = waitMaterial.color;
		if (chair.IsAvailable) {
			chair.IsAvailable = false;
		}
		await chair.IWantToEat (this);
	}

	public async Task Eat ()
	{
		print (idPhilosopher + " : EATING");
		GetComponent<Renderer> ().material.color = eatMaterial.color;
		await Task.Delay (Random.Range (5,10)*1000);
	}

	private void ReleaseChops ()
	{
		chair.DoneEating ();
	}
}
