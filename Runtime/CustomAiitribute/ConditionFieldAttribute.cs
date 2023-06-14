using System;
using System.Linq;
using UnityEngine;
namespace AC.Attribute
{
   /// <summary>
   /// Hiển thì Field trên Inspector theo điều kiện
   /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ConditionFieldAttribute : PropertyAttribute
    {
        public readonly string[] FieldToCheck;
        public readonly string[] CompareValues;
        public readonly bool Inverse;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldToCheck">Tên của Field để kiểm tra điều kiện</param>
        /// <param name="inverse">Có đảo ngược kết quả kiểm tra hay không?</param>
        /// <param name="compareValues">Danh sách các kết quả cần kiểm tra</param>
        public ConditionFieldAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues)
        {
            FieldToCheck = fieldToCheck.Split(',').Select(e => e.Trim()).ToArray();
            Inverse = inverse;
            CompareValues = compareValues.Select(c => c.ToString().ToUpper()).ToArray();
        }
    }
}

