using UnityEngine;
using System.Collections;
using LindaCoordination;
using System;
using Linda;

public class SpatialTable : AbstractLinda
{
	private GameObject[] objs;

	// Use this for initialization
	void Start ()
	{
		//base.Start ();
		LindaLibrary.SetLocalKB (path,gameObject);
		SphereCollider c = gameObject.GetComponent<SphereCollider>();
		objs = LindaCoordinationUtilities.GetSituatedObjectsFromArea (transform.localPosition, c.bounds.extents.x, 25);
		for (int i = 0; i < objs.Length; i++) {
			if (objs[i].name.Contains("Chop")) {
				string thename = objs [i].name;
				print (thename.Substring (thename.Length - 1));
				print(LindaCoordinationUtilities.Tell ("chop(" + thename.Substring (thename.Length - 1) + ")",gameObject));
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

