using UnityEngine;
using Linda;
using Prolog;

public class AbstractLinda : MonoBehaviour {

	/// <summary>
	/// Path to KB, useful only for SituatedKB scripts.
	/// </summary>
	public string path = "blabla.prolog";
	/// <summary>
	/// Bool value representing whether the gameobject is suspended in some suspensive Linda call or not.
	/// </summary>
	bool suspended = false;

	/// <summary>
	/// The local KB, can be exchanged among GOs.
	/// </summary>
	private KnowledgeBase localKB = new KnowledgeBase("",null,null);

	/// <summary>
	/// Gets or sets the local KB.
	/// </summary>
	/// <value>The local KB.</value>
	public KnowledgeBase LocalKB {
		get {
			return localKB;
		}
		set {
			localKB = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="AbstractLinda"/> is suspended.
	/// </summary>
	/// <value><c>true</c> if suspended; otherwise, <c>false</c>.</value>
	public bool Suspended {
		get {
			return suspended;
		}
		set {
			suspended = value;
		}
	}

//	protected virtual void Start(){
//		print ("LALALA");
//		LindaLibrary.SetLocalKB (path, gameObject);
//	}

	//trying to control here if the sub-class is suspended or is enabled (base.OnTriggerEnter() on sub-class overridden method does not work), luck if the method is not an OnTrigger* method (otherwise it will be called on every
	//script extending AbstractLinda)
	protected bool enabledOrSuspensionCheck(Collider coll){
		if (!enabled || suspended) {
			print (coll + " SUSPENDED: " + suspended + ", ENABLED: " + enabled);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Awakes the agent suspended on some tuple.
	/// </summary>
	public void awakeAgent(){
		print ("AWAKEN - " + suspended);
		if (suspended) {
			LindaLibrary.AwakeAgent (this);
			//suspended = false;
		}
	}

}
