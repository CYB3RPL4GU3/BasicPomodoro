using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BasicPomodoro.Common
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Method that gets the Description attribute of a given enumeration value.
        /// </summary>
        /// <param name="enumerationValue">The enumeration value.</param>
        /// <returns>The contents of the Description attribute, or the string literal if such attribute is not defined.</returns>
        public static string GetEnumValueDescription(this Enum enumerationValue)
        {
            Type type = enumerationValue.GetType();

            MemberInfo[] members = type.GetMember(enumerationValue.ToString());
            if (members is not null && members.Length > 0)
            {
                object[] attributes = members[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes is not null && attributes.Length > 0)
                {
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }
    }
}
