using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FaceToTargetClip))]
public class FaceParamInspector : Editor
{
	private SerializedProperty commandProp;
	private int typeIndex;

	private void OnEnable()
	{
		SceneView.onSceneGUIDelegate += OnSceneGUI;
		commandProp = serializedObject.FindProperty("faceType");
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(commandProp);

		typeIndex = serializedObject.FindProperty("faceType").enumValueIndex;
        FaceParam.FaceType commandType = (FaceParam.FaceType)typeIndex;

		//Draws only the appropriate information based on the Command Type
		switch(commandType)
		{
			case FaceParam.FaceType.ToPosition:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("targetPosition")); //position
				break;

			case FaceParam.FaceType.ToTransform:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("targetTransform")); //Unit to attack
				break;
        }

		serializedObject.ApplyModifiedProperties();
	}

	private void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}


	//Draws a position handle on the position associated with the AICommand
	//the handle can be moved to reposition the targetPosition property
	private void OnSceneGUI(SceneView v)
	{
		if((FaceParam.FaceType)typeIndex == FaceParam.FaceType.ToPosition)
		{
			EditorGUI.BeginChangeCheck();
			Vector2 gizmoPos = Handles.PositionHandle(serializedObject.FindProperty("targetPosition").vector2Value, Quaternion.identity);
			
			if(EditorGUI.EndChangeCheck())
			{
				serializedObject.FindProperty("targetPosition").vector2Value = gizmoPos;
				serializedObject.ApplyModifiedProperties();
				
				Repaint();
			}
		}
	}
}
