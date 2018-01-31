using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prolog;
using Linda;
using System.Threading.Tasks;

public class LindaKBTest2 : AbstractLinda {

	bool once = false;

	// Use this for initialization
	void Start () {

		//KnowledgeBase k = new KnowledgeBase("linda",gameObject,null);//in questo modo vede solo la kb locale, non ereditando dalla globale
		//k.Consult ("KB/linda.prolog");

		//KnowledgeBase k = LindaLibrary.setLocalKB(gameObject);
		//LindaLibrary.setGlobalKB ("linda");
		//testLindaLibrary (k);
		//print("rd(pasticcio(loves,ciccio)). " + LindaLibrary.Linda_RD ("pinolo(666)"));
//		await Task.Delay (5000);
//		print ("OUT 1 " + LindaLibrary.Linda_OUT ("pasticcio(loves,ciccio)"));
//		print ("OUT DONE 1");
//
//		await Task.Delay (5000);
//		print ("OUT 2 " + LindaLibrary.Linda_OUT ("pasticcio(loves,ciccio)"));
//		print ("OUT DONE 2");
		LindaLibrary.SetLocalKB ("linda",gameObject);
		//setLocalKB ("test");

	}

	async void Ehi ()
	{
		while (!once) {
			once = true;
			await Task.Delay (3000);
			print ("OUT " + LindaLibrary.Linda_OUT ("chop(101)", LocalKB));
		}
	}

	// Update is called once per frame
	void Update () {


		while (!once) {
			Ehi ();
//			await Task.Delay (1500);
//			print ("GLOBAL KB, MUST BE TRUE: " + LindaLibrary.Linda_RD ("chop(0)"));
//			print ("RD ON LOCAL KB, MUST BE FALSE: " + LindaLibrary.Linda_RD ("chop(0)",LocalKB));//pinolo(666) dà true, c'è e va bene
//			print ("RD ON LOCAL KB, MUST BE TRUE: " + LindaLibrary.Linda_RD ("chop(11)",LocalKB));
//			KnowledgeBase kk = LindaLibrary.GetGameobjectLocalKB (GameObject.Find ("LindaAgent"));
//			print ("RD ON EXTERNAL KB, MUST BE TRUE: " + LindaLibrary.Linda_RD ("pinolo(666)",kk));

//			var res = LindaLibrary.Linda_U_IN ("X", "chop(X)", LocalKB, this);
//			print ("RD ON LOCAL KB, MUST BE TRUE: " + LindaLibrary.Linda_U_RD("X","chop(X)",LocalKB,this));
//			print ("RD ON LOCAL KB, MUST BE TRUE: " + res );
//			print ("RD ON LOCAL KB, MUST BE TRUE: " + LindaLibrary.Linda_U_RD("X","chop(X)",LocalKB,this));
			//print ("RD ON LOCAL KB, MUST BE TRUE: " +LindaLibrary.Linda_U_IN ("X", "chop(X)", LocalKB, this) );
			//print ("BLABLA " + LocalKB.IsTrue (new Structure ("u_rd","chop")));//pinolo è solo nella KB locale, non nella globale
		}

		//lindaKBTest ();


		//gameObject.KnowledgeBase ().Reconsult ("KB/linda.prolog");
		//print (gameObject.IsTrueParsed ("paperino(paperina)."));//è nella globale
		//print (gameObject.IsTrueParsed ("pinolo(loves,pasticcio)."));//è nella locale
		//print (gameObject.SolveForParsed ("X:pinolo(loves,X)."));//è nella locale

		//print("test(pinolo) -> " + k.IsTrue (new Structure ("test","pinolo"))); //per unificare con un fatto del tipo pinolo(gino) bisogna trasformarlo in atomo
		//print("rd pinolo -> " + k.IsTrue (new Structure ("rd","pinolo")));
		//print(KnowledgeBase.Global.IsTrue (new Structure ("tryThis",666)));
		//print(k.IsTrue (new Structure ("++!kill")));

		//print (k.IsTrue (new Structure ("!", "ciccio")));//con la closed world assumption, se non c'è, stampa False e continua


	}

