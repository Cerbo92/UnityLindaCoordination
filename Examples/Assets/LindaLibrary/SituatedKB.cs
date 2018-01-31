using Linda;

/// <summary>
/// Situated object with only a KB decided in the Editor.
/// Use this script for base objects created in Editor mode, with no complicated scripts, only with a Linda TS.
/// </summary>
public class SituatedKB : AbstractLinda
{
		
	void Start ()
	{
		print ("SITUATEDKB");
		LindaLibrary.SetLocalKB (path, gameObject);
	}
}

