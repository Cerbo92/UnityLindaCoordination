using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prolog;
using System.Threading.Tasks;
using System;

namespace Linda
{

	/// <summary>
	/// Library with utilities for interacting with Prolog using Linda primitives.
	/// </summary>
	[Documentation ("Static class with utilities for interacting with Prolog using Linda primitives.")]
	public static class LindaLibrary
	{
		
		//private static List<TaskCompletionSource<bool>> tcs = new List<TaskCompletionSource<bool>> ();
		private static Dictionary<Component,TaskCompletionSource<bool>> tcs = new Dictionary<Component,TaskCompletionSource<bool>> ();
		private static Dictionary<Component,bool> tcss = new Dictionary<Component,bool> ();

		/// <summary>
		/// Sets the global KB, in the GameObject called GlobalKB.
		/// </summary>
		/// <param name="kb">The path of Prolog file</param>
		/// <param name="erase">If set to <c>true</c> erase.</param>
		public static void SetGlobalKB (string kb, bool erase = false)
		{
			string kb1 = kb.Contains (".prolog") ? kb : kb + ".prolog";
			if (erase) {
				KnowledgeBase.Global.Reconsult (kb1);
			}
			KnowledgeBase.Global.Consult (kb1);
		}

		/// <summary>
		/// Initialize the local gameObject's KB with the one indicated by path: it is also the KB which could be exchanged with
		/// other GOs
		/// </summary>
		/// <returns>The local KB.</returns>
		/// <param name="path">Path of the Prolog file to be loaded, must be inside KB folder.
		/// NB: if you write "empty" or "", it will be loaded an empty KB with only Linda support</param>
		/// <param name="gameObject">The target GameObject</param>
		/// <param name="erase">If set to <c>true</c> erase the previous KB.</param>
		public static KnowledgeBase SetLocalKB (string path, GameObject gameObject, bool erase = false)
		{
			if (path == null)
				throw new SyntaxErrorException (path, "Path must be a non null .prolog file");
			if (path.Equals ("") || path.Equals ("empty") || path.Equals ("linda")) {
				Debug.Log ("Empty path = empty kb, only Linda support");
				KnowledgeBase kb = new KnowledgeBase ("", gameObject, null);
				kb.Consult ("KB/linda.prolog");
				gameObject.GetComponent<AbstractLinda> ().LocalKB = kb;
				return gameObject.GetComponent<AbstractLinda> ().LocalKB;
			}
			string path1 = path.Contains (".prolog") ? path : path + ".prolog";
			//Debug.Log (path + " VS " + path1);
			KnowledgeBase k = new KnowledgeBase ("", gameObject, null);
			if (erase) {
				k.Reconsult (path1);
				gameObject.GetComponent<AbstractLinda> ().LocalKB = k;
				return gameObject.GetComponent<AbstractLinda> ().LocalKB;
			}
			k.Consult (path1);
			gameObject.GetComponent<AbstractLinda> ().LocalKB = k;
			return gameObject.GetComponent<AbstractLinda> ().LocalKB;
		}

		/// <summary>
		/// Creates a local private KB with the specified Prolog theory.
		/// </summary>
		/// <returns>The local private KB.</returns>
		/// <param name="path">Path of the Prolog file to be loaded, must be inside KB folder.</param>
		public static KnowledgeBase CreateLocalPrivateKB (string path)
		{
			if (path == null || path.Equals (""))
				throw new SyntaxErrorException (path, "Path must be a non null .prolog file");
			string path1 = path.Contains (".prolog") ? path : path + ".prolog";
			KnowledgeBase k = new KnowledgeBase (path, null, null);
			k.Consult (path1);
			return k;
		}

		/// <summary>
		/// Gets the gameobject's local KB: it is a reference, so it may be empty if not yet explicitly created. IMPORTANT: the GameObject must have a script which extends AbstractLinda in order to have a
		/// local public KB.
		/// </summary>
		/// <returns>The gameobject's local KB, null if no KB was found.</returns>
		/// <param name="gameObject">The Gameobject.</param>
		public static KnowledgeBase GetGameobjectLocalKB (GameObject gameObject)
		{
			if (gameObject == null) {
				Debug.LogWarning ("ATTENTION: The searched gameobject is null.");
				return null;
			}
			if (gameObject.GetComponent<AbstractLinda> () == null) {
				Debug.LogWarning ("ATTENTION: AbstractLinda script not found: do you have extended it in your script?");
				return null;
			}
			KnowledgeBase target = gameObject.GetComponent<AbstractLinda> ().LocalKB;
			return target;

		}

