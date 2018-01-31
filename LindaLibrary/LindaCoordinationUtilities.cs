using System;
using Linda;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using UnityEditor;

namespace LindaCoordination
{
	/// <summary>
	/// High-level Linda interaction and coordination utilities.
	/// </summary>
	public static class LindaCoordinationUtilities
	{
		/// <summary>
		/// Sends the message to location, creating a new GameObject with tag "SpatialTupleSpace" and layer "SpatialTupleSpace": only cubic SpatialTupleSpaces are currently supported.
		/// </summary>
		/// <returns><c>true</c>, if message to location was sent, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to send to the tuple space.</param>
		/// <param name="ts">SpatialTupleSpace object, it will contain location, name and other params useful to create a situated tuple space</param>
		public static bool SendMessageToLocation (string message, SpatialTupleSpace ts)
		{
			GameObject point = GameObject.CreatePrimitive (PrimitiveType.Cube);
			point.GetComponent<MeshRenderer> ().enabled = !ts.Invisible;
			point.transform.localPosition = ts.Location;
			//point.transform.localPosition = location;
			point.transform.localScale = ts.Scale;
			point.name = ts.Name;
			point.tag = "SpatialTupleSpace";
			point.layer = LayerMask.NameToLayer ("SpatialTupleSpace");
			if (ts.IsRigid) {
				point.AddComponent<Rigidbody> ();
				//point.AddComponent<BoxCollider> ();
			}
			//if the collider scale is a negative infinite float number, it must be set to the default one, otherwise it's a custom number to be inserted
			if (!float.IsNegativeInfinity (ts.ColliderScale.x)) {
				point.GetComponent<BoxCollider> ().size = ts.ColliderScale;
			}
			point.GetComponent<Collider> ().isTrigger = ts.IsTrigger;
			SituatedPassiveKB script = point.AddComponent<SituatedPassiveKB> ();
			script.path = "";
			script.InitKB ();
			return LindaLibrary.Linda_OUT (message, script.LocalKB);
			//return LindaLibrary.Linda_OUT (message, LindaLibrary.GetGameobjectLocalKB (point));
		}

		/// <summary>
		/// Gets a limited amount of situated objects from given location within a given radius: it will cast a sphere of the given radius and
		/// retrieve all situated gameobjects within it (that are, the ones with layer of "SpatialTupleSpace" or "Region"), even yourself if you're of the right tipe.
		/// </summary>
		/// <returns>The situated objects from area.</returns>
		/// <param name="location">Location where to search.</param>
		/// <param name="radius">Radius of the area to search.</param>
		/// <param name="maxNumColliders">Max number of colliders to retrieve.</param>
		public static GameObject[] GetSituatedObjectsFromArea (Vector3 location, float radius, int maxNumColliders)
		{
			Collider[] results = new Collider[maxNumColliders];
			int layermask = (1 << LayerMask.NameToLayer ("SpatialTupleSpace")) | (1 << LayerMask.NameToLayer ("Region"));
			Physics.OverlapSphereNonAlloc (location, radius, results, layermask);
			return results.Where(x => x!=null).Select(x => x.gameObject).ToArray ();
		}

		/// <summary>
		/// Sends the message to global KB.
		/// </summary>
		/// <returns><c>true</c>, if message to global KB was sent, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to send.</param>
		public static bool SendMessageToGlobalKB (string message)
		{
			return LindaLibrary.Linda_OUT (message);
		}

		/// <summary>
		/// Broadcasts the message to all GameObjects with an AbstractLinda script attached to it, HEAVY COMPUTATION YOU ARE WARNED.
		/// </summary>
		/// <param name="message">Message to broadcast.</param>
		public static void BroadcastMessage (string message)
		{
			AbstractLinda[] al = UnityEngine.Object.FindObjectsOfType<AbstractLinda> ();
			for (int i = 0, alLength = al.Length; i < alLength; i++) {
				LindaLibrary.Linda_OUT (message, al [i].LocalKB);
			}
			SendMessageToGlobalKB (message);
		}

		/// <summary>
		/// Broadcasts the message to all GameObjects with the specified tag. NB: it will only perform on objects with AbstractLinda script.
		/// Supported tags are: "SpatialTupleSpace" and "Region"
		/// </summary>
		/// <param name="message">Message to broadcast.</param>
		/// <param name="tag">Searched tag.</param>
		public static void BroadcastMessage (string message, string tag)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag (tag);
			for (int i = 0, targetsLength = targets.Length; i < targetsLength; i++) {
				var item = targets [i];
				if (item.GetComponent<AbstractLinda> () != null) {
					LindaLibrary.Linda_OUT (message, LindaLibrary.GetGameobjectLocalKB (item));
				}
			}
		}

		/// <summary>
		/// Asks message to the global KB, not destructive.
		/// </summary>
		/// <returns><c>true</c>, if GlobalKB has the message, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to ask.</param>
		public static bool Ask (string message)
		{
			return LindaLibrary.Linda_RD (message);
		}

		/// <summary>
		/// Ask the specified message to the targeted GameObject (assuming an AbstractLinda script attached to it) if you only know its name, not destructive.
		/// </summary>
		/// <returns><c>true</c>, if the message was present, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to ask.</param>
		/// <param name="name">Target GameObject's name, must have an unique name and an AbstractLinda script.</param>
		public static bool Ask (string message, string name)
		{
			return LindaLibrary.Linda_RD (message, LindaLibrary.GetGameobjectLocalKB (GameObject.Find (name)));
		}

		/// <summary>
		/// Ask the specified message to the targeted GameObject, not destructive.
		/// </summary>
		/// <returns><c>true</c>, if the message was present, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to ask.</param>
		/// <param name="target">Target gameobject.</param>
		public static bool Ask (string message, GameObject target)
		{
			return LindaLibrary.Linda_RD (message, LindaLibrary.GetGameobjectLocalKB (target));
		}

		/// <summary>
		/// Ask the specified message to the GlobalKB, unifying var with the first occurence founded, not destructive..
		/// </summary>
		/// <returns>The variable bound to some value if message was present, false otherwise.</returns>
		/// <param name="var">Variable.</param>
		/// <param name="message">Message.</param>
		public static object AskParam (string var, string message)
		{
			return LindaLibrary.Linda_RD (var, message, null);
		}

