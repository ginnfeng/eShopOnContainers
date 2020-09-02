using System;
using System.Reflection;

namespace Common.Support.Web
{
    public class WSProxy
    {

        internal WSProxy(Assembly _assembly)
        {
            assembly = _assembly;
        }

        public Uri WsdlUri
        {
            get { return wsdlUri; }
            set { wsdlUri = value; }
        }
        public object Invoke(string fullTypeName,string methodName,object [] parameters)
        {            
            object ws = CreateObject(fullTypeName);
            MethodInfo methodInfo=ws.GetType().GetMethod(methodName);    
            return methodInfo.Invoke(ws,parameters);
        }
        
        public object CreateObject(string fullTypeName)
        {
            Type wsType = GetType(fullTypeName);
            object entity = Activator.CreateInstance(wsType);
            if (entity == null) return null;
            return entity;
        }

        public Type GetType(string fullTypeName)
        { //ex:OrderServiceImplService            
            return assembly.GetType(fullTypeName);
        }

        Assembly assembly;
        private Uri wsdlUri;
    }

}
