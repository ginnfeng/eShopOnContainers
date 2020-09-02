////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/8/2009 3:55:43 PM 
// Description: EntityAttribute .cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Text.RegularExpressions;

namespace Common.DataCore
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public sealed class EntityAttribute : System.Attribute
    {
        public EntityAttribute()
            :this(false)
        {            
        }
        public EntityAttribute(bool byIndex)
        {
            BindingByIndex = byIndex;
        }
        /// <summary>
        /// true: Propery與Column依序Binding與,Column Name將被Propery Name替換
        ///       =>此為bug,Type.GetProperties()回傳的不保證順序與class定義一樣
        /// false:Propery將依Propery Name去Binding相同Nam的Column,若找不到將發生Exception
        /// </summary>
        public bool BindingByIndex { get; set; }
        public string Namespace { get; set; }
        public string TableName { get; set; }
        static public EntityAttribute GetEntityAttribute<T>()
        {
            return GetEntityAttribute(typeof(T));
        }
        static public EntityAttribute GetEntityAttribute(Type type)
        {            
            EntityAttribute entityAttribute = defaultEntityAttribute;
            object[] entityAttributes = type.GetCustomAttributes(typeof(EntityAttribute), true);
            return (entityAttributes.Length == 0)
                ? new EntityAttribute() { TableName = tableRegex.Replace(type.Name,"$1"), Namespace = type.Namespace } :
                (EntityAttribute)entityAttributes[0];           
        }

        static private EntityAttribute defaultEntityAttribute = new EntityAttribute();
        private static Regex tableRegex = new Regex("I{0,1}(.{1,})");
    }
}
