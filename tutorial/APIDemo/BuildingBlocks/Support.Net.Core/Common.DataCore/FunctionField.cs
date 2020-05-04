////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/24/2009 5:08:26 PM 
// Description: FunctionField.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Linq.Expressions;
using CodeExpression = System.Linq.Dynamic.DynamicExpression;

namespace Common.DataCore
{
    public class FunctionField<T, TResult> : StatementField<T>
    {
        public FunctionField()
        {
            DefaultResult = default(TResult);
        }

        public TResult DefaultResult { get; set; }

        public TResult Exec<TParameter>(TParameter api)
            where TParameter : T
        {
            return (TResult)base.Invoke(api); ;
        }


        protected override LambdaExpression GenLambdaExpression(string expressionString)
        {
            return CodeExpression.ParseLambda(new ParameterExpression[] { base.GetMethodProviderParameterExpression() }, typeof(TResult), string.IsNullOrEmpty(expressionString) ? DefaultResult.ToString() : expressionString);
        }

    }
    //public class FunctionField<TResult> : FunctionField<IEntityAccess, TResult>
    public class FunctionField<TResult> : FunctionField<object, TResult>
    {
    }
}
