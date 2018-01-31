using System;
using UnityEngine;

/// <summary>
/// Class representing a location with a spatial tuple space, to be utilized with LindaCommunicationUtilities methods.
/// </summary>
public class SpatialTupleSpace
{
	/// <summary>
	/// The name.
	/// </summary>
	private string name;
	/// <summary>
	/// The location.
	/// </summary>
	private Vector3 location;
	/// <summary>
	/// Is the object invisible?
	/// </summary>
	private bool invisible;
	/// <summary>
	/// Is the object trigger? (Usually no) Use trigger if you want the space to be 
	/// </summary>
	private bool isTrigger;
	/// <summary>
	/// The scale.
	/// </summary>
	private Vector3 scale;
	/// <summary>
	/// Is the object with a rigidbody?
	/// </summary>
	private bool isRigid;
	/// <summary>
	/// The Collider scale.
	/// </summary>
	private Vector3 colliderScale;

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>The name.</value>
	public string Name {
		get {
			return name;
		}
	}

	/// <summary>
	/// Gets the location.
	/// </summary>
	/// <value>The location.</value>
	public Vector3 Location {
		get {
			return location;
		}
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref="SpatialTupleSpace"/> is invisible.
	/// </summary>
	/// <value><c>true</c> if invisible; otherwise, <c>false</c>.</value>
	public bool Invisible {
		get {
			return invisible;
		}
	}

	/// <summary>
	/// Gets a value indicating whether this instance is trigger.
	/// </summary>
	/// <value><c>true</c> if this instance is trigger; otherwise, <c>false</c>.</value>
	public bool IsTrigger {
		get {
			return isTrigger;
		}
	}

	/// <summary>
	/// Gets the scale.
	/// </summary>
	/// <value>The scale.</value>
	public Vector3 Scale {
		get {
			return scale;
		}
	}

	/// <summary>
	/// Gets a value indicating whether this instance is rigid.
	/// </summary>
	/// <value><c>true</c> if this instance is rigid; otherwise, <c>false</c>.</value>
	public bool IsRigid {
		get {
			return isRigid;
		}
	}
	
	/// <summary>
	/// Gets the collider scale.
	/// </summary>
	/// <value>The location.</value>
	public Vector3 ColliderScale {
		get {
			return colliderScale;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SpatialTupleSpace"/> class.
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="location">Location.</param>
	/// <param name="invisible">If set to <c>true</c> invisible.</param>
	/// <param name="isTrigger">If set to <c>true</c> is trigger (usually is false).</param>
	/// <param name="scale">Scale.</param>
	/// <param name="isRigid">If set to <c>true</c> is rigid.</param>
	public SpatialTupleSpace(string name, Vector3 location, bool invisible, bool isTrigger, Vector3 scale, bool isRigid){
		this.name = name;
		this.location = location;
		this.invisible = invisible;
		this.isTrigger = isTrigger;
		this.scale = scale;
		this.isRigid = isRigid;
		this.colliderScale = Vector3.negativeInfinity;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SpatialTupleSpace"/> class.
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="location">Location.</param>
	/// <param name="invisible">If set to <c>true</c> invisible.</param>
	/// <param name="scale">Scale.</param>
	/// <param name="isRigid">If set to <c>true</c> is rigid.</param>
	public SpatialTupleSpace(string name, Vector3 location, bool invisible, Vector3 scale, bool isRigid){
		this.name = name;
		this.location = location;
		this.invisible = invisible;
		this.scale = scale;
		this.isRigid = isRigid;
		this.colliderScale = Vector3.negativeInfinity;
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="SpatialTupleSpace"/> class.
	/// </summary>
	/// <param name="name">Name of the GameObject.</param>
	/// <param name="location">Location of the object.</param>
	/// <param name="invisible">If set to <c>true</c> invisible.</param>
	/// <param name="isTrigger">If set to <c>true</c> is trigger (usually is false).</param>
	/// <param name="scale">Scale of the object.</param>
	/// <param name="isRigid">If set to <c>true</c> is rigid.</param>
	/// <param name = "colliderScale">Scale of the collider.</param>
	public SpatialTupleSpace(string name, Vector3 location, bool invisible, bool isTrigger, Vector3 scale, bool isRigid, Vector3 colliderScale){
		this.name = name;
		this.location = location;
		this.invisible = invisible;
		this.isTrigger = isTrigger;
		this.scale = scale;
		this.isRigid = isRigid;
		this.colliderScale = colliderScale;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SpatialTupleSpace"/> class.
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="location">Location.</param>
	/// <param name="invisible">If set to <c>true</c> invisible.</param>
	/// <param name="scale">Scale.</param>
	/// <param name="isRigid">If set to <c>true</c> is rigid.</param>
	/// <param name = "colliderScale">Scale of the collider.</param>
	public SpatialTupleSpace(string name, Vector3 location, bool invisible, Vector3 scale, bool isRigid, Vector3 colliderScale){
		this.name = name;
		this.location = location;
		this.invisible = invisible;
		this.scale = scale;
		this.isRigid = isRigid;
		this.colliderScale = colliderScale;
	}
}

