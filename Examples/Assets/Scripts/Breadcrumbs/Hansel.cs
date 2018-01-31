using LindaCoordination;
using UnityEngine;
using System.Collections;

public class Hansel : AbstractLinda {

	public float deltaTime;
	private int counter = 0;
	private Coroutine coroutine;
	private bool doing;

	// Use this for initialization
	void Start () {
		coroutine = StartCoroutine (DoThingsPeriodically ());
		doing = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			print ("SPACE, CREATING REGION HERE...");
			Region reg = new Region ("Caution", transform.position, new Vector3 (20f, 20f, 20f), PrimitiveType.Cube,true, Quaternion.identity);
			LindaCoordinationUtilities.SendMessageToRegion ("stop("+counter+")",reg);
		}
		if (doing && Input.GetKeyDown (KeyCode.P)) {
			print ("P, STOPPING COROUTINE");
			doing = false;
			StopCoroutine (coroutine);
		}
		if (!doing && Input.GetKeyDown (KeyCode.LeftAlt)) {
			print ("ALT, STARTING COROUTINE");
			doing = true;
			LindaCoordinationUtilities.SendMessageToAllRegionsName ("allGood","Caution");
			coroutine = StartCoroutine (DoThingsPeriodically ());
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			print ("C, SENDING MESSAGE TO REGION");
			LindaCoordinationUtilities.SendMessageToAllRegionsName ("allGood","Caution");
			print("TELLING HELLOWORLD ON BAGOFTUPLES "+LindaCoordinationUtilities.TellToObjectBag ("hello(world)",gameObject));
			GameObject bot = LindaCoordinationUtilities.IsObjectWithBagOfTuples (gameObject);
			Vector3 vertx = bot.GetComponent<Mesh> ().vertices [Random.Range (0, bot.GetComponent<Mesh> ().vertexCount)];
			Vector3 instancePos = transform.TransformPoint (vertx);
			bot.transform.position = vertx;
		}

	}

	private IEnumerator DoThingsPeriodically ()
	{
		while (true) {
			print ("WAITING...");
			yield return new WaitForSeconds (deltaTime);
			print ("DONE WAITING, MAKING BREADCRUMB... ");
			LindaCoordinationUtilities.SendMessageToLocation ("iWasHere("+this.name.ToLower ()+","+counter+")",
				new SpatialTupleSpace("Location"+counter,transform.localPosition,false,Vector3.one,false,new Vector3 (3,3,3)));
			counter++;
		}
	}
}
