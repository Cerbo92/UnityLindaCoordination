using System;
using UnityEngine;

/// <summary>
/// A class representing a region, to be utilized with LindaCoordinationUtilities methods to create regions with tuple spaces
/// </summary>
public class Region
{
	/// <summary>
	/// The name of the region.
	/// </summary>
	private string name;
	/// <summary>
	/// The centre of the region.
	/// </summary>
	private Vector3 centre;
	/// <summary>
	/// The scale of the region.
	/// </summary>
	private Vector3 scale;
	/// <summary>
	/// The type of the region.
	/// </summary>
	private PrimitiveType type;
	/// <summary>
	/// Is the region with a trigger collider? Use this if you want to deposit items inside it
	/// </summary>
	private bool isTrigger;
	/// <summary>
	/// The rotation of the region (use this only with cilinders or capsules), null by default.
	/// </summary>
	private Quaternion rotation;

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
	/// Gets the centre.
	/// </summary>
	/// <value>The centre.</value>
	public Vector3 Centre {
		get {
			return centre;
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
	/// Gets the type.
	/// </summary>
	/// <value>The type.</value>
	public PrimitiveType Type {
		get {
			return type;
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
	/// Gets the rotation.
	/// </summary>
	/// <value>The rotation.</value>
	public Quaternion Rotation {
		get {
			return rotation;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Region"/> class.
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="centre">Centre.</param>
	/// <param name="scale">Scale.</param>
	/// <param name="type">Type.</param>
	/// <param name="isTrigger">If set to <c>true</c> is trigger (usually set it to false).</param>
	/// <param name="rotation">Rotation, use this only with ciliders or capsules (set it to whatever value if not used).</param>
	public Region (string name, Vector3 centre, Vector3 scale, PrimitiveType type, bool isTrigger, Quaternion rotation)
	{
		this.name = name;
		this.centre = centre;
		this.scale = scale;
		this.type = type;
		this.isTrigger = isTrigger;
		this.rotation = rotation;
	}
}

