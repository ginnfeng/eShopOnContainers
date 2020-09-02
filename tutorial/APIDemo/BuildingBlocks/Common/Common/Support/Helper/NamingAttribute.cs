using System;
using System.Reflection;

namespace Common.Support.Helper
{

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class IndexingAttribute : System.Attribute
    {
        public IndexingAttribute() { Idx = -1; }
        public IndexingAttribute(int idx,string id=null)
        {
            Idx = idx;
            Id = id;
        }
        public IndexingAttribute(string id, int idx=-1)
        {
            Id = id;
            Idx = idx;
        }
        /// <summary>
        /// Zero-based idx
        /// </summary>
        public int Idx { get; set; }
        public string StringIdx {
            get
            {
                
                if (Idx < 26)
                    return Convert.ToString((char)(Idx+'A'));
                if (Idx < 52)
                {
                    return "A"+Convert.ToString((char)(Idx-26 + 'A'));
                }
                throw new NotImplementedException("IndexingAttribute:StringIdx");
            }        
            set {
                switch (value.Length)
                {
                    case 1:
                        Idx = (int)(value[0] - 'A');
                        break;
                    case 2:
                        //A~Z ... AA~AZ
                        Idx = ((int)(value[0] - 'A') + 1) * 26 + (int)(value[1] - 'A');
                        break;
                    default:
                        throw new NotImplementedException("IndexingAttribute:StringIdx");
                }
                
            }
        }
        public string Id { get; set; }
        public PropertyInfo OriginalPropertyInfo { get; set; }
        public bool IsIdxAssigned() { return Idx > -1; }
    }
    [Serializable]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class NamingAttribute : IndexingAttribute
    {   
        public string Presentation { get; set; }        
        public bool IsReadOnly { get; set; }        
        public bool IsVisible { get; set; }
        public string Description { get; set; }        
        public object Catagory { get; set; } 
    }
}
