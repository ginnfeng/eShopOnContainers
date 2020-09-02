using System;
using System.Collections.Generic;

using System.Reflection;

namespace Common.Support.Helper
{
    /*
            string answer="客戶證號";
            //old method
            string enumstringvalue=((QueryProductOrderEunm)eh.ParseFromNaming(answer)).ToString();
            //new method
            string enumstringvalue2=eh.Parse(  eh.ParseFromNaming(answer)).ToString();

           補上EnumHelper使用方法：
                        //載入要使用的enum
            EnumHelper eh = new EnumHelper(typeof(QueryProductOrderEunm));
                        //把enum中的value轉成中文，可以讓您塞入combobox中
            string Enumchinese = eh.TranslateAttribute(QueryProductOrderEunm.XOrderNo).ToString();
                        //中文轉enum字串，提供前端丟給OH
            string answer="客戶證號";
            string enumstringvalue=((QueryProductOrderEunm)eh.ParseFromNaming(answer)).ToString();
            //enum字串轉enum value，可供OH判斷是哪一種enum
QueryProductOrderEunm enumvalue = (QueryProductOrderEunm)eh.Parse(enumstringvalue);

//下面是switch case範例！
            QueryProductOrderEunm idx=(QueryProductOrderEunm)eh.ParseFromNaming(answer);
            switch(idx )
            {
                case QueryProductOrderEunm.XOrderNo:
                    break;
                case QueryProductOrderEunm.BopsNo:
                    break;
                case QueryProductOrderEunm.TopsNo:
                    break;
                case QueryProductOrderEunm.MbmsNo:
                    break;
                case QueryProductOrderEunm.SingleEquipmentNo:
                    break;
                case QueryProductOrderEunm.CustomerID:
                    break;
                default:
                    break;
            }

      

     
     */

    public class EnumHelper<TEnum>:EnumHelper
    {
        public EnumHelper()
            :base(typeof(TEnum))
        {
        }
        public NamingAttribute GetNamingAttribute(TEnum value)
        {
            NamingAttribute namingAttribute;
            base.TryGetNamingAttributeByValue(value,out  namingAttribute);
            return namingAttribute;
        }
        public NamingAttribute GetNamingAttribute(string valueStringName)
        {
            return GetNamingAttribute((TEnum)base.Parse(valueStringName));
        }
        
 
    }

    public class EnumHelper
    {
        public EnumHelper(Type enumType)
        {
            EnumType = enumType;
        }
        public string GetName(object enumValue)
        {
            return Enum.GetName(enumType, enumValue);
        }        

        public object Parse(string enumName)
        {
            return Enum.Parse(this.EnumType, enumName);

        }
        public object Parse(object enumName)
        {
            return Enum.Parse(this.EnumType, GetName(enumName));

        }
        public bool TryParseConds(int orBits, out List<int> matchs)
        {
            var enumValues = Enum.GetValues(EnumType);
            matchs = new List<int>();
            foreach (var item in enumValues)
            {
                int v = (int)item;
                if (v!=0 && v == (orBits & v))
                    matchs.Add(v);
            }
            return matchs.Count > 0;

        }

        public bool TryGetNamingAttributeByValue(object enumValue, out NamingAttribute namingAttribute)
        {
            return namingMap.TryGetValue(enumValue, out namingAttribute);

            //return false;

        }
        public bool TryGetNamingAttributeByName(string enumName, out NamingAttribute namingAttribute)
        {
            //  namingAttribute = null;
            return TryGetNamingAttributeByValue(Parse(enumName), out namingAttribute);

        }
        public object TranslateAttribute(object enumValue)
        {
            foreach (object key in namingMap.Keys)
            {
                NamingAttribute temp = new NamingAttribute();
                namingMap.TryGetValue(key, out temp);
               if(GetName(key).Equals(GetName(enumValue))) {
                    return temp.Presentation;
                }
                
            }
            return null;


        }
        public object ParseFromNaming(string presentation)
        {
            foreach (object key in namingMap.Keys)
            {
                NamingAttribute temp = new NamingAttribute();
                namingMap.TryGetValue(key, out temp);
                if (temp.Presentation.Equals(presentation))
                {
                    return key;
                }
            }
            return null;
        }
        public Type EnumType
        {
            get
            {
                return enumType;
            }
            set
            {
                if (value != enumType)
                    BuildNamingMap(value);
            }

        }
        private void BuildNamingMap(Type enumTypeParam)
        {
            this.enumType = enumTypeParam;

            FieldInfo[] fieldInfos = EnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                object[] atributes = fieldInfo.GetCustomAttributes(typeof(NamingAttribute), false);
                NamingAttribute namingAttribute = null;
                if (atributes.Length == 0)
                {
                    namingAttribute = new NamingAttribute();
                }
                else
                {
                    namingAttribute = (NamingAttribute)atributes[0];
                }
                namingMap.Add(Parse(fieldInfo.Name), namingAttribute);
            }

        }
        private Type enumType;
        private Dictionary<object, NamingAttribute> namingMap = new Dictionary<object, NamingAttribute>();
    }

}
