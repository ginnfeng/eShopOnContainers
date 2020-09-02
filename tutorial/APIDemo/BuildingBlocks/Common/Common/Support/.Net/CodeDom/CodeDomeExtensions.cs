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

namespace Common.Support.Net.CodeDom
{
    public static class CodeDomeExtensions
    {
        public static CodeNamespace AddClass(this CodeNamespace codeNamespace, CodeTypeDeclaration codeType)
        {
            codeNamespace.Types.Add(codeType);

            return codeNamespace;
        }

        public static CodeTypeDeclaration AddMethod(this CodeTypeDeclaration classCode, CodeSnippetTypeMember methodBody)
        {
            classCode.Members.Add(methodBody);

            return classCode;
        }

        public static CodeNamespace Imports(this CodeNamespace codeNamespace, string namespaceName)
        {
            codeNamespace.Imports.Add(new CodeNamespaceImport(namespaceName));

            return codeNamespace;
        }
    }
}
