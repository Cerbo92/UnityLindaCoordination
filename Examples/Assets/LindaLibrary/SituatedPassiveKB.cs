using Linda;

/// <summary>
/// Situated passive KB: a passive, runtime created object with only Linda support.
/// Use this for objects created on runtime.
/// </summary>
public class SituatedPassiveKB : AbstractLinda
{

	public void InitKB ()
	{
		print ("SITUATEDPASSIVEKB");
		LindaLibrary.SetLocalKB (path, gameObject);
	}
}

