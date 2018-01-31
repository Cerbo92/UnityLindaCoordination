using LindaCoordination;
using UnityEngine;
using Prolog;
using Linda;
using System;
using System.Threading.Tasks;

//Meant to be utilized with CommLibTest2
public class CommunicationLibraryTest : AbstractLinda
{
	bool once;
	void Start(){

		SpatialTupleSpace ts = new SpatialTupleSpace ("Ciccio", new Vector3(8f,4f,3f), false, true, Vector3.one, true);
		print("SENDMESSAGETOLOCATION: " + LindaCoordinationUtilities.SendMessageToLocation ("ciao",ts));
		print("ASK: " + LindaCoordinationUtilities.Ask ("ciao", "Ciccio"));


		Region reg = new Region ("Floor", Vector3.zero, new Vector3 (15f, 15f, 15f), PrimitiveType.Sphere, false, Quaternion.identity);
		print("SENDMESSAGETOREGION: " + LindaCoordinationUtilities.SendMessageToRegion ("aiuto",reg));

		print("ASKPAR: " + LindaCoordinationUtilities.AskParam ("X","chop(X)").Equals (0));

		print ("SituatedObjectFromArea " + LindaCoordinationUtilities.GetSituatedObjectsFromArea (Vector3.zero,1f,4).Length);

		LindaCoordinationUtilities.BroadcastMessage ("peppino(prisco)");

		print ("RESULT BROADCAST " + LindaCoordinationUtilities.Ask ("peppino(prisco)") + " " + 
			LindaCoordinationUtilities.Ask ("peppino(prisco)","Ciccio") + " " + LindaCoordinationUtilities.Retrieve ("peppino(prisco)","Floor"));

		print ("RETRIEVE THE PREVIOUS MESSAGE " + LindaCoordinationUtilities.RetrieveParam ("X","peppino(X)"));
	}

	async void Update(){
		//print ("CommunicationLibraryTest Update");
		if (!once) {
			once = true;
			await Task.Delay (5000);
			print("TELL: " + LindaCoordinationUtilities.Tell ("pippolo","Ciccio"));
		}
	}

	void OnTriggerEnter(Collider coll) {
		print ("TRIGGER " + gameObject.name + " : " + coll);
		print("TRIGGEREDDDD " + LindaCoordinationUtilities.AskMessageFromSituatedObjectTriggered ("pippolo",coll));
	}

}

