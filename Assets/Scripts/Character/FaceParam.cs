using System;
using UnityEngine;

[Serializable]
public class FaceParam
{
	public FaceType faceType;

	public Vector2 destination;
	public Transform target;

	public FaceParam(FaceType ty, Vector2 v, Transform ta)
	{
		faceType = ty;
		destination = v;
		target = ta;
	}

	public FaceParam(FaceType ty, Vector2 v)
	{
		faceType = ty;
		destination = v;
	}

	public FaceParam(FaceType ty, Transform ta)
	{
		faceType = ty;
		target = ta;
	}

	public FaceParam(FaceType ty)
	{
		faceType = ty;
	}

	public enum FaceType
	{
		ToPosition,
		ToTransform
    }
}