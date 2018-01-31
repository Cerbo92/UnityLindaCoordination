using System;
/// <summary>
/// Custom return type in order to deal with KBs and Linda libraries.
/// </summary>
public enum ReturnTypeKB
{
	/// <summary>
	/// True value (e.g. tuple present, tuple added, request correctly done).
	/// </summary>
	True,
	/// <summary>
	/// False value (e.g. tuple not present, tuple not added, request not correctly done)
	/// </summary>
	False,
	/// <summary>
	/// KB not present or null
	/// </summary>
	NotPresent,
	/// <summary>
	/// KB not created or null
	/// </summary>
	NotCreated,
	/// <summary>
	/// Tag problem (wrong, not inserted, not supported)
	/// </summary>
	TagProblem
}

