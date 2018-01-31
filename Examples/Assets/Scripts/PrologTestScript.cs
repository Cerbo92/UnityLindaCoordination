using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prolog;

public class PrologTestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		KnowledgeBase.Global.Consult ("KB/globalKB");
		KnowledgeBase k = new KnowledgeBase("knowledgebase",gameObject,null);//in questo modo vede solo la kb locale, non ereditando dalla globale
		k.Consult ("KB/knowledgebase2");
		if (k.IsTrue (new Structure("piano"))) {
			Debug.Log ("DAJEEEEEE");
		}
//		KnowledgeBase k = new KnowledgeBase("knowledgebase2",GameObject.Find ("Agent1"));
//		PrologContext.Allocate (k, this);
//		UnityExtensionMethods.KnowledgeBase (gameObject);
//
//		if (UnityExtensionMethods.IsTrue (this.gameObject, new Structure ("pippo"))) {
//			Debug.Log ("AAAAAAAAA");
//		}
		k.Assert (new Structure("ciccio", 2),false,false);
		if (k.IsTrue (new Structure("ciccio",2))) {
			Debug.Log ("OSTIA funziona l'assert da C#");
		}

		if(KnowledgeBase.Global.IsTrue (new Structure("pippo"))){
			Debug.Log ("pippo term called");
		}

		if(KnowledgeBase.Global.IsTrue (new Structure("pippo", 2))){
			Debug.Log ("pippo(2) term called");
		} 

//		if(KnowledgeBase.Global.IsTrue(new Structure("pluto"))){
//			Debug.Log ("pluto predicate called (if any)");//se questo predicato/fatto non esiste, si inchioda tutto
//		}

		LogicVariable x = new LogicVariable ("X");
		KnowledgeBase.Global.SolveFor (x, new Structure ("pluto", x), this);//unificazione di X con il fatto paperino(X) passando dal predicato pluto(X)
		Debug.Log("C# X value through predicate: " + x.ToString ());

		LogicVariable y = new LogicVariable ("X");
		KnowledgeBase.Global.SolveFor (y, new Structure ("paperino", y), this);//unificazione di X con il fatto paperino(X)
		Debug.Log("C# X value unifying fact: " + y.ToString ());

		KnowledgeBase.Global.IsTrue(new Structure("testp"), this);//chiamata a metodo da Prolog senza argomenti

		KnowledgeBase.Global.IsTrue (new Structure("testpArgs"),this);//chiamata di metodo da Prolog con argomenti
	}

	public string TestM(){
		//Debug.Log ("TestM called");
		return "Method TestM called";
	}

	public string TestMWithArgs(int x){
		//Debug.Log ("TestMWithArgs called");
		return "Method TestMWithArgs called with par: " + x;
	}
}
