using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prolog;
using System;

namespace Linda{

	/// <summary>
	/// Static class with utilities for interacting with Prolog using Linda primitives.
	/// </summary>
	[Documentation("Static class with utilities for interacting with Prolog using Linda primitives.")]
	public static class LindaLibraryCoroutines {

		//private static List<TaskCompletionSource<bool>> tcs = new List<TaskCompletionSource<bool>> ();
		private static Dictionary<Component,bool> tcs = new Dictionary<Component,bool> ();

		/// <summary>
		/// Sets the global KB, in the GameObject called GlobalKB.
		/// </summary>
		/// <param name="kb">The path of Prolog file</param>
		/// <param name="erase">If set to <c>true</c> erase.</param>
		public static void SetGlobalKB(string kb, bool erase = false){
			string kb1 = kb.Contains (".prolog") ? kb : kb + ".prolog";
			if (erase) {
				KnowledgeBase.Global.Reconsult (string.Format ("KB/{0}", kb1));
			}
			KnowledgeBase.Global.Consult (string.Format ("KB/{0}", kb1));
		}

		/// <summary>
		/// Initialize the local gameObject's KB with the one indicated by path: it is also the KB which could be exchanged with
		/// other GOs
		/// </summary>
		/// <returns>The local KB.</returns>
		/// <param name="path">Path of the Prolog file to be loaded, must be inside KB folder</param>
		/// <param name="gameObject">The target GameObject</param>
		/// <param name="erase">If set to <c>true</c> erase the previous KB.</param>
		public static KnowledgeBase SetLocalKB(string path, GameObject gameObject, bool erase = false){
			if (path == null || path.Equals (""))
				throw new SyntaxErrorException (path, "Path must be a non null .prolog file");
			string path1 = path.Contains (".prolog") ? path : path + ".prolog";
			//Debug.Log (path + " VS " + path1);
			KnowledgeBase k = new KnowledgeBase ("", gameObject, null);
			if (erase) {
				k.Reconsult (string.Format ("KB/{0}", path1));
				gameObject.GetComponent<AbstractLinda> ().LocalKB = k;
				return gameObject.GetComponent<AbstractLinda> ().LocalKB;
			}
			k.Consult (string.Format ("KB/{0}", path1));
			gameObject.GetComponent<AbstractLinda> ().LocalKB = k;
			return gameObject.GetComponent<AbstractLinda> ().LocalKB;
		}

		/// <summary>
		/// Creates a local private KB.
		/// </summary>
		/// <returns>The local private KB.</returns>
		/// <param name="path">Path of the Prolog file to be loaded.</param>
		/// <param name="gameObject">Game object.</param>
		public static KnowledgeBase CreateLocalPrivateKB(string path, GameObject gameObject){
			if (path == null || path.Equals (""))
				throw new SyntaxErrorException (path, "Path must be a non null .prolog file");
			string path1 = path.Contains (".prolog") ? path : path + ".prolog";
			KnowledgeBase k = new KnowledgeBase(path,gameObject,null);
			k.Consult (string.Format ("KB/{0}", path1));
			return k;
		}

		/// <summary>
		/// Gets the gameobject's local KB: it is a reference, so it may be null if the gameobject . IMPORTANT: the GameObject must have a script which extends AbstractLinda in order to have a
		/// local public KB.
		/// </summary>
		/// <returns>The gameobject's local KB, null if no KB was found.</returns>
		/// <param name="gameObject">The Gameobject.</param>
		public static KnowledgeBase GetGameobjectLocalKB(GameObject gameObject){
			if (gameObject == null) {
				Debug.LogWarning ("LindaLibrary:GetGameobjectLocalKB -> The gameobject is null: is it real? Please check");
			}
			KnowledgeBase target = null;
			try {
				target = gameObject.GetComponent<AbstractLinda> ().LocalKB;
			} catch (Exception ex) {
				Debug.LogWarning ("LindaLibrary:GetGameobjectLocalKB -> AbstractLinda script not found: do you have extended it in your script? " + ex);
			}
			return gameObject.GetComponent<AbstractLinda> ().LocalKB;

		}

		//chiamata su una determinata KB
		/// <summary>
		/// RD call on a specific KB
		/// </summary>
		/// <returns><c>true</c>, if tuple was founded, <c>false</c> otherwise.</returns>
		/// <param name="tuple">The tuple.</param>
		/// <param name="k">K.</param>
		public static bool Linda_RD(string tuple, KnowledgeBase k){
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("rd("+tuple1+").") as Structure;
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated... wait a little longer");
				//Debug.Break ();
			}
			return k.IsTrue (str);
		}

