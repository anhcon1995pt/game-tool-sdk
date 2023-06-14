
using System;
using System.Linq;
using UnityEngine;

namespace AC.Attribute
{
    /// <summary>
    /// [StringSelector("C", "D", "A", "B")]
    /// [StringSelector(typeof(EnumName))]
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class StringSelectorAttribute : PropertyAttribute
    {
        public string[] arrString;
        public StringSelectorAttribute(params object[] arrString)
        {
            this.arrString = arrString.Select(o=> o.ToString()).ToArray();
        }
        public StringSelectorAttribute(Type enumType)
        {
            if(enumType.IsEnum)
            {
                this.arrString = Enum.GetNames(enumType);
            }
            
        }
        
    }
}

