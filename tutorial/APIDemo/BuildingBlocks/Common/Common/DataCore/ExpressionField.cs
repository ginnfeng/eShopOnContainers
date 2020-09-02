////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/24/2009 1:24:10 PM 
// Description: DataCondField.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

using System.Linq.Expressions;


namespace Common.DataCore
{

    public abstract class ExpressionField<T> : ConverterFieldBase 
    {
        public ExpressionField()
        {
        }

        protected object DoExec<TParameter>(TParameter api)
           where TParameter : T
        {
            Type parameterType = api.GetType();
            return CompileExpression(parameterType).DynamicInvoke(api);
        }

        protected ParameterExpression GetMethodProviderParameterExpression()
        {
            return Expression.Parameter(apiParameterType, methodProviderName);
        }
        internal Delegate CompileExpression()
        {
            return CompileExpression(typeof(T));
        }
        internal Delegate CompileExpression(Type parameterType)
        {
            if (compiledExpression == null || !apiParameterType.Equals(parameterType))
            {
                apiParameterType = parameterType;
                compiledExpression = GenLambdaExpression().Compile();
            }
            return compiledExpression;
        }

        protected abstract LambdaExpression GenLambdaExpression();

        private Delegate compiledExpression;
        private const string methodProviderName = "api";
        Type apiParameterType = typeof(T);
    }
}
