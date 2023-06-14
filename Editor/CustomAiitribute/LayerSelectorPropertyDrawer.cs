using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AC.Attribute
{
    [CustomPropertyDrawer(typeof(LayerSelectorAttribute))]
    public class LayerSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.BeginProperty(position, label, property);
                property.intValue = EditorGUI.LayerField(position, label, property.intValue);
                EditorGUI.EndProperty();
            }
            else
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);
                int layerSelect = LayerMask.NameToLayer(property.stringValue);
                layerSelect = layerSelect < 0 ? 0 : layerSelect;
                layerSelect = EditorGUI.LayerField(position, label, layerSelect);
                property.stringValue = LayerMask.LayerToName(layerSelect);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}

