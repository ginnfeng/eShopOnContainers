////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 5/16/2011 11:00:48 AM 
// Description: AttbibuteHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Reflection;

namespace Common.Support.Helper
{
    public class AttributeHelper
    {
        static public bool TryGetTypeCustomAttribute<TAttribute>(Type type, out TAttribute attri)          
            where TAttribute : System.Attribute
        {
            attri = null;
            object[] attributes = type.GetCustomAttributes(typeof(TAttribute), true);
            if (attributes.Length != 0)
            {
                attri = (TAttribute)attributes[0];
                return true;
            }
            return false;
        }
        static public bool TryGetCustomAttribute<TMemberInfo, TAttribute>(TMemberInfo memberInfo, out TAttribute attri)
            where TMemberInfo : MemberInfo
            where TAttribute : System.Attribute
        {
            attri = null;           
            object[] attributes = memberInfo.GetCustomAttributes(typeof(TAttribute), true);
            if (attributes.Length != 0)
            {
                attri = (TAttribute)attributes[0];
                return true;
            }
            return false;
        }
    }
}
