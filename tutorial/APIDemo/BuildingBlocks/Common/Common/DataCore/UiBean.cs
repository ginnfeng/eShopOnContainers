////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/7/2011 2:01:31 PM 
// Description: UiBean.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Support.DataTransaction;
using Common.DataContract;

namespace Common.DataCore
{
    public class UiBean<TSpec,TPresentation> : ObjectTransaction
        where TPresentation : IPresentationBase, IDictionaryAccess
        where TSpec : ISpecBase, IDictionaryAccess
    {
        public UiBean(TSpec spec, TPresentation presentation)
        {
            DataWrap = new UiDataWrap<TSpec>(spec);
            DataWrap.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnDataWrapPropertyChanged);
            Presentation = new UiPresentation<TPresentation>(presentation);
        }                
        public UiBean(TSpec spec,TPresentation presentation,object data)
        {
            DataWrap = new UiDataWrap<TSpec>(spec, data);
            DataWrap.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnDataWrapPropertyChanged);
            Presentation = new UiPresentation<TPresentation>(presentation);
        }

        void OnDataWrapPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }                
        public UiPresentation<TPresentation> Presentation { get; private set; }
        
        
        [DataTransaction] 
        public UiDataWrap<TSpec> DataWrap { get;  private set; }

        //static public UiBean<TSpec, TPresentation> CreateStardard(object dataValue)
        //{
        //    if (dataValue == null) throw new NullReferenceException("UiBean.CreateDefault()");
        //    var spec = new DictionaryProxy<object>().GenEntityProxy<TSpec>();
        //    var presentation = new DictionaryProxy<object>().GenEntityProxy<TPresentation>();
        //    presentation.Visible = true;
        //    spec.Type = dataValue.GetType().FullName;
        //    return new UiBean<TSpec, TPresentation>(spec, presentation, dataValue);
        //}
        //static public UiBean<TSpec, TPresentation> CastAsUiBean(object uiBean)
        //{
        //    return uiBean as UiBean<TSpec, TPresentation>;
        //}
    }
}