		/// <summary>
		/// RD call on the global KB
		/// </summary>
		/// <returns><c>true</c>, if tuple was founded, <c>false</c> otherwise.</returns>
		/// <param name="tuple">The tuple.</param>
		public static bool Linda_RD(string tuple){
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("rd("+tuple1+").") as Structure;
			return KnowledgeBase.Global.IsTrue (str);
		}

		/// <summary>
		/// RD call on local KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value.</returns>
		/// <param name="var">Variable.</param>
		/// <param name="tuple">Tuple.</param>
		/// <param name="k">The KB.</param>
		/// <param name="g">The object to be bounded to $this Prolog side.</param>
		public static object Linda_RD(string var, string tuple, KnowledgeBase k, object g){
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, i did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1+":rd("+tuple1+").") as Structure;
			k.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, false);
			return str.Argument (0);
		}

		//chiamata sulla KB globale
		/// <summary>
		/// RD call on global KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value.</returns>
		/// <param name="var">Variable.</param>
		/// <param name="tuple">Tuple.</param>
		/// <param name="g">The object to be bounded to $this Prolog side.</param>
		public static object Linda_RD(string var, string tuple, object g){
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, i did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1+":rd("+tuple1+").") as Structure;
			KnowledgeBase.Global.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, false);
			return str.Argument (0);
		}

		//chiamata su una determinata KB
		/// <summary>
		/// OUT call on specific KB
		/// </summary>
		/// <returns><c>true</c>, if tuple was asserted, <c>false</c> otherwise.</returns>
		/// <param name="tuple">Tuple.</param>
		/// <param name="k">KB.</param>
		public static bool Linda_OUT(string tuple, KnowledgeBase k){
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated... wait a little longer");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("out("+tuple1+").") as Structure;
			return k.IsTrue (str);
		}

		//chiamata sulla KB globale
		/// <summary>
		/// OUT call on global KB
		/// </summary>
		/// <returns><c>true</c>, if tuple was asserted, <c>false</c> otherwise.</returns>
		/// <param name="tuple">Tuple.</param>
		public static bool Linda_OUT(string tuple){
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("out("+tuple1+").") as Structure;
			return KnowledgeBase.Global.IsTrue (str);
		}

		//chiamata su una determinata KB
		/// <summary>
		/// IN call on specific KB
		/// </summary>
		/// <returns><c>true</c>, if the tuple was founded, <c>false</c> otherwise.</returns>
		/// <param name="tuple">Tuple.</param>
		/// <param name="k">KB.</param>
		public static bool Linda_IN(string tuple, KnowledgeBase k){
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated... wait a little longer");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("in("+tuple1+").") as Structure;
			return k.IsTrue (str);
		}

		//chiamata sulla KB globale
		/// <summary>
		/// IN call on global KB
		/// </summary>
		/// <returns><c>true</c>, if the tuple was founded, <c>false</c> otherwise.</returns>
		/// <param name="tuple">Tuple.</param>
		public static bool Linda_IN(string tuple){
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("in("+tuple1+").") as Structure;
			return KnowledgeBase.Global.IsTrue (str);
		}

		//chiamata sulla KB locale
		/// <summary>
		/// IN call on specific KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value.</returns>
		/// <param name="var">Variable.</param>
		/// <param name="tuple">Tuple.</param>
		/// <param name="k">KB.</param>
		/// <param name="g">The object to be bounded to $this Prolog side.</param>
		public static object Linda_IN(string var, string tuple, KnowledgeBase k, object g){
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated... wait a little longer");
			}
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, i did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
				Debug.Log (var1);
			}
			var str = ISOPrologReader.Read (var1+":in("+tuple+").") as Structure;
			k.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, false);
			return str.Argument (0);
		}

		//chiamata sulla KB globale
		/// <summary>
		/// IN call on global KB, with variable to be unified
		/// </summary>
		/// <returns>The I.</returns>
		/// <param name="var">Variable.</param>
		/// <param name="tuple">Tuple.</param>
		/// <param name="g">The object to be bounded to $this Prolog side.</param>
		public static object Linda_IN(string var, string tuple, object g){
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, i did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
				Debug.Log (var1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1+":in("+tuple1+").") as Structure;
			KnowledgeBase.Global.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, false);
			return str.Argument (0);
		}

		//chiamata a KB locale
		/// <summary>
		/// Suspensive IN call on a specific KB, must be called inside an async method and properly awaited in order to be synchronous,
		/// otherwise it will be an asynchronous call
		/// </summary>
		/// <returns>If it returns, it always succeed.</returns>
		/// <param name="tuple">Tuple.</param>
		/// <param name="k">KB.</param>
		/// <param name="g">The object to be bounded to $this Prolog side, IT MUST BE an AbstractLinda component,
		///  so please extend your script with it and write "this" here.</param>
		public static IEnumerator Linda_IN_SUSP(string tuple, KnowledgeBase k, Component g)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated... wait a little longer");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("in_susp(" + tuple1 + ").") as Structure;
			bool res = k.IsTrue (str,g);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				if (!tcs.ContainsKey (g)) {
					bool tcs1 = false;
					tcs.Add (g,tcs1);
				}

				while (!tcs[g]) {
					yield return null;
				}
				//Debug.Log ("DONE COROUTINE");
				//await Prolog call
				tcs.Remove (g);
				//tcs = new TaskCompletionSource<bool>();
				//Debug.Log ("CIAO SONO STATO SVEGLIATO");
			}
		}

		//chiamata a KB globale
		/// <summary>
		/// Suspensive IN call on a global KB, must be called inside an async method and properly awaited in order to be synchronous,
		/// otherwise it will be an asynchronous call
		/// </summary>
		/// <returns>If it returns, it always succeed.</returns>
		/// <param name="tuple">Tuple.</param>
		/// <param name="g">The object to be bounded to $this Prolog side, IT MUST BE an AbstractLinda component,
		///  so please extend your script with it and write "this" here.</param>
		public static IEnumerator Linda_IN_SUSP(string tuple,Component g)
		{
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("in_susp(" + tuple1 + ").") as Structure;
			bool res = KnowledgeBase.Global.IsTrue (str,g);
			//Debug.Log (res);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				if (!tcs.ContainsKey (g)) {
					bool tcs1 = false;
					tcs.Add (g,tcs1);
				}

				while (!tcs[g]) {
					yield return null;
				}
				//Debug.Log ("DONE COROUTINE");
				//await Prolog call
				tcs.Remove (g);
				//tcs = new TaskCompletionSource<bool>();
				//Debug.Log ("CIAO SONO STATO SVEGLIATO");
			}
		}

		//chiamata a KB locale
		//chiamata a KB locale
		/// <summary>
		/// Suspensive RD call on a specific KB, must be called inside an async method and properly awaited in order to be synchronous,
		/// otherwise it will be an asynchronous call
		/// </summary>
		/// <returns>If it returns, it always succeed.</returns>
		/// <param name="tuple">Tuple.</param>
		/// <param name="k">KB.</param>
		/// <param name="g">The object to be bounded to $this Prolog side, IT MUST BE an AbstractLinda component,
		///  so please extend your script with it and write "this" here.</param>
		public static IEnumerator Linda_RD_SUSP(string tuple, KnowledgeBase k, Component g)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated... wait a little longer");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("rd_susp(" + tuple1 + ").") as Structure;
			bool res = k.IsTrue (str,g);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				if (!tcs.ContainsKey (g)) {
					bool tcs1 = false;
					tcs.Add (g,tcs1);
				}

				while (!tcs[g]) {
					yield return null;
				}
				//Debug.Log ("DONE COROUTINE");
				//await Prolog call
				tcs.Remove (g);
				//tcs = new TaskCompletionSource<bool>();
				//Debug.Log ("CIAO SONO STATO SVEGLIATO");
			}
		}

		//chiamata a KB globale
		/// <summary>
		/// Suspensive RD call on a global KB, must be called inside an async method and properly awaited in order to be synchronous,
		/// otherwise it will be an asynchronous call
		/// </summary>
		/// <returns>If it returns, it always succeed.</returns>
		/// <param name="tuple">Tuple.</param>
		/// <param name="g">The object to be bounded to $this Prolog side, IT MUST BE an AbstractLinda component,
		///  so please extend your script with it and write "this" here.</param>
		public static IEnumerator Linda_RD_SUSP(string tuple,Component g)
		{
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("rd_susp(" + tuple1 + ").") as Structure;
			bool res = KnowledgeBase.Global.IsTrue (str,g);
			Debug.Log (res);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				if (!tcs.ContainsKey (g)) {
					bool tcs1 = false;
					tcs.Add (g,tcs1);
				}

				while (!tcs[g]) {
					yield return null;
				}
				//Debug.Log ("DONE COROUTINE");
				//await Prolog call
				tcs.Remove (g);
				//tcs = new TaskCompletionSource<bool>();
				//Debug.Log ("CIAO SONO STATO SVEGLIATO");
			}
		}

		/// <summary>
		/// Awakes the agent.
		/// </summary>
		public static void AwakeAgent(Component c){
			try {
				if(tcs.ContainsKey (c)){
					tcs[c] = true;
				}
				//tcs.TrySetResult (true);
			} catch (System.Exception ex) {
				Debug.LogError ("LindaLibrary: EXCEPTION in AwakeAgent "+ex.Data);
			}

		}


	}
}