		//chiamata su una determinata KB
		/// <summary>
		/// RD call on a specific KB
		/// </summary>
		/// <returns><c>true</c>, if tuple was founded, <c>false</c> otherwise.</returns>
		/// <param name="tuple">The tuple.</param>
		/// <param name="k">K.</param>
		public static bool Linda_RD (string tuple, KnowledgeBase k)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("rd(" + tuple1 + ").") as Structure;
			return k.IsTrue (str);
		}

		/// <summary>
		/// RD call on the global KB
		/// </summary>
		/// <returns><c>true</c>, if tuple was founded, <c>false</c> otherwise.</returns>
		/// <param name="tuple">The tuple.</param>
		public static bool Linda_RD (string tuple)
		{
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("rd(" + tuple1 + ").") as Structure;
			return KnowledgeBase.Global.IsTrue (str);
		}

		#pragma warning disable 168

		/// <summary>
		/// RD call on local KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value if correctly unified, false otherwise.</returns>
		/// <param name="var">Variable to be bounded to a value.</param>
		/// <param name="tuple">Searched tuple.</param>
		/// <param name="k">The KB.</param>
		/// <param name="g">The object to be bounded to $this Prolog side, could be null this time.</param>
		public static object Linda_RD (string var, string tuple, KnowledgeBase k, object g)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, I did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1 + ":rd(" + tuple1 + ").") as Structure;
			try {
				k.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, true);
				return str.Argument (0);
			} catch (Exception ex) {
				return false;
			}
		}

		#pragma warning restore 168

		#pragma warning disable 168

		//chiamata sulla KB globale
		/// <summary>
		/// RD call on global KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value if correctly unified, false otherwise.</returns>
		/// <param name="var">Variable to be bounded to a value.</param>
		/// <param name="tuple">Searched tuple.</param>
		/// <param name="g">The object to be bounded to $this Prolog side, could be null this time.</param>
		public static object Linda_RD (string var, string tuple, object g)
		{
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, I did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1 + ":rd(" + tuple1 + ").") as Structure;
			try {
				KnowledgeBase.Global.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, true);
				return str.Argument (0);
			} catch (Exception ex) {
				return false;
			}

		}

		#pragma warning restore 168

		//chiamata su una determinata KB
		/// <summary>
		/// OUT call on specific KB, no duplicates
		/// </summary>
		/// <returns><c>true</c>, if tuple was asserted, <c>false</c> otherwise.</returns>
		/// <param name="tuple">Tuple.</param>
		/// <param name="k">KB.</param>
		public static bool Linda_OUT (string tuple, KnowledgeBase k)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("out(" + tuple1 + ").") as Structure;
			return k.IsTrue (str);
		}

		//chiamata sulla KB globale
		/// <summary>
		/// OUT call on global KB, no duplicates
		/// </summary>
		/// <returns><c>true</c>, if tuple was asserted, <c>false</c> otherwise.</returns>
		/// <param name="tuple">Tuple.</param>
		public static bool Linda_OUT (string tuple)
		{
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("out(" + tuple1 + ").") as Structure;
			return KnowledgeBase.Global.IsTrue (str);
		}

		//chiamata su una determinata KB
		/// <summary>
		/// OUT call on specific KB, allowing duplicates
		/// </summary>
		/// <returns><c>true</c>, if tuple was asserted, <c>false</c> otherwise.</returns>
		/// <param name="tuple">Tuple.</param>
		/// <param name="k">KB.</param>
		public static bool Linda_OUT_D (string tuple, KnowledgeBase k)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated... wait a little longer");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("out_d(" + tuple1 + ").") as Structure;
			return k.IsTrue (str);
		}

		//chiamata sulla KB globale
		/// <summary>
		/// OUT call on global KB, allowing duplicates
		/// </summary>
		/// <returns><c>true</c>, if tuple was asserted, <c>false</c> otherwise.</returns>
		/// <param name="tuple">Tuple.</param>
		public static bool Linda_OUT_D (string tuple)
		{
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("out_d(" + tuple1 + ").") as Structure;
			return KnowledgeBase.Global.IsTrue (str);
		}

		//chiamata su una determinata KB
		/// <summary>
		/// IN call on specific KB.
		/// </summary>
		/// <returns><c>true</c>, if the tuple was founded, <c>false</c> otherwise.</returns>
		/// <param name="tuple">Tuple.</param>
		/// <param name="k">KB.</param>
		public static bool Linda_IN (string tuple, KnowledgeBase k)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("in(" + tuple1 + ").") as Structure;
			return k.IsTrue (str);
		}

		//chiamata sulla KB globale
		/// <summary>
		/// IN call on global KB
		/// </summary>
		/// <returns><c>true</c>, if the tuple was founded, <c>false</c> otherwise.</returns>
		/// <param name="tuple">Tuple.</param>
		public static bool Linda_IN (string tuple)
		{
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("in(" + tuple1 + ").") as Structure;
			return KnowledgeBase.Global.IsTrue (str);
		}

		#pragma warning disable 168

		//chiamata sulla KB locale
		/// <summary>
		/// IN call on specific KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value if correctly unified, false otherwise.</returns>
		/// <param name="var">Variable to be unified.</param>
		/// <param name="tuple">Searched tuple.</param>
		/// <param name="k">KB.</param>
		/// <param name="g">The object to be bounded to $this Prolog side.</param>
		public static object Linda_IN (string var, string tuple, KnowledgeBase k, object g)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, i did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1 + ":in(" + tuple1 + ").") as Structure;
			try {
				k.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, true);
				return str.Argument (0);
			} catch (Exception ex) {
				return false;
			}
		}

		#pragma warning restore 168

		#pragma warning disable 168

		//chiamata sulla KB globale
		/// <summary>
		/// IN call on global KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value if correctly unified, false otherwise.</returns>
		/// <param name="var">Variable to be unified.</param>
		/// <param name="tuple">Searched tuple.</param>
		/// <param name="g">The object to be bounded to $this Prolog side.</param>
		public static object Linda_IN (string var, string tuple, object g)
		{
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, i did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1 + ":in(" + tuple1 + ").") as Structure;
			try {
				KnowledgeBase.Global.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, true);
				return str.Argument (0);
			} catch (Exception ex) {
				return false;
			}
		}

		#pragma warning restore 168



		#region UniformRDCalls



		#pragma warning disable 168

		//chiamata sulla KB globale
		/// <summary>
		/// U_RD call on global KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value if correctly unified, false otherwise.</returns>
		/// <param name="var">Variable to be unified.</param>
		/// <param name="tuple">Searched tuple.</param>
		/// <param name="g">The object to be bounded to $this Prolog side.</param>
		public static object Linda_U_RD (string var, string tuple, object g)
		{
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, i did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1 + ":u_rd(" + var1 + "," + tuple1 + ").") as Structure;
			try {
				KnowledgeBase.Global.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, true);
				return str.Argument (0);
			} catch (Exception ex) {
				return false;
			}
		}

		#pragma warning restore 168

		#pragma warning disable 168

		/// <summary>
		/// U_RD call on local KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value if correctly unified, false otherwise.</returns>
		/// <param name="var">Variable to be unified.</param>
		/// <param name="tuple">Searched tuple.</param>
		/// <param name="k">The KB.</param>
		/// <param name="g">The object to be bounded to $this Prolog side.</param>
		public static object Linda_U_RD (string var, string tuple, KnowledgeBase k, object g)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, i did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1 + ":u_rd(" + var1 + "," + tuple1 + ").") as Structure;
			try {
				k.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, true);
				return str.Argument (0);
			} catch (Exception ex) {
				return false;
			}
		}

		#pragma warning restore 168



		#endregion

		#region UniformINCalls



		#pragma warning disable 168

		//chiamata sulla KB locale
		/// <summary>
		/// U_IN call on specific KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value if correctly unified, false otherwise.</returns>
		/// <param name="var">Variable to be unified.</param>
		/// <param name="tuple">Searched tuple.</param>
		/// <param name="k">KB.</param>
		/// <param name="g">The object to be bounded to $this Prolog side.</param>
		public static object Linda_U_IN (string var, string tuple, KnowledgeBase k, object g)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, i did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1 + ":u_in(" + var1 + "," + tuple1 + ").") as Structure;
			try {
				k.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, true);
				return str.Argument (0);
			} catch (Exception ex) {
				return false;
			}
		}

		#pragma warning restore 168

		#pragma warning disable 168

		//chiamata sulla KB globale
		/// <summary>
		/// U_IN call on global KB, with variable to be unified
		/// </summary>
		/// <returns>The variable value if correctly unified, false otherwise.</returns>
		/// <param name="var">Variable to be unified.</param>
		/// <param name="tuple">Searched tuple.</param>
		/// <param name="g">The object to be bounded to $this Prolog side.</param>
		public static object Linda_U_IN (string var, string tuple, object g)
		{
			string var1 = var;
			if (char.IsLower (var [0])) {
				Debug.LogWarning ("WARNING: The variable must have at least the first char in uppercase, i did it for you but please fix it");
				var1 = var [0].ToString ().ToUpper () + var.Substring (1);
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read (var1 + ":u_in(" + var1 + "," + tuple1 + ").") as Structure;
			try {
				KnowledgeBase.Global.SolveFor (str.Argument (0) as LogicVariable, str.Argument (1), g, true);
				return str.Argument (0);
			} catch (Exception ex) {
				return false;
			}
		}

		#pragma warning restore 168



		#endregion

		#region SuspensiveAsyncCalls

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
		public async static Task<bool> Linda_IN_SUSP (string tuple, KnowledgeBase k, Component g)
		{
			if (tcs.ContainsKey (g)) {
				Debug.LogWarning ("SUSPENSION ALREADY CALLED");
				return false;
			}
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("in_susp(" + tuple1 + ").") as Structure;
			bool res = k.IsTrue (str, g);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				TaskCompletionSource<bool> tcs1 = new TaskCompletionSource<bool> ();
				tcs.Add (g, tcs1);
				(g as AbstractLinda).Suspended = true;
				await tcs1.Task;
				//(g as AbstractLinda).Suspended = false;
				//await Prolog call
				tcs.Remove (g);
				//tcs = new TaskCompletionSource<bool>();
				//Debug.Log ("CIAO SONO STATO SVEGLIATO");
				return true;
			}

			return res;
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
		public async static Task<bool> Linda_IN_SUSP (string tuple, Component g)
		{
			if (tcs.ContainsKey (g)) {
				Debug.LogWarning ("SUSPENSION ALREADY CALLED");
				return false;
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("in_susp(" + tuple1 + ").") as Structure;
			bool res = KnowledgeBase.Global.IsTrue (str, g);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				TaskCompletionSource<bool> tcs1 = new TaskCompletionSource<bool> ();
				tcs.Add (g, tcs1);
				(g as AbstractLinda).Suspended = true;
				Debug.LogWarning (g.name + (g as AbstractLinda).Suspended);
				await tcs1.Task;
				//await Prolog call
				//(g as AbstractLinda).Suspended = false;
				Debug.LogWarning (g.name + (g as AbstractLinda).Suspended);
				tcs.Remove (g);
				//tcs = new TaskCompletionSource<bool>();
				//Debug.Log ("CIAO SONO STATO SVEGLIATO");
				return true;
			}

			return res;
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
		public async static Task<bool> Linda_RD_SUSP (string tuple, KnowledgeBase k, Component g)
		{
			if (tcs.ContainsKey (g)) {
				Debug.LogWarning ("SUSPENSION ALREADY CALLED");
				return false;
			}
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("rd_susp(" + tuple1 + ").") as Structure;
			bool res = k.IsTrue (str, g);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				TaskCompletionSource<bool> tcs1 = new TaskCompletionSource<bool> ();
				tcs.Add (g, tcs1);
				(g as AbstractLinda).Suspended = true;
				await tcs1.Task;
				//(g as AbstractLinda).Suspended = false;
				//await Prolog call
				tcs.Remove (g);
				//tcs = new TaskCompletionSource<bool>();
				//Debug.Log ("CIAO SONO STATO SVEGLIATO");
				return true;
			}

			return res;
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
		public async static Task<bool> Linda_RD_SUSP (string tuple, Component g)
		{
			if (tcs.ContainsKey (g)) {
				Debug.LogWarning ("SUSPENSION ALREADY CALLED");
				return false;
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("rd_susp(" + tuple1 + ").") as Structure;
			bool res = KnowledgeBase.Global.IsTrue (str, g);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				TaskCompletionSource<bool> tcs1 = new TaskCompletionSource<bool> ();
				tcs.Add (g, tcs1);
				(g as AbstractLinda).Suspended = true;
				await tcs1.Task;
				//await Prolog call
				tcs.Remove (g);
				//tcs = new TaskCompletionSource<bool>();
				//Debug.Log ("CIAO SONO STATO SVEGLIATO");
				return true;
			}

			return res;
		}

		#endregion


		#region SuspensiveCoroutineCalls

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
		public static IEnumerator Linda_IN_SUSP_Coroutine (string tuple, KnowledgeBase k, Component g)
		{
			if (!tcss.ContainsKey (g)) {
				Debug.LogWarning ("SUSPENSION ALREADY CALLED");
			}
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("in_susp(" + tuple1 + ").") as Structure;
			bool res = k.IsTrue (str, g);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				if (!tcss.ContainsKey (g)) {
					bool tcs1 = false;
					tcss.Add (g, tcs1);
				}
				(g as AbstractLinda).Suspended = true;
				while (!tcss [g]) {
					yield return null;
				}
				//Debug.Log ("DONE COROUTINE");
				//await Prolog call
				tcss.Remove (g);
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
		public static IEnumerator Linda_IN_SUSP_Coroutine (string tuple, Component g)
		{
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("in_susp(" + tuple1 + ").") as Structure;
			bool res = KnowledgeBase.Global.IsTrue (str, g);
			//Debug.Log (res);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				if (!tcss.ContainsKey (g)) {
					bool tcs1 = false;
					tcss.Add (g, tcs1);
				}
				(g as AbstractLinda).Suspended = true;
				while (!tcss [g]) {
					yield return null;
				}
				//Debug.Log ("DONE COROUTINE");
				//await Prolog call
				tcss.Remove (g);
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
		public static IEnumerator Linda_RD_SUSP_Coroutine (string tuple, KnowledgeBase k, Component g)
		{
			if (k == null) {
				throw new LindaLibraryException ("The KB is null, maybe it is not yet instantiated or the gameobject was not found");
			}
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("rd_susp(" + tuple1 + ").") as Structure;
			bool res = k.IsTrue (str, g);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				if (!tcss.ContainsKey (g)) {
					bool tcs1 = false;
					tcss.Add (g, tcs1);
				}
				(g as AbstractLinda).Suspended = true;
				while (!tcss [g]) {
					yield return null;
				}
				//Debug.Log ("DONE COROUTINE");
				//await Prolog call
				tcss.Remove (g);
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
		public static IEnumerator Linda_RD_SUSP_Coroutine (string tuple, Component g)
		{
			string tuple1 = tuple.EndsWith (".") ? tuple.Remove (tuple.Length - 1) : tuple; 
			var str = ISOPrologReader.Read ("rd_susp(" + tuple1 + ").") as Structure;
			bool res = KnowledgeBase.Global.IsTrue (str, g);
			Debug.Log (res);
			if (!res) {
				//Debug.Log ("CIAO SONO SOSPESO NELL'AWAIT");
				if (!tcss.ContainsKey (g)) {
					bool tcs1 = false;
					tcss.Add (g, tcs1);
				}
				(g as AbstractLinda).Suspended = true;
				while (!tcss [g]) {
					yield return null;
				}
				//Debug.Log ("DONE COROUTINE");
				//await Prolog call
				tcss.Remove (g);
				//tcs = new TaskCompletionSource<bool>();
				//Debug.Log ("CIAO SONO STATO SVEGLIATO");
			}
		}

		#endregion

		/// <summary>
		/// Awakes the agent.
		/// </summary>
		public static void AwakeAgent (Component c)
		{
			try {
				if (tcs.ContainsKey (c)) {
					(c as AbstractLinda).Suspended = false;
					tcs [c].TrySetResult (true);
				} else if (tcss.ContainsKey (c)) {
					(c as AbstractLinda).Suspended = false;
					tcss [c] = true;
				}
				//tcs.TrySetResult (true);
			} catch (Exception ex) {
				Debug.LogError ("LindaLibrary: EXCEPTION in AwakeAgent " + ex.Data);
			}

		}


	}
}