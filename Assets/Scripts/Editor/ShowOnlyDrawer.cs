using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string strValue;

        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                strValue = property.intValue.ToString();
                break;
            case SerializedPropertyType.Boolean:
                strValue = property.boolValue.ToString();
                break;
            case SerializedPropertyType.Float:
                strValue = property.floatValue.ToString("0.00000");
                break;
            case SerializedPropertyType.String:
                strValue = property.stringValue;
                break;
            default:
                strValue = "(Not Supported)";
                break;
        }

        EditorGUI.LabelField(position, label.text, strValue);
    }
}
