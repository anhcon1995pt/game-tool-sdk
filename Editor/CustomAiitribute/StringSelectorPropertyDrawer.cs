
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AC.Attribute
{
    [CustomPropertyDrawer(typeof(StringSelectorAttribute))]
    public class StringSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                var attrib = this.attribute as StringSelectorAttribute;


                List<string> strList = new List<string>();
                strList.AddRange(attrib.arrString);
                string propertyString = property.stringValue;
                int index = 0;
                if (propertyString == "")
                {
                    //The String is empty
                    index = -1; //first index is entry
                }
                else
                {
                    for (int i = 0; i < strList.Count; i++)
                    {
                        if (strList[i] == propertyString)
                        {
                            index = i;
                            break;
                        }
                    }
                }

                //Draw the popup box with the current selected index
                index = EditorGUI.Popup(position, label.text, index, strList.ToArray());

                //Adjust the actual string value of the property based on the selection
                if (index >= 0 && index < strList.Count)
                {
                    property.stringValue = strList[index];
                }
                else
                {
                    property.stringValue = "";
                }


                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }

}
