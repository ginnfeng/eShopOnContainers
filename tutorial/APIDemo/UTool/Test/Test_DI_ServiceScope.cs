////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/18/2020 5:35:27 PM 
// Description: Test_DI_ServiceScope.cs  
// Revisions  :            		
// **************************************************************************** 
// https://csharpkh.blogspot.com/2019/06/NET-Core-DI-Dependency-Injection-ServiceProvider-AddScoped-CreateScope-DependencyInjection.html
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UTDll;
namespace UTool.Test
{
    
    class Test_DI_ServiceScope : UTest
    {
        public Test_DI_ServiceScope()
        {
            //
            // TODO: Add constructor logic here
            //   
            serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IMessage, ConsoleMessage>();
            //serviceCollection.AddScoped<IMessage, ConsoleMessage>();
            //serviceCollection.AddSingleton<IMessage, ConsoleMessage>();
        }
        private IServiceCollection serviceCollection;
        [UMethod]
        public void T1()
        {// TODO: Add Testing logic here
            
            var sp1 = serviceCollection.BuildServiceProvider();
            var msg1 = sp1.GetService<IMessage>();

            msg1.Write("MSG-1");
            var msg2 = sp1.GetService<IMessage>();

            msg2.Write("MSG-2");
            Debug.WriteLine("END!");
            GC.Collect(2);
        }
        [UMethod]
        public void T2()
        {// TODO: Add Testing logic here

            var sp1 = serviceCollection.BuildServiceProvider();
            var scopeFactory = sp1.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var msg1 = scope.ServiceProvider.GetService<IMessage>();
                msg1.Write("MSG-1");
                var msg2 = scope.ServiceProvider.GetService<IMessage>();
                msg2.Write("MSG-2");
            }
            using (var scope = scopeFactory.CreateScope())
            {
                var msg3 = scope.ServiceProvider.GetService<IMessage>();
                msg3.Write("MSG-3");
                var msg4 = scope.ServiceProvider.GetService<IMessage>();
                msg4.Write("MSG-4");
            }
            Debug.WriteLine("END!");
            GC.Collect(2);
        }
    }
}
