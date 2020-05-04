////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 8/31/2010 5:55:40 PM 
// Description: SearchTaskContent.cs  
// Revisions  :            		
// **************************************************************************** 

#if WINONLY


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Support.Net.WPF
{
    public class SearchTaskContent<TFilter, TResult> : ThreadTaskContentBase
    {
        public SearchTaskContent() : this(default(TFilter)) { }
        public SearchTaskContent(TFilter filter)
        {
            this.Filter = filter;
        }
        protected override void DispathCall()
        {
            if (DispathCallEvent != null)
                DispathCallEvent(this);
        }

        protected override void DoRun()
        {
            if (DoSearchEvent != null)
                SearchResults = DoSearchEvent(Filter);
        }
        public event Action<SearchTaskContent<TFilter, TResult>> DispathCallEvent;
        public event Func<TFilter, TResult> DoSearchEvent;
        public TResult SearchResults { get; private set; }
        public TFilter Filter { get; set; }

    }
}

#endif