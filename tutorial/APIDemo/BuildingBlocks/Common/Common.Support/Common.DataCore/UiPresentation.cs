////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/7/2011 2:36:21 PM 
// Description: UiPresentation.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.DataContract;

namespace Common.DataCore
{
    public class UiPresentation<TPresentation>// : INotifyPropertyChanged
        where TPresentation : IPresentationBase
    {
        public UiPresentation(TPresentation presentationSpec)
        {
            Spec = presentationSpec;           
        }
        public TPresentation Attributions { get; set; }
        public TPresentation Spec 
        {
            get { return spec; }
            set
            {
                spec = value;
                var entityProxy = value as IEntityProxyInfo;                
                if (entityProxy != null)
                {                    
                    var row = entityProxy.Proxy.Row.Clone();
                    Attributions = row.GetEntity<TPresentation>();
                }
            }
        }
        private TPresentation spec;
    }
}