	void lindaKBTest ()
	{
		//KnowledgeBase.Global.Consult ("KB/globalKB.prolog");
		KnowledgeBase k = new KnowledgeBase("linda",gameObject,null);//in questo modo vede solo la kb locale, non ereditando dalla globale
		k.Consult ("KB/linda.prolog");

		print("test(pinolo) -> " + k.IsTrue (new Structure ("test","pinolo"))); //c'è tuple(pinolo), non pinolo.

		LogicVariable x = new LogicVariable ("X");
		k.SolveFor (x, new Structure ("rd", new Structure("pinolo", x)), this);
		Debug.Log("Solve rd(pinolo(X)): " + x);

		LogicVariable y = new LogicVariable ("X");
		k.SolveFor (y, new Structure ("rd", new Structure ("pinolo", Symbol.Intern ("loves"), y)), this);
		Debug.Log("Who does Pinolo love?: " + y);

		var str = ISOPrologReader.Read ("rd(pinolo(loves,pasticcio)).") as Structure;
		print("rd(pinolo(loves,pasticcio)). " + k.IsTrue (str));

		var str1 = ISOPrologReader.Read ("rd(pinolo(loves,ciccio)).") as Structure;
		print("rd(pinolo(loves,ciccio)). " + k.IsTrue (str1));

		var str2 = ISOPrologReader.Read ("X:rd(pinolo(X,pasticcio)).") as Structure;
		k.SolveFor (str2.Argument (0) as LogicVariable, str2.Argument (1), this);
		print ("X:pinolo(X,pasticcio). " + str2.Argument (0));

		var str3 = ISOPrologReader.Read ("paperino(paperina).") as Structure; //se fallisce, torna false, questo fatto sarebbe nella globalKB
		print("paperino(paperina). " + k.IsTrue (str3));

		var str4 = ISOPrologReader.Read ("X:rd(paperino(X)).") as Structure;
		k.SolveFor (str4.Argument (0) as LogicVariable, str4.Argument (1), this, false); //se fallisce, tira un'eccezione, ma con false alla fine non si blocca
		print ("X:rd(paperino(X)). " + str4.Argument (0));

		var str5 = ISOPrologReader.Read ("out(pinolo(loves,pasticcio)).") as Structure; //c'è già, fallisce
		print("out(pinolo(loves,pasticcio)). " + k.IsTrue (str5));

		str5 = ISOPrologReader.Read ("in(pinolo(true)).") as Structure;
		print("in(pinolo(true)). " + k.IsTrue (str5));

		var str8 = ISOPrologReader.Read ("rd(pasticcio(loves,pinolo)).") as Structure; 
		print("test rd before out -> " + k.IsTrue (str8));

		var str6 = ISOPrologReader.Read ("out(pasticcio(loves,pinolo)).") as Structure; 
		print("out(pasticcio(loves,pinolo)). " + k.IsTrue (str6));

		var str7 = ISOPrologReader.Read ("rd(pasticcio(loves,pinolo)).") as Structure; 
		print("test rd after out -> " + k.IsTrue (str7));

		//TEST WAITQUEUE METODI rd_susp E in_susp
		// NB: lo spazio di tuple già possiede tuple_s(pasticcio(loves,ciccio),gesu), quindi gesù è arrivato prima di tutti e deve essere servito prima


		str7 = ISOPrologReader.Read ("rd_susp(pasticcio(loves,pinolo)).") as Structure; 
		print("test rd_susp with tuple already present -> " + k.IsTrue (str7));//tupla messa prima, torna true subito

		str7 = ISOPrologReader.Read ("in_susp(pasticcio(loves,pinolo)).") as Structure; 
		print("test in_susp with tuple already present -> " + k.IsTrue (str7));//tupla messa prima, torna true subito

		str7 = ISOPrologReader.Read ("rd_susp(pasticcio(loves,ciccio)).") as Structure; 
		print("test rd_susp with tuple not present -> " + k.IsTrue (str7));//non c'è, mi dovrei sospendere

		str4 = ISOPrologReader.Read ("X:rd(tuple_s(rd,_,X)).") as Structure;
		k.SolveFor (str4.Argument (0) as LogicVariable, str4.Argument (1), this, false); //se fallisce, tira un'eccezione, ma con false alla fine non si blocca
		print ("X:rd(tuple_s(rd,_,X)). " + str4.Argument (0));

		str7 = ISOPrologReader.Read ("out(pasticcio(loves,ciccio)).") as Structure; 
		print("Writing a tuple the agent is waiting for, but the italian Jesus came first -> " + k.IsTrue (str7));
		//c'è gesu che aspetta questa tupla da prima di me, ma siamo entrambi in rd e quindi veniamo svegliati entrambi
		//NB: torna false perché non c'è nessuno da svegliare nella coda di IN

		str7 = ISOPrologReader.Read ("out(pasticcio(loves,ciccio)).") as Structure; 
		print("Writing a tuple the agent is waiting for, now it's my turn -> " + k.IsTrue (str7));
		//provo a rifare la out ma, essendoci solo agenti sospesi in rd, la tupla non è stat eliminata, quindi la out fallisce

		//SIMULAZIONE in_susp
		//tuple_s(in,pasticcio(kills,pinolo),giuseppina). già presente, quindi mi sospendo dopo di lei non essendoci ancora la tupla
		str7 = ISOPrologReader.Read ("in_susp(pasticcio(kill,pinolo)).") as Structure; 
		print("Test in_susp with tuple not present -> " + k.IsTrue (str7));

		str7 = ISOPrologReader.Read ("out(pasticcio(kill,pinolo)).") as Structure; 
		print("Writing a tuple the agent is waiting for, but Giuseppina came first -> " + k.IsTrue (str7));//giuseppina viene servita

		str7 = ISOPrologReader.Read ("out(pasticcio(kill,pinolo)).") as Structure; 
		print("My turn now -> " + k.IsTrue (str7));//vengo servito io
	}

