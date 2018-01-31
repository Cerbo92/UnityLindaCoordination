using UnityEngine;
using Linda;
using Prolog;

public abstract class AbstractLindaCoroutines : MonoBehaviour {

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
	/// Awakes the agent.
	/// </summary>
	public void awakeAgent(){
		print ("AWAKEN");
		LindaLibraryCoroutines.AwakeAgent (this);
	}

}
