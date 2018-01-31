using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prolog;

public class PrologTestScript2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		KnowledgeBase.Global.Consult ("KB/globalKB");
		KnowledgeBase k = new KnowledgeBase("knowledgebase3",gameObject,null);//in questo modo vede solo la kb locale, non ereditando dalla globale
		//k.Consult ("KB/knowledgebase3");
		k.Consult ("KB/knowledgebase3");

		print(k.IsTrue (new Structure ("tryThis",666)));
		//print(KnowledgeBase.Global.IsTrue (new Structure ("tryThis",666)));
		//print(k.IsTrue (new Structure ("++!kill")));

		//print (k.IsTrue (new Structure ("!", "ciccio")));//con la closed world assumption, se non c'è, stampa False e continua


	}


}