		/// <summary>
		/// Ask the specified message to the target's KB, unifying var with the first occurence founded, not destructive.
		/// NB: if there are more than 1 gameobject with the same name, Unity will perform only on one of them, be careful, use different names
		/// </summary>
		/// <returns>The variable bound to some value if message was present, false otherwise.</returns>
		/// <param name="var">Variable.</param>
		/// <param name="message">Message.</param>
		/// <param name="name">Name of the gameobject to perform method.</param>
		public static object AskParam (string var, string message, string name)
		{
			return LindaLibrary.Linda_RD (var, message, LindaLibrary.GetGameobjectLocalKB (GameObject.Find (name)), null);
		}

		/// <summary>
		/// Ask the specified message to the target's KB, unifying var with the first occurence founded, not destructive.
		/// </summary>
		/// <returns>The variable bound to some value if message was present, false otherwise.</returns>
		/// <param name="var">Variable.</param>
		/// <param name="message">Message.</param>
		/// <param name="name">The gameobject to perform method.</param>
		public static object AskParam (string var, string message, GameObject name)
		{
			return LindaLibrary.Linda_RD (var, message, LindaLibrary.GetGameobjectLocalKB (name), null);
		}

		/// <summary>
		/// Asks message to the targeted GameObject, suspending the execution until it succeed, not destructive.
		/// </summary>
		/// <returns>Whether the message is found or not.</returns>
		/// <param name="message">Message to ask.</param>
		/// <param name="name">GameObject name.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		public async static Task<bool> AskSuspend (string message, string name, Component me)
		{
			(me as AbstractLinda).Suspended = true;
			bool x = await LindaLibrary.Linda_RD_SUSP (message, LindaLibrary.GetGameobjectLocalKB (GameObject.Find (name)), me);
			(me as AbstractLinda).Suspended = false;
			return x;
		}

		/// <summary>
		/// Asks message to the targeted GameObject, suspending the execution until it succeed, not destructive.
		/// </summary>
		/// <returns>Whether the message is found or not.</returns>
		/// <param name="message">Message to ask.</param>
		/// <param name="name">GameObject to perform the method.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		public async static Task<bool> AskSuspend (string message, GameObject name, Component me)
		{
			(me as AbstractLinda).Suspended = true;
			bool x = await LindaLibrary.Linda_RD_SUSP (message, LindaLibrary.GetGameobjectLocalKB (name), me);
			(me as AbstractLinda).Suspended = false;
			return x;
		}

		/// <summary>
		/// Coroutine version, asks message to the targeted GameObject, suspending the execution until it succeed, not destructive.
		/// </summary>
		/// <returns>Whether the message is found or not.</returns>
		/// <param name="message">Message to ask.</param>
		/// <param name="target">Target GameObject's name.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		public static IEnumerator AskSuspend_Coroutine (string message, string target, Component me)
		{
			(me as AbstractLinda).Suspended = true;
			yield return LindaLibrary.Linda_RD_SUSP_Coroutine (message, LindaLibrary.GetGameobjectLocalKB (GameObject.Find (target)), me);
			(me as AbstractLinda).Suspended = false;
		}

		/// <summary>
		/// Coroutine version, asks message to the targeted GameObject, suspending the execution until it succeed, not destructive.
		/// </summary>
		/// <returns>Whether the message is found or not.</returns>
		/// <param name="message">Message to ask.</param>
		/// <param name="target">Target GameObject.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		public static IEnumerator AskSuspend_Coroutine (string message, GameObject target, Component me)
		{
			(me as AbstractLinda).Suspended = true;
			yield return LindaLibrary.Linda_RD_SUSP_Coroutine (message, LindaLibrary.GetGameobjectLocalKB (target), me);
			(me as AbstractLinda).Suspended = false;
		}

