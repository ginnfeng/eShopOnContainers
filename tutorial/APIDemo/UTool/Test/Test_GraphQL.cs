////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 9/11/2020 9:25:52 AM 
// Description: Test_GraphQL.cs  
// Revisions  :            		
// **************************************************************************** 
//https://github.com/graphql-dotnet/graphql-dotnet
//https://www.twblogs.net/a/5ca1b692bd9eee5b1a06b31a
using GraphQL;
using GraphQL.Types;
using GraphQL.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UTDll;
using GraphQL.NewtonsoftJson;
using GraphQL.Resolvers;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace UTool.Test
{
    
    public class Droid
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
    

    public class Query
    {
        [GraphQLMetadata("all_droids")]
        public IEnumerable<Droid> GetDroids()
        {
            return new List<Droid> {
                new Droid { Id = "123", Name = "R1" ,Address="信義路"},
                new Droid { Id = "888", Name = "R888" ,Address="忠孝路"},
                new Droid { Id = "999", Name = null ,Address="仁愛路"}
            };

        }
        [GraphQLMetadata("one_droid")]
        public Droid GetOneDroid(string id)
        {
            //IFieldResolver
            var droids = GetDroids();
            foreach (var it in droids)
            {
                if (it.Id == id) return it;
            }            
            return null;           
        }
    }
    
    
    public class JObjectFieldResolver : IFieldResolver
    {
        
        object IFieldResolver.Resolve(IResolveFieldContext context)
        {
            return new List<Droid> { new Droid { Id = "123", Name = "R1", Address = "YI load" } };      

            //var o = JObject.Parse(" {all_droids:[{'id': '123','name': 'R1','address': '信義路'}]}");
            //var o=JArray.Parse("[{'id': '123','name': 'R1','address': '信義路'}]");
            //return o;
        }
    }
    class Test_GraphQL : UTest
    {
        public Test_GraphQL()
        {
            //
            // TODO: Add constructor logic here
            //      
        }
        [UMethod]
        async public void T_1()
        {// TODO: Add Testing logic here
            var schema1 = Schema.For(@"
              type Droid {
                id: ID
                name: String
                address: String
              }

              type Query {
                all_droids: [Droid]
                one_droid(id: ID): Droid
              }
            ", _ => {
                _.Types.Include<Query>();
            });
            //new DocumentExecuter()
            schema1.Query.Fields.First().Resolver = new JObjectFieldResolver();
            var dw=new DocumentWriter(indent: true);
            var json = await schema1.ExecuteAsync(dw, _ =>
            {
                _.Query = $"{{ all_droids {{ id name address}} }}";
                
            });

            var schema2 = Schema.For(@"
              type Droid {
                id: ID
                name: String!
                address: String
              }

              type Query {
                all_droids: [Droid]
                one_droid(id: ID): Droid
              }
            ", _ => {
                _.Types.Include<Query>();
            });
            //var root = @"{  'data': {'droid': {'id': '999', 'address'='仁愛路'}}}";
            var json2 = await schema2.ExecuteAsync(dw, _ =>
            {
                _.Query = $"{{ one_droid(id: \"999\") {{ id name address}} }}";   
               
                //_.Root = root;

            });
            
        }


        [UMethod]
        public void T_Method()
        {// TODO: Add Testing logic here
            dynamic sampleObject = new ExpandoObject();
            var t=typeof(ExpandoObject);
            
            sampleObject.Id = "123";
            sampleObject.Name = "Feng";
            
            IDictionary<string, object> expando = sampleObject as IDictionary<string, object>;
            expando["Address"] = "YI Road";
            var o=JObject.FromObject(sampleObject);

        }

    }
}



