using UnityEngine;
using com.tksr.statemachine.defines;

public class Alliance : MonoBehaviour
{
	public EnumAlliances type;
	public bool confused;

	public bool IsMatch(Alliance other, EnumTargets targets)
	{
		bool isMatch = false;
		switch (targets)
		{
		case EnumTargets.Self:
			isMatch = other == this;
			break;
		case EnumTargets.Ally:
			isMatch = type == other.type;
			break;
		case EnumTargets.Foe:
			isMatch = (type != other.type) && other.type != EnumAlliances.Neutral;
			break;
		}
		return confused ? !isMatch : isMatch;
	}
}