		/// <summary>
		/// Asks message to all objects in scene with specified tag. NB: it will perform only on AbstractLinda scripts, not destructive.
		/// WARNING: could impact on the simulation performance
		/// </summary>
		/// <returns><c>true</c>, if all was asked, <c>false</c> otherwise.</returns>
		/// <param name="message">Message.</param>
		/// <param name="tag">Tag.</param>
		public static bool AskAll (string message, string tag)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag (tag);
			for (int i = 0, targetsLength = targets.Length; i < targetsLength; i++) {
				var item = targets [i];
				if (item.GetComponent<AbstractLinda> () != null) {
					if (!LindaLibrary.Linda_RD (message, LindaLibrary.GetGameobjectLocalKB (item))) {
						return false;
					}
				}
			}
			return true;
		}

		//		public async static bool AskAllSuspend(string message, Component me){
		//			return await LindaLibrary.Linda_RD_SUSP (message, me);
		//		}

		//		public static IEnumerator AskAllSuspend(string message, Component me){
		//			yield return LindaLibrary.Linda_RD_SUSP_Coroutine (message, me);
		//		}

		/// <summary>
		/// Retrieve the specified message from the GlobalKB, destructive.
		/// </summary>
		/// <returns><c>true</c>, if the message was present, <c>false</c> otherwise.</returns>
		/// <param name="message">Message.</param>
		public static bool Retrieve (string message)
		{
			return LindaLibrary.Linda_IN (message);
		}

		/// <summary>
		/// Retrieve the specified message from targeted GameObject's name if you only know the name, destructive.
		/// </summary>
		/// <returns><c>true</c>, if the message was present, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="name">Target GameObject's name.</param>
		public static bool Retrieve (string message, string name)
		{
			return LindaLibrary.Linda_IN (message, LindaLibrary.GetGameobjectLocalKB (GameObject.Find (name)));
		}

		/// <summary>
		/// Retrieve the specified message from targeted GameObject, destructive.
		/// </summary>
		/// <returns><c>true</c>, if the message was present, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="name">Target GameObject.</param>
		public static bool Retrieve (string message, GameObject name)
		{
			return LindaLibrary.Linda_IN (message, LindaLibrary.GetGameobjectLocalKB (name));
		}

		/// <summary>
		/// Retrieve the specified message to the GlobalKB, unifying var with the first occurence founded, destructive.
		/// </summary>
		/// <returns>The variable bound to some value if message was present, false otherwise.</returns>
		/// <param name="var">Variable.</param>
		/// <param name="message">Message.</param>
		public static object RetrieveParam (string var, string message)
		{
			return LindaLibrary.Linda_IN (var, message, null);
		}

		/// <summary>
		/// Retrieve the specified message to the target's KB, unifying var with the first occurence founded, destructive.
		/// </summary>
		/// <returns>The variable bound to some value if message was present, false otherwise.</returns>
		/// <param name="var">Variable.</param>
		/// <param name="message">Message.</param>
		/// <param name="name">Target gameobject's name.</param>
		public static object RetrieveParam (string var, string message, string name)
		{
			return LindaLibrary.Linda_IN (var, message, LindaLibrary.GetGameobjectLocalKB (GameObject.Find (name)));
		}

		/// <summary>
		/// Retrieve the specified message to the target's KB, unifying var with the first occurence founded, destructive.
		/// </summary>
		/// <returns>The variable bound to some value if message was present, false otherwise.</returns>
		/// <param name="var">Variable.</param>
		/// <param name="message">Message.</param>
		/// <param name="name">Target gameobject.</param>
		public static object RetrieveParam (string var, string message, GameObject name)
		{
			return LindaLibrary.Linda_IN (var, message, LindaLibrary.GetGameobjectLocalKB (name));
		}

		/// <summary>
		/// Retrieves the message on targeted GameObject with suspension semantic, destructive
		/// </summary>
		/// <returns>The result.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="name">Target GameObject's name.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		public async static Task<bool> RetrieveSuspend (string message, string name, Component me)
		{
			(me as AbstractLinda).Suspended = true;
			bool x = await LindaLibrary.Linda_IN_SUSP (message, LindaLibrary.GetGameobjectLocalKB (GameObject.Find (name)), me);
			(me as AbstractLinda).Suspended = false;
			return x;
		}

		/// <summary>
		/// Retrieves the message on targeted GameObject with suspension semantic, destructive
		/// </summary>
		/// <returns>The result.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="name">Target GameObject.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		public async static Task<bool> RetrieveSuspend (string message, GameObject name, Component me)
		{
			(me as AbstractLinda).Suspended = true;
			bool x = await LindaLibrary.Linda_IN_SUSP (message, LindaLibrary.GetGameobjectLocalKB (name), me);
			(me as AbstractLinda).Suspended = false;
			return x;
		}

		/// <summary>
		/// Coroutine version: retrieves the message on targeted GameObject with suspension semantic, destructive
		/// </summary>
		/// <returns>The result.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="name">Target GameObject's name.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		public static IEnumerator RetrieveSuspend_Coroutine (string message, string name, Component me)
		{
			(me as AbstractLinda).Suspended = true;
			yield return LindaLibrary.Linda_IN_SUSP_Coroutine (message, LindaLibrary.GetGameobjectLocalKB (GameObject.Find (name)), me);
			(me as AbstractLinda).Suspended = false;
		}

		/// <summary>
		/// Coroutine version: retrieves the message on targeted GameObject with suspension semantic, destructive
		/// </summary>
		/// <returns>The result.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="name">Target GameObject.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		public static IEnumerator RetrieveSuspend_Coroutine (string message, GameObject name, Component me)
		{
			(me as AbstractLinda).Suspended = true;
			yield return LindaLibrary.Linda_IN_SUSP_Coroutine (message, LindaLibrary.GetGameobjectLocalKB (name), me);
			(me as AbstractLinda).Suspended = false;
		}

		/// <summary>
		/// Retrieves message to all objects in scene with specified tag. NB: it will perform only on AbstractLinda scripts. Destructive.
		/// It will fail if one or more KBs don't have message in it
		/// </summary>
		/// <returns><c>true</c>, if message was present in all KBs, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="tag">Tag of GameObjects to perform the method.</param>
		public static bool RetrieveAll (string message, string tag)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag (tag);
			for (int i = 0, targetsLength = targets.Length; i < targetsLength; i++) {
				var item = targets [i];
				if (item.GetComponent<AbstractLinda> () != null) {
					if (!LindaLibrary.Linda_IN (message, LindaLibrary.GetGameobjectLocalKB (item))) {
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Tell the specified message to the GlobalKB.
		/// </summary>
		/// <param name="message">Message to tell.</param>
		public static bool Tell (string message)
		{
			return LindaLibrary.Linda_OUT (message);
		}

		/// <summary>
		/// Tell the specified message to the target's local KB if you only know the target's name.
		/// </summary>
		/// <param name="message">Message to tell.</param>
		/// <param name="name">Gameobject's name.</param>
		public static bool Tell (string message, string name)
		{
			return LindaLibrary.Linda_OUT (message, LindaLibrary.GetGameobjectLocalKB (GameObject.Find (name)));
		}

		/// <summary>
		/// Tell the specified message to the target's local KB.
		/// </summary>
		/// <param name="message">Message to tell.</param>
		/// <param name="name">Target gameobject..</param>
		public static bool Tell (string message, GameObject name)
		{
			return LindaLibrary.Linda_OUT (message, LindaLibrary.GetGameobjectLocalKB (name));
		}

		/// <summary>
		/// Creates the specified region with message inside it, it will have tag and layer assigned to "Region".
		/// </summary>
		/// <returns><c>true</c>, if region was created, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to send.</param>
		/// <param name="reg">Region to create.</param>
		public static bool SendMessageToRegion (string message, Region reg)
		{
			GameObject region = GameObject.CreatePrimitive (reg.Type);
			region.transform.localPosition = reg.Centre;
			region.transform.localScale = reg.Scale;
			region.name = reg.Name;
			region.tag = "Region";
			region.layer = LayerMask.NameToLayer ("Region");
			region.GetComponent<MeshRenderer> ().material = Resources.Load ("Materials/Transparent") as Material;
			region.GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			if (reg.Type.Equals (PrimitiveType.Capsule) || reg.Type.Equals (PrimitiveType.Cylinder)) {
				region.transform.rotation = reg.Rotation;
			}
			region.GetComponent<Collider> ().isTrigger = reg.IsTrigger;
			SituatedPassiveKB script = region.AddComponent<SituatedPassiveKB> ();
			script.path = "";
			script.InitKB ();
			return LindaLibrary.Linda_OUT (message, script.LocalKB);
		}

		/// <summary>
		/// Creates the specified Region and sends the specified message to the Region itself and to #num GameObjects within Region reg: it only targets objects with the "SpatialTupleSpace" tag.
		/// </summary>
		/// <returns><c>true</c>, if message was sent, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to send.</param>
		/// <param name="reg">Target region.</param>
		/// <param name = "num">Number of GameObjects to be targeted.</param>
		public static bool SendMessageToAllObjectsInRegion (string message, Region reg, int num)
		{
			GameObject region = GameObject.CreatePrimitive (reg.Type);
			region.transform.localPosition = reg.Centre;
			region.transform.localScale = reg.Scale;
			region.name = reg.Name;
			region.tag = "Region";
			region.layer = LayerMask.NameToLayer ("Region");
			region.GetComponent<MeshRenderer> ().material = Resources.Load ("Materials/Transparent") as Material;
			region.GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			if (reg.Type.Equals (PrimitiveType.Capsule) || reg.Type.Equals (PrimitiveType.Cylinder)) {
				region.transform.rotation = reg.Rotation;
			}
			region.GetComponent<Collider> ().isTrigger = reg.IsTrigger;
			SituatedPassiveKB script = region.AddComponent<SituatedPassiveKB> ();
			script.path = "";
			script.InitKB ();
			LindaLibrary.Linda_OUT (message, script.LocalKB);
			GameObject[] objs = GetSituatedObjectsFromArea (reg.Centre, reg.Scale.x, num);
			for (int i = 0; i < objs.Length; i++) {
				if (objs [i].layer.Equals (LayerMask.NameToLayer ("SpatialTupleSpace")) && objs [i].GetComponent<AbstractLinda> () != null) {
					LindaLibrary.Linda_OUT (message,LindaLibrary.GetGameobjectLocalKB (objs[i]));
				}
			}
			return true;

		}
		
		/// <summary>
		/// Sends the specified message to #num GameObjects within a spherical region with the specified centre and radius, not creating it: it only targets objects with the "SpatialTupleSpace" tag.
		/// </summary>
		/// <returns><c>true</c>, if message was sent, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to send.</param>
		/// <param name="centre">Centre location of the region.</param>
		/// <param name="radius">Radius of the region.</param>
		/// <param name = "num">Number of GameObjects to be targeted.</param>
		public static bool SendMessageToAllObjectsInRegion (string message, Vector3 centre, float radius, int num)
		{
			GameObject[] objs = GetSituatedObjectsFromArea (centre, radius, num);
			for (int i = 0; i < objs.Length; i++) {
				if (objs [i].layer.Equals (LayerMask.NameToLayer ("SpatialTupleSpace")) && objs [i].GetComponent<AbstractLinda> () != null) {
					LindaLibrary.Linda_OUT (message,LindaLibrary.GetGameobjectLocalKB (objs[i]));
				}
			}
			return true;

		}

		/// <summary>
		/// Sends message to a region with specified name, assuming that it already exists.
		/// Warning: it will send the message to only one region gameobject, so if there exist multiple regions with the same name, only the first one will receive the message (use SendMessageToAllRegionsName to send message to all regions with a certain name)
		/// </summary>
		/// <returns>ReturnTypeKB.TagProblem if the collider is not with "SpatialTupleSpace" or "Region" tag, ReturnTypeKB.True/False if the message is correctly/!correctly read.</returns>
		/// <param name="message">Message to send.</param>
		/// <param name="name">Name of the existing region.</param>
		public static ReturnTypeKB SendMessageToRegionName (string message, string name)
		{
			try {
				if (GameObject.Find (name).CompareTag("Region")) {
					return LindaLibrary.Linda_OUT (message, LindaLibrary.GetGameobjectLocalKB (GameObject.Find (name))) ? ReturnTypeKB.True : ReturnTypeKB.False;
				} else {
					Debug.LogWarning ("The GameObject with that name is not tagged as Region, unable to send message");
					return ReturnTypeKB.TagProblem;
				}
			} catch (Exception ex) {
				Debug.LogWarning ("Unable to find a gameobject with that name");
				return ReturnTypeKB.NotCreated;
			}
		}

		/// <summary>
		/// Sends the message to all regions with tag "Region".
		/// </summary>
		/// <param name="message">Message.</param>
		public static void SendMessageToAllRegions (string message)
		{
			GameObject[] regions = GameObject.FindGameObjectsWithTag ("Region");
			for (int i = 0, regionsLength = regions.Length; i < regionsLength; i++) {
				LindaLibrary.Linda_OUT (message, LindaLibrary.GetGameobjectLocalKB (regions [i]));
			}

		}

		/// <summary>
		/// Sends the message to all regions with tag "Region" and name "name"
		/// </summary>
		/// <param name="message">Message to send.</param>
		/// <param name = "name">Name of the region.</param>
		public static void SendMessageToAllRegionsName (string message, string name)
		{
			GameObject[] regions = GameObject.FindGameObjectsWithTag ("Region");
			for (int i = 0, regionsLength = regions.Length; i < regionsLength; i++) {
				if (regions[i].name.Equals (name)) {
					LindaLibrary.Linda_OUT (message, LindaLibrary.GetGameobjectLocalKB (regions [i]));
				}
			}
		}

		/// <summary>
		/// Asks the message from region triggered: use this inside OnTriggerEnter method of your agent.
		/// </summary>
		/// <returns>ReturnTypeKB.TagProblem if the collider is not with "SpatialTupleSpace" or "Region" tag, ReturnTypeKB.True/False if the message is correctly/!correctly read.</returns>
		/// <param name="message">Message to ask.</param>
		/// <param name="coll">The encountered collider, assumed to be the same of the OnTriggerEnter one.</param>
		public static ReturnTypeKB AskMessageFromSituatedObjectTriggered (string message, Collider coll)
		{
			if (coll.gameObject.CompareTag ("SpatialTupleSpace") || coll.gameObject.CompareTag ("Region")) {
				return LindaLibrary.Linda_RD (message, LindaLibrary.GetGameobjectLocalKB (coll.gameObject)) ? ReturnTypeKB.True : ReturnTypeKB.False;
			}
			Debug.LogWarning ("The GameObject with that name is not tagged as Region or SpatialTupleSpace, unable to send message");
			return ReturnTypeKB.TagProblem;
		}

		/// <summary>
		/// Retrieves the message from region triggered: use this inside OnTriggerEnter method of your agent.
		/// </summary>
		/// <returns>ReturnTypeKB.TagProblem if the collider is not with "SpatialTupleSpace" or "Region" tag, ReturnTypeKB.True/False if the message is correctly/!correctly read.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="coll">The encountered collider, assumed to be the same of the OnTriggerEnter one.</param>
		public static ReturnTypeKB RetrieveMessageFromSituatedObjectTriggered (string message, Collider coll)
		{
			if (coll.gameObject.CompareTag ("SpatialTupleSpace") || coll.gameObject.CompareTag ("Region")) {
				return LindaLibrary.Linda_IN (message, LindaLibrary.GetGameobjectLocalKB (coll.gameObject)) ? ReturnTypeKB.True : ReturnTypeKB.False;
			}
			Debug.LogWarning ("The GameObject with that name is not tagged as Region or SpatialTupleSpace, unable to send message");
			return ReturnTypeKB.TagProblem;
		}

		/// <summary>
		/// Asks the message from a SituatedObject collided (a region, a SituatedPassiveKB, SituatedKB, ecc...), any GameObject with a collider and
		/// an AbstractLinda script attached.
		/// WARNING: disableCollider=true will disable temporarily all trigger colliders of this gameobject, be aware of that: during the suspension, you have no collider
		/// in order to properly suspend the execution. If you want to be able to continue triggering colliders, set disableCollider to false
		/// </summary>
		/// <returns>ReturnTypeKB.TagProblem if the collider is not with "SpatialTupleSpace" or "Region" tag, ReturnTypeKB.True/False if the message is correctly/!correctly read.</returns>
		/// <param name="message">Message to ask.</param>
		/// <param name="coll">Collider of the region (or whatever with an AbstractLinda script) triggered, meant to be the collider of OnTriggerEnter.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		/// <param name="disableCollider">Decide whether disable the trigger collider on suspensive call or not, true by default because it's a suspensive call; if false, it will only check the suspended var.</param>
		public async static Task<ReturnTypeKB> AskMessageSuspendedFromSituatedObjectTriggered (string message, Collider coll, Component me, bool disableCollider = true)
		{
			if (coll.gameObject.CompareTag ("SpatialTupleSpace") || coll.gameObject.CompareTag ("Region")) {
				Collider[] colls = me.GetComponents<Collider> ().Where (x => x.isTrigger).ToArray ();
				if (disableCollider) {
					for (int i = 0; i < colls.Length; i++) {
						colls [i].enabled = false;
					}	
				}
				//(me as AbstractLinda).Suspended = true;

				var res = await LindaLibrary.Linda_RD_SUSP (message, LindaLibrary.GetGameobjectLocalKB (coll.gameObject), me);

				if (disableCollider) {
					for (int i = 0; i < colls.Length; i++) {
						colls [i].enabled = true;
					}	
				}
				//(me as AbstractLinda).Suspended = false;
				return res ? ReturnTypeKB.True : ReturnTypeKB.False;
			} else {
				return ReturnTypeKB.TagProblem;
			}
		}
		
		/// <summary>
		/// Retrieves the message from a SituatedObject collided (a region, a SituatedPassiveKB, SituatedKB, ecc...), any GameObject with a collider and
		/// an AbstractLinda script attached.
		/// WARNING: disableCollider=true will disable temporarily all trigger colliders of this gameobject, be aware of that: during the suspension, you have no collider
		/// in order to properly suspend the execution. If you want to be able to continue triggering colliders, set disableCollider to false
		/// </summary>
		/// <returns>ReturnTypeKB.TagProblem if the collider is not with "SpatialTupleSpace" or "Region" tag, ReturnTypeKB.True/False if the message is correctly/!correctly read.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="coll">Collider of the region (or whatever with an AbstractLinda script) triggered, meant to be the collider of OnTriggerEnter.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		/// <param name="disableCollider">Decide whether disable the trigger collider on suspensive call or not, true by default because it's a suspensive call; if false, it will only check the suspended var.</param>
		public async static Task<ReturnTypeKB> RetrieveMessageSuspendedFromSituatedObjectTriggered (string message, Collider coll, Component me, bool disableCollider = true)
		{
			if (coll.gameObject.CompareTag ("SpatialTupleSpace") || coll.gameObject.CompareTag ("Region")) {
				Collider[] colls = me.GetComponents<Collider> ().Where (x => x.isTrigger).ToArray ();
				if (disableCollider) {
					for (int i = 0; i < colls.Length; i++) {
						colls [i].enabled = false;
					}	
				}
				//(me as AbstractLinda).Suspended = true;

				var res = await LindaLibrary.Linda_IN_SUSP (message, LindaLibrary.GetGameobjectLocalKB (coll.gameObject), me);
				//Debug.Log ("DONE AWAITING COMMLIBRARY");
				if (disableCollider) {
					for (int i = 0; i < colls.Length; i++) {
						colls [i].enabled = true;
					}	
				}

				//(me as AbstractLinda).Suspended = false;

				return res ? ReturnTypeKB.True : ReturnTypeKB.False;
			} else {
				return ReturnTypeKB.TagProblem;
			}
		}

		/// <summary>
		/// Coroutine version: asks the message from a SituatedObject collided (a region, a SituatedPassiveKB, SituatedKB, ecc...), any GameObject with a collider and
		/// an AbstractLinda script attached.
		/// WARNING: disableCollider=true will disable temporarily all trigger colliders of this gameobject, be aware of that: during the suspension, you have no collider
		/// in order to properly suspend the execution. If you want to be able to continue triggering colliders, set disableCollider to false
		/// </summary>
		/// <returns>Null if the collider is not with "SpatialTupleSpace" or "Region" tag, true/false if the message is correctly/!correctly read.</returns>
		/// <param name="message">Message to read.</param>
		/// <param name="coll">Collider of the region (or whatever with an AbstractLinda script) triggered, meant to be the collider of OnTriggerEnter.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		/// <param name="disableCollider">Decide whether disable the trigger collider on suspensive call or not, true by default because it's a suspensive call; if false, it will only check the suspended var.</param>
		public static IEnumerator AskMessageSuspendedFromSituatedObjectTriggeredCoroutine (string message, Collider coll, Component me, bool disableCollider = true)
		{
			if (coll.gameObject.CompareTag ("SpatialTupleSpace") || coll.gameObject.CompareTag ("Region")) {
				if (disableCollider) {
					me.GetComponent<Collider> ().enabled = false;
				}
				//(me as AbstractLinda).Suspended = true;

				yield return LindaLibrary.Linda_RD_SUSP_Coroutine (message, LindaLibrary.GetGameobjectLocalKB (coll.gameObject), me);

				if (disableCollider) {
					me.GetComponent<Collider> ().enabled = true;	
				}
				//(me as AbstractLinda).Suspended = false;
			}
		}

		/// <summary>
		/// Coroutine version: retrieves the message from a SituatedObject collided (a region, a SituatedPassiveKB, SituatedKB, ecc...), any GameObject with a collider and
		/// an AbstractLinda script attached.
		/// WARNING: disableCollider=true will disable temporarily all trigger colliders of this gameobject, be aware of that: during the suspension, you have no collider
		/// in order to properly suspend the execution. If you want to be able to continue triggering colliders, set disableCollider to false
		/// </summary>
		/// <returns>Null if the collider is not with "SpatialTupleSpace" or "Region" tag, true/false if the message is correctly/!correctly read.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="coll">Collider of the region (or whatever with an AbstractLinda script) triggered, meant to be the collider of OnTriggerEnter.</param>
		/// <param name="me">This component, must be an AbstractLinda script.</param>
		/// <param name="disableCollider">Decide whether disable the trigger collider on suspensive call or not, true by default because it's a suspensive call; if false, it will only check the suspended var.</param>
		public static IEnumerator RetrieveMessageSuspendedFromSituatedObjectTriggeredCoroutine (string message, Collider coll, Component me, bool disableCollider = true)
		{
			if (coll.gameObject.CompareTag ("SpatialTupleSpace") || coll.gameObject.CompareTag ("Region")) {
				if (disableCollider) {
					me.GetComponent<Collider> ().enabled = false;
				}
				//(me as AbstractLinda).Suspended = true;

				yield return LindaLibrary.Linda_IN_SUSP_Coroutine (message, LindaLibrary.GetGameobjectLocalKB (coll.gameObject), me);

				if (disableCollider) {
					me.GetComponent<Collider> ().enabled = true;
				}
				//(me as AbstractLinda).Suspended = false;
			}
		}
		
		/// <summary>
		/// Makes the selected object a situated one, with SpatialTupleSpace tag and layer and with a SituatedPassiveKB script supporting a private KB.
		/// </summary>
		/// <returns><c>true</c>, if object situated was made, <c>false</c> if it already is a situated one.</returns>
		/// <param name="go">Go.</param>
		public static bool MakeObjectSituated(GameObject go){
			if (go.GetComponent<AbstractLinda> () == null) {
				go.tag = "SpatialTupleSpace";
				go.layer = LayerMask.NameToLayer ("SpatialTupleSpace");
				SituatedPassiveKB script = go.AddComponent<SituatedPassiveKB> ();
				script.path = "";
				script.InitKB ();
				return true;
			} else {
				Debug.LogWarning ("GameObject already a situated one, no conversion needed");
				return false;
			}
		}
		/// <summary>
		/// Removes the situatedness from object, destroying its AbstractLinda script, its KB and resetting tag and layer
		/// </summary>
		/// <returns><c>true</c>, if situatedness from object has been removed, <c>false</c> otherwise.</returns>
		/// <param name="go">GameObject target.</param>
		public static bool RemoveSituatednessFromObject(GameObject go){
			if (go.GetComponent<AbstractLinda> () == null) {
				Debug.LogWarning ("GameObject is not a situated one, no remove needed");
				return false;
			} else {
				go.tag = "Untagged";
				go.layer = LayerMask.NameToLayer ("Default");
				UnityEngine.Object.Destroy (go.GetComponent<AbstractLinda> ());
				return true;
			}
		}

		#region BagOfTuples

		/// <summary>
		/// Determines if the gameobject with the specified name is provided with Prolog support.
		/// </summary>
		/// <returns><c>The object</c> if the object has its own KB; otherwise, <c>null</c>.</returns>
		/// <param name="name">Gameobject's name.</param>
		public static GameObject IsObjectSituated(string name){
			AbstractLinda obj = GameObject.Find (name).GetComponent<AbstractLinda> ();
			return obj == null ? null : obj.gameObject;
		}

		/// <summary>
		/// Determines if the targeted gameobject is provided with Prolog support.
		/// </summary>
		/// <returns><c>true</c> if the object has its own KB; otherwise, <c>null</c>.</returns>
		/// <param name="target">Target gameobject.</param>
		public static bool IsObjectSituated(GameObject target){
			AbstractLinda obj = target.GetComponent<AbstractLinda> ();
			return obj != null;
		}

		/// <summary>
		/// Determines if this object is provided with a bag of tuples as a child object, if so it returns it
		/// </summary>
		/// <returns>A reference to BagOfTuples' GameObject if available, null otherwise</returns>
		/// <param name="name">Name of the gameobject to analyze.</param>
		public static GameObject IsObjectWithBagOfTuples(string name){
			Transform[] res = GameObject.Find (name).GetComponentsInChildren<Transform> ();
			if (res == null) {
				Debug.LogWarning ("The object with name "+name+" is not provided with a BagOfTuples or it doesn't exist or it doesn't have children");
				return null;
			}
			//GameObject result = res.ElementAt (0).gameObject;
			int i;
			for (i = 1; i < res.Length; i++) {
				if (res [i].gameObject.CompareTag ("BagOfTuples") && res [i].gameObject.layer.Equals (LayerMask.NameToLayer ("BagOfTuples"))) {
					Debug.LogWarning ("BAGOFTUPLES FOUND " + res [i].gameObject);
					return res [i].gameObject;
				} else {
					Debug.LogWarning ("Child object is not a BagOfTuples " + res [i].gameObject);
				}
			}
			return null;
		}

		/// <summary>
		/// Determines if this gameobject is provided with a bag of tuples as a child object
		/// </summary>
		/// <returns>A reference to BagOfTuples' GameObject if available, null otherwise</returns>
		/// <param name="target">Gameobject to analyze.</param>
		public static GameObject IsObjectWithBagOfTuples(GameObject target){
			Transform[] res = target.GetComponentsInChildren<Transform> ();
			if (res == null) {
				Debug.LogWarning ("The object with name "+target.name+" is not provided with a BagOfTuples or it doesn't exist");
				return null;
			}
			//GameObject result = res.ElementAt (0).gameObject;
			int i;
			for (i = 1; i < res.Length; i++) {
				if (res [i].gameObject.CompareTag ("BagOfTuples") && res [i].gameObject.layer.Equals (LayerMask.NameToLayer ("BagOfTuples"))) {
					Debug.LogWarning ("BAGOFTUPLES FOUND " + res [i].gameObject);
					return res [i].gameObject;
				} else {
					Debug.LogWarning ("Child object is not a BagOfTuples " + res [i].gameObject);
				}
			}
			return null;
		}

		/// <summary>
		/// Attaches the bag of tuples to object named name, providing (if needed) the collider's radius (currently it's an empty object, no shape, only collider).
		/// </summary>
		/// <returns><c>true</c>, if bag of tuples to object was attached, <c>false</c> if parent is not present or if it's impossible to load the prefab.</returns>
		/// <param name="name">Name of the gameobject to be provided with a BagOfTuples.</param>
		/// <param name = "radius">Optional parameter: BagOfTuples' collider radius, if not specified it will be the same as the parent one. NB: 1f = 1unit = 1meter</param>
		public static bool AttachBagOfTuplesToObject(string name, float radius = float.NaN){
			GameObject newObj;
			GameObject parent = GameObject.Find (name);
			if (parent == null) {
				return false;
			}
			if (IsObjectWithBagOfTuples(parent) != null){
				Debug.LogWarning("Object already with a BagOfTuples");
				return false;
			}
			newObj = PrefabUtility.InstantiatePrefab (Resources.Load ("Prefabs/BagOfTuples", typeof(GameObject))) as GameObject;
			if (newObj == null) {
				Debug.LogError ("Wrong prefab");
				return false;
			}
			newObj.transform.position = parent.transform.position;
			newObj.transform.rotation = parent.transform.rotation;
			newObj.transform.parent = parent.transform;
			//newObj.transform.localScale = Vector3.one; //with this setting all collider radius must be recalculate
			SphereCollider cc = newObj.AddComponent<SphereCollider> ();
			cc.isTrigger = true;
			//CONTROLLINO: SE IL PADRE NON HA COLLIDER?
			Bounds b;
			try {
				b = parent.GetComponent<Collider> ().bounds;
			} catch (Exception ex) {
				Debug.LogWarning ("Parent without collider" + parent.transform.localScale.magnitude);
				if (float.IsNaN (radius)) {
					Debug.LogWarning ("No radius provided: setting default one of 5 meters");
					cc.radius = 5f;
				} else {
					Debug.LogWarning ("Radius provided: setting it");
					cc.radius = radius;
				}
				return true;
			}
			if (float.IsNaN (radius)) {
				Debug.LogWarning ("No custom radius detected: setting the parent one " + b.extents.x);
				cc.radius = b.extents.x;
				return true;
			} else {
				Debug.LogWarning ("Setting custom radius");
				cc.radius = radius;
				return true;
			}
			return true;
		}

		/// <summary>
		/// Attaches the bag of tuples to object named name, providing (if needed) the collider's radius (currently it's an empty object, no shape, only collider).
		/// </summary>
		/// <returns><c>true</c>, if bag of tuples to object was attached, <c>false</c> if parent is not present or if it's impossible to load the prefab.</returns>
		/// <param name="parent">The Gameobject to be provided with a BagOfTuples.</param>
		/// <param name = "radius">Optional parameter: BagOfTuples' collider radius, if not specified it will be the same as the parent one. NB: 1f = 1unit = 1meter</param>
		public static bool AttachBagOfTuplesToObject(GameObject parent, float radius = float.NaN){
			GameObject newObj;
			if (parent == null) {
				return false;
			}
			if (IsObjectWithBagOfTuples(parent) != null){
				Debug.LogWarning("Object already with a BagOfTuples");
				return false;
			}
			newObj = PrefabUtility.InstantiatePrefab (Resources.Load ("Prefabs/BagOfTuples", typeof(GameObject))) as GameObject;
			if (newObj == null) {
				Debug.LogError ("Wrong prefab");
			}
			newObj.transform.position = parent.transform.position;
			newObj.transform.rotation = parent.transform.rotation;
			newObj.transform.parent = parent.transform;
			SphereCollider cc = newObj.AddComponent<SphereCollider> ();
			cc.isTrigger = true;
			Bounds b;
			try {
				b = parent.GetComponent<Collider> ().bounds;
			} catch (Exception ex) {
				Debug.LogWarning ("Parent without collider" + parent.transform.localScale.magnitude);
				if (float.IsNaN (radius)) {
					Debug.LogWarning ("No radius provided: setting default one of 5 meters");
					cc.radius = 5f;
				} else {
					Debug.LogWarning ("Radius provided: setting it");
					cc.radius = radius;
				}
				return true;
			}
			if (float.IsNaN (radius)) {
				Debug.LogWarning ("No custom radius detected: setting the parent one");
				cc.radius = b.extents.x;
				return true;
			} else {
				Debug.LogWarning ("Setting custom radius");
				cc.radius = radius;
				return true;
			}
			return true;

		}

		/// <summary>
		/// Tells something to the BagOfTuples attached to a specified parent gameobject.
		/// </summary>
		/// <returns><c>true</c>, if message was correctly told to the bag, <c>false</c> otherwise.</returns>
		/// <param name="message">Message.</param>
		/// <param name="parentName">Parent name.</param>
		public static bool TellToObjectBag(string message, string parentName){
			GameObject r = IsObjectWithBagOfTuples (parentName);
			if (r == null) {
				Debug.LogWarning ("The object with name "+parentName+" is not provided with a BagOfTuples");
			}
			return r != null && Tell (message, r);
		}

		/// <summary>
		/// Tells something to the BagOfTuples attached to a specified parent gameobject.
		/// </summary>
		/// <returns><c>true</c>, if message was correctly told to the bag, <c>false</c> otherwise.</returns>
		/// <param name="message">Message.</param>
		/// <param name="parentObject">Parent gameobject.</param>
		public static bool TellToObjectBag(string message, GameObject parentObject){
			GameObject r = IsObjectWithBagOfTuples (parentObject);
			if (r == null) {
				Debug.LogWarning ("The object with name "+parentObject.name+" is not provided with a BagOfTuples, no actions performed");
			}
			return r != null && Tell (message, r);
		}

		/// <summary>
		/// Asks something to the BagOfTuples attached to a specified parent gameobject.
		/// </summary>
		/// <returns><c>true</c>, if the message was asked, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to ask.</param>
		/// <param name="parentName">Name of the BagOfTuples' parent GameObject.</param>
		public static bool AskToObjectBag(string message, string parentName){
			GameObject r = IsObjectWithBagOfTuples (parentName);
			if (r == null) {
				Debug.LogWarning ("The object with name "+parentName+" is not provided with a BagOfTuples, no actions performed");
			}
			return r != null && Ask (message, r);
		}

		/// <summary>
		/// Asks something to the BagOfTuples attached to a specified parent gameobject.
		/// </summary>
		/// <returns><c>true</c>, if the message was asked, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to ask.</param>
		/// <param name="parentObject">Reference to the BagOfTuples' parent GameObject.</param>
		public static bool AskToObjectBag(string message, GameObject parentObject){
			GameObject r = IsObjectWithBagOfTuples (parentObject);
			if (r == null) {
				Debug.LogWarning ("The object with name "+parentObject.name+" is not provided with a BagOfTuples, no actions performed");
			}
			return r != null && Ask (message, r);
		}

		/// <summary>
		/// Retrieves something to the BagOfTuples attached to a specified parent gameobject.
		/// </summary>
		/// <returns><c>true</c>, if the message was retrieved, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="parentName">Name of the BagOfTuples' parent GameObject.</param>
		public static bool RetrieveFromObjectBag(string message, string parentName){
			GameObject r = IsObjectWithBagOfTuples (parentName);
			if (r == null) {
				Debug.LogWarning ("The object with name "+parentName+" is not provided with a BagOfTuples, no actions performed");
			}
			return r != null && Retrieve (message, r);
		}

		/// <summary>
		/// Retrieves something to the BagOfTuples attached to a specified parent gameobject.
		/// </summary>
		/// <returns><c>true</c>, if the message was retrieved, <c>false</c> otherwise.</returns>
		/// <param name="message">Message to retrieve.</param>
		/// <param name="parentObject">Reference to the BagOfTuples' parent GameObject.</param>
		public static bool RetrieveFromObjectBag(string message, GameObject parentObject){
			GameObject r = IsObjectWithBagOfTuples (parentObject);
			if (r == null) {
				Debug.LogWarning ("The object with name "+parentObject.name+" is not provided with a BagOfTuples, no actions performed");
			}
			return r != null && Retrieve (message, r);
		}

		/// <summary>
		/// Acquires the BagOfTuples GameObject of the targeted one (with name target), attaching it to myself GameObject (usually, myself is yourself)
		/// </summary>
		/// <returns><c>true</c>, if BagOfTuples was correctly acquired, <c>false</c> otherwise.</returns>
		/// <param name="myself">Your GameObject reference.</param>
		/// <param name="target">Name of the targeted GameObject (the parent one).</param>
		public static bool AcquireBagOfTuples(GameObject myself, string target){
			GameObject targ = IsObjectWithBagOfTuples (target);
			if (targ != null) {
				GameObject neww = UnityEngine.Object.Instantiate (targ,myself.transform.position,myself.transform.rotation,myself.transform); 
				neww.GetComponent<AbstractLinda> ().LocalKB = LindaLibrary.GetGameobjectLocalKB (targ);//targ.GetComponent<AbstractLinda> ().LocalKB;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Acquires the BagOfTuples GameObject of the targeted one (with name target), creating a new one attaching it to myself GameObject (usually, myself is yourself)
		/// </summary>
		/// <returns><c>true</c>, if BagOfTuples was correctly acquired as a child GameObject, <c>false</c> otherwise.</returns>
		/// <param name="myself">Your GameObject reference.</param>
		/// <param name="target">Target parent GameObject reference.</param>
		public static bool AcquireBagOfTuples(GameObject myself, GameObject target){
			//verificare che la copia dell'oggetto possieda anche tutte le tuple
			GameObject targ = IsObjectWithBagOfTuples (target);
			if (targ != null) {
				GameObject neww = UnityEngine.Object.Instantiate (IsObjectWithBagOfTuples (target),myself.transform.position,myself.transform.rotation,myself.transform); 
				neww.GetComponent<AbstractLinda> ().LocalKB = LindaLibrary.GetGameobjectLocalKB (targ);//targ.GetComponent<AbstractLinda> ().LocalKB;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Sends your BagOfTuples child GameObject to the targeted one (by name), creating a new one and attaching it to the target GameObject
		/// </summary>
		/// <returns><c>true</c>, if BagOfTuples was correctly sent, <c>false</c> otherwise.</returns>
		/// <param name="bag">BagOfTuples' gameObject reference, the one to be sent.</param>
		/// <param name="name">Name of the targeted parent GameObject, the one who will receive the BagOfTuple child.</param>
		public static bool SendBagOfTuples(GameObject bag, string name){
			//agganciare bag all'oggetto target
			GameObject target = GameObject.Find (name);
			if (target == null) {
				return false;
			}
			GameObject neww = UnityEngine.Object.Instantiate (bag,target.transform.position,target.transform.rotation,target.transform);
			neww.GetComponent<AbstractLinda> ().LocalKB = LindaLibrary.GetGameobjectLocalKB (bag);
			return true;
		}

		/// <summary>
		/// Sends your BagOfTuples child GameObject to the targeted one (by referenced target), creating a new one and attaching it to the target GameObject
		/// </summary>
		/// <returns><c>true</c>, if BagOfTuples was correctly sent, <c>false</c> otherwise.</returns>
		/// <param name="bag">BagOfTuples' gameObject reference, the one to be sent.</param>
		/// <param name="target">The targeted Gameobject, will receive the BagOfTuples object as a child.</param>
		public static bool SendBagOfTuples(GameObject bag, GameObject target){
			//agganciare bag all'oggetto target
			GameObject neww = UnityEngine.Object.Instantiate (bag,target.transform.position,target.transform.rotation,target.transform);
			neww.GetComponent<AbstractLinda> ().LocalKB = LindaLibrary.GetGameobjectLocalKB (bag);
			return true;
		}

		#endregion
	}
}

