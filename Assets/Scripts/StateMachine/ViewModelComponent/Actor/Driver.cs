using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

public class Driver : MonoBehaviour 
{
	public EnumDrivers Normal;
	public EnumDrivers Special;

	public EnumDrivers Current
	{
		get
		{
			return Special != EnumDrivers.None ? Special : Normal;
		}
	}
}