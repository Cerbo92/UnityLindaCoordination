using LindaCoordination;
using UnityEngine;
using Prolog;
using Linda;
using System;
using System.Threading.Tasks;

public class CommLibTest2 : AbstractLinda
{
	bool once = false;
	async void Start(){
		print ("CommLibTest2");


	}

	async void Update(){
		//print ("CIAOOOO");
		if (!once) {
			once = true;
			print("TEST4: " + await LindaCoordinationUtilities.AskSuspend ("pippolo","Ciccio",this));
			print("OUTFLOOR: " + LindaCoordinationUtilities.Tell ("pippolo","Floor"));
		}

	}

}

