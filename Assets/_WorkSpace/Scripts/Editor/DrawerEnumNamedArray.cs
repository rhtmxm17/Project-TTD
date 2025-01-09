using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumNamedArrayAttribute))]
public class DrawerEnumNamedArray : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnumNamedArrayAttribute enumNames = attribute as EnumNamedArrayAttribute;

        //propertyPath returns something like component_hp_max.Array.data[4]
        //so get the index from there
        int index = System.Convert.ToInt32(property.propertyPath.Substring(property.propertyPath.IndexOf("[")).Replace("[", "").Replace("]", ""));

        if (index < enumNames.names.Length)
        {
            //change the label
            label.text = enumNames.names[index];
        }
        else
        {
            // enum 값이 지정되지 않은 경우
            label.text = $"_{index}(undef enum)";
        }

        //draw field
        EditorGUI.PropertyField(position, property, label, true);

    }
}