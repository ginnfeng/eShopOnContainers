#region Copyright (C) 2007 IgorM

/* 
 *	Copyright (C) 2005-2007 Igor Moochnick
 *	http://igorshare.wordpress.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 * 
 *  The code originally developed by IgorM (C) 2007
 *
 */

#endregion

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Common.Support.Net.CodeDom
{
    public class CodeDomHelper
    {
        private List<CodeCompileUnit> listCompileUnits = new List<CodeCompileUnit>();
        private List<CodeNamespace> listNamespaces = new List<CodeNamespace>();
        private System.Collections.Specialized.StringCollection listReferencedAssemblies = 
            new System.Collections.Specialized.StringCollection() { "System.dll" };

        public enum Language { CSharp, VP };

        private Language _language;

        public CodeDomHelper()
            : this(Language.CSharp)
        {
        }

        private CodeDomHelper(Language language)
        {
            _language = language;
        }

        public static CodeDomProvider Provider(Language provider)
        {
            var providerOptions = new Dictionary<string, string>(); providerOptions.Add("CompilerVersion", "v3.5");

            switch (provider)
            {
                case Language.VP:
                    return new Microsoft.VisualBasic.VBCodeProvider(providerOptions);

                case Language.CSharp:
                default:
                    return new Microsoft.CSharp.CSharpCodeProvider(providerOptions);
            }
        }

        public CodeNamespace AddNamespace(string namespaceName)
        {
            CodeNamespace codeNamespace = new CodeNamespace(namespaceName);
            listNamespaces.Add(codeNamespace);

            return codeNamespace;
        }

        public CodeDomHelper AddReference(string referencedAssembly)
        {
            listReferencedAssemblies.Add(referencedAssembly);

            return this;
        }

        public CodeTypeDeclaration Class(string className)
        {
            return new CodeTypeDeclaration(className);
        }

        public CodeSnippetTypeMember Method(string returnType, string methodName, string paramList, string methodBody)
        {
            return Method(string.Format("public static {0} {1}({2}) {{ {3} }} ", returnType, methodName, paramList, methodBody));
        }

        public CodeSnippetTypeMember Method(string methodName, string paramList, string methodBody)
        {
            return Method("void", methodName, paramList, methodBody);
        }

        public CodeSnippetTypeMember Method(string methodName, string methodBody)
        {
            return Method("void", methodName, "", methodBody);
        }

        public CodeSnippetTypeMember Method(string methodBody)
        {
            return new CodeSnippetTypeMember(methodBody);
        }

        public CodeCompileUnit CompileUnit
        {
            get
            {
                // Create a new CodeCompileUnit to contain 
                // the program graph.
                CodeCompileUnit compileUnit = new CodeCompileUnit();

                foreach (var ns in listNamespaces)
                    compileUnit.Namespaces.Add(ns);

                return compileUnit;
            }
        }

        public Assembly Compile()
        {
            return Compile(null);
        }

        public Assembly Compile(string assemblyPath)
        {
            CompilerParameters options = new CompilerParameters();
            options.IncludeDebugInformation = false;
            options.GenerateExecutable = false;
            options.GenerateInMemory = (assemblyPath == null);
            foreach (string refAsm in listReferencedAssemblies)
                options.ReferencedAssemblies.Add(refAsm);
            if (assemblyPath != null)
                options.OutputAssembly = assemblyPath.Replace('\\', '/');

            CodeDomProvider codeProvider = Provider(_language);

            CompilerResults results =
               codeProvider.CompileAssemblyFromDom(options, CompileUnit);
            codeProvider.Dispose();

            if (results.Errors.Count ==  0)
                return results.CompiledAssembly;

            // Process compilation errors
            System.Diagnostics.Trace.WriteLine("Compilation Errors:");
            foreach (string outpt in results.Output)
                System.Diagnostics.Trace.WriteLine(outpt);
            foreach (CompilerError err in results.Errors)
                System.Diagnostics.Trace.WriteLine(err.ToString());

            return null;
        }

        public string GenerateCode()
        {
            StringBuilder sb = new StringBuilder();
            TextWriter tw = new IndentedTextWriter(new StringWriter(sb));

            CodeDomProvider codeProvider = Provider(_language);
            codeProvider.GenerateCodeFromCompileUnit(CompileUnit, tw, new CodeGeneratorOptions());
            codeProvider.Dispose();

            tw.Close();

            return sb.ToString();
        }
    }
}
