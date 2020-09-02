////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 7/28/2009 2:05:55 PM 
// Description: StatementField.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

using Common.Support.ErrorHandling;
using CodeExpression = System.Linq.Dynamic.DynamicExpression;
using Common.DataCore.Error;

namespace Common.DataCore
{

    public class StatementField<T> : ConverterFieldBase
    {
        public StatementField()
        {
            MethodProviderName = defaultMethodProviderName;
        }

        public object Invoke<TParameter>(TParameter api)
           where TParameter : T
        {
            try
            {
                object rlt = null;
                foreach (var proc in CompileExpression((api != null) ? api.GetType(): typeof(TParameter)))
                {
                    rlt = proc.DynamicInvoke(api);
                }
                return rlt;
            }
            catch (Exception error)
            {
                throw new Exception<EntityBindingError>(error) { Reference = Content };
            }

        }

        protected ParameterExpression GetMethodProviderParameterExpression()
        {
            return Expression.Parameter(apiParameterType, MethodProviderName);
        }

        public List<Delegate> CompileExpression()
        {
            return CompileExpression(typeof(T));
        }

        public List<Delegate> CompileExpression(Type parameterType)
        {
            if (compiledExpressions == null || !apiParameterType.Equals(parameterType))
            {
                apiParameterType = parameterType;
                compiledExpressions = new List<Delegate>();
                List<string> expressionStrings = GetExpressionStrings();
                if (expressionStrings.Count != 0)
                {
                    for (int i = 0; i < expressionStrings.Count; i++)
                    {
                        string expString = expressionStrings[i];
                        //if (string.IsNullOrEmpty(expString)) continue;
                        try
                        {
                            if (i != (expressionStrings.Count - 1))
                                compiledExpressions.Add(GenDefaultActionExpression(expString).Compile());
                            else
                                compiledExpressions.Add(GenLambdaExpression(expString).Compile());
                        }
                        catch (Exception error)
                        {
                            throw new Exception<EntityBindingError>(error) { Reference = expString };
                        }
                    }
                }
            }
            return compiledExpressions;
        }
        private List<string> GetExpressionStrings()
        {
            //string apiExp = MethodProviderName + ".";
            List<string> expressionStrings = new List<string>();
            foreach (Match mtch in regex.Matches(base.Content))
            {
                if (mtch.Success)
                    expressionStrings.Add(mtch.Groups[1].Value);
            }
            return expressionStrings;
        }

        protected LambdaExpression GenDefaultActionExpression(string expressionString)
        {
            return CodeExpression.ParseLambda(new ParameterExpression[] { GetMethodProviderParameterExpression() }, null, expressionString);
        }

        protected virtual LambdaExpression GenLambdaExpression(string expressionString)
        {
            return GenDefaultActionExpression(expressionString);
        }

        public string MethodProviderName { get; set; }
        private const string defaultMethodProviderName = "api";
        private Type apiParameterType = typeof(T);
        private List<Delegate> compiledExpressions;
        static private Regex regex = new Regex(@"([^;]{1,})");
    }
}
