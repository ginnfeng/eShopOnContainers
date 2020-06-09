////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/8/2009 5:51:42 PM 
// Description: LogTraceSource.cs  
// Revisions  :            	
// If you want to trace in the same trace source as WCF you can use this Example in Damir Dobric Blog. 
//   But if you use the WCF Configuration Editor to generate the System.Diagnostic section in the configuration file, 
//   this call will fail with the error “'propagateActivity' is not a valid configuration attribute for type 'System.Diagnostics.TraceSource'.” The Problem is that WCF uses his own TraceSource implemention, which has additional attributes, as the “propagateActivity”.	
// **************************************************************************** 
using System;
using System.Diagnostics;

namespace Support.Log
{
    internal class LogTraceSource : TraceSource
    {
        /// <summary>
        /// Create the WCF Trace Source.
        /// </summary>
        public LogTraceSource()
            : base("System.ServiceModel")
        {
        }
        public LogTraceSource(string name, SourceLevels level)
            : base(name, level)
        {
        }

        /// <summary>
        /// Gets the supported attributes from WCF
        /// "propagateActivity" and "logKnownPii"
        /// </summary>
        /// <returns>Returns the supported attributes.</returns>
        protected override string[] GetSupportedAttributes()
        {
            string[] supportedAttributes = base.GetSupportedAttributes();
            if (supportedAttributes == null)
                return new string[] { "propagateActivity"};

            string[] newSupportedAttributes =
                  new string[supportedAttributes.Length + 1];
            Array.Copy(
                 supportedAttributes,
                 newSupportedAttributes,
                 supportedAttributes.Length);
            newSupportedAttributes[supportedAttributes.Length - 1] =
                "propagateActivity";
            
            return newSupportedAttributes;
        }
    }
}