	private void lindaRead(){

	}


	async void DoThis ()
	{
		print ("CIAO IO MI SVEGLIO TRA 5 SECONDI");
		await Task.Delay (5000);
		LindaLibrary.AwakeAgent (this);
		print ("EHI HO FATTO LA AWAKE E ME NE VADO");
	}

	void testLindaLibrary (KnowledgeBase k)
	{
		print("out(pinolo(loves,pasticcio)). " + LindaLibrary.Linda_OUT ("pasticcio(loves,ciccio)",k));
		//		var str5 = ISOPrologReader.Read ("out(pasticcio(loves,ciccio)).") as Structure;
		//		print("out(pinolo(loves,pasticcio)). " + k.IsTrue (str5));
		print("rd(tuple_s(rd,pasticcio(loves,ciccio),gesu)). " + LindaLibrary.Linda_RD ("tuple_s(rd,pasticcio(loves,ciccio),gesu)",k));
		//		var str5 = ISOPrologReader.Read ("rd(tuple_s(rd,pasticcio(loves,ciccio),gesu)).") as Structure;
		//		print("rd(tuple_s(rd,pasticcio(loves,ciccio),gesu)). " + k.IsTrue (str5));
		print("rd(pasticcio(loves,ciccio)). " + LindaLibrary.Linda_RD ("pasticcio(loves,ciccio)",k));
		//		var str5 = ISOPrologReader.Read ("rd(pasticcio(loves,ciccio)).") as Structure;
		//		print("rd(pasticcio(loves,ciccio)). " + k.IsTrue (str5));
		print ("X:pinolo(X,pasticcio). " + LindaLibrary.Linda_RD ("Y","pinolo(Y,pasticcio)",k,this));
		//DoThis ();
	}
}
