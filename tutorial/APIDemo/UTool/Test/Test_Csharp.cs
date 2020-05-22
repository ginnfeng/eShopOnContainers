////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/22/2020 9:29:19 AM 
// Description: Test_Csharp.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UTDll;
namespace UTool.Test
{
    
    class Test_Csharp : UTest
    {
        public Test_Csharp()
        {
            //
            // TODO: Add constructor logic here
            //      
        }
        private void BuildInstanceFactortry(int impIdx)
        {
            if (factory == null)
            {
                factory = new InstanceFactortry();
                factory.Register<IDemoSvc, DemoSvc>(factory => new DemoSvc(factory.CreateInstance<IDemoImp>()));
            }
            if (impIdx == 1)
                factory.Register<IDemoImp, DemoImp1>(factory => new DemoImp1() { PreString = "=>" });            
            else 
                factory.Register<IDemoImp, DemoImp2>(factory => new DemoImp2() { PostString = "<=" });
        }
        [UMethod]
        public void T_FactoryPattern(string s,int impIdx)
        {// TODO: Add Testing logic here
            BuildInstanceFactortry(impIdx);
            var svc = factory.CreateInstance<IDemoSvc>();
            print(svc.Echo(s));
        }
        private InstanceFactortry factory;
    }
}
