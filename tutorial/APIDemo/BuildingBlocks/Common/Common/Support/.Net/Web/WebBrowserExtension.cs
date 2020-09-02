////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/23/2010 1:15:30 PM 
// Description: WebBrowserExtension.cs  
// Revisions  :            		
// **************************************************************************** 

#if WINONLY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;
using System.Windows.Controls;
using System.Reflection;
namespace Support.Help.Web
{
    static public class WebBrowserExtension
    {

        /// <summary>
        /// 相當於設ScriptErrorsSuppressed =true,但WPF的WebBrowser無開放此property
        /// </summary>
        /// <param name="it"></param>
        /// <param name="hide"></param>
        static public void SetScriptErrorsSuppressed(this WebBrowser it, bool hide)
        {
            HTMLDocument document = it.Document as HTMLDocument;
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            object objComWebBrowser = fiComWebBrowser.GetValue(it);
            if (objComWebBrowser == null) return;
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

    }
}

#endif