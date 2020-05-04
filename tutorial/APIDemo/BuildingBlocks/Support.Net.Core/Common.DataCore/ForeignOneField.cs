////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/15/2011 10:01:06 AM 
// Description: ForeignOneField.cs  
// Revisions  :            		
// **************************************************************************** 

namespace Common.DataCore
{
    public class ForeignOneField<TEntity> : ForeignFieldBase<TEntity>
    {
        public ForeignOneField()
        {            
        }
        public TEntity Entity 
        {
            get 
            {
                return (base.GetCount() == 0)?default(TEntity):base.GetFirst();
            }           
        }
        public void AssignNew()
        {
            AssignNew(EntityRelation.Namespace, EntityRelation.TableName);
        }

        public void AssignNew(string ns, string tableName)
        {
            Remove();
            base.DoAddNew(ns, tableName);
        }
        public void Remove(bool referenceOnly = true)
        {
            if (Entity != null)  
                base.DoRemove(Entity, referenceOnly);
        }
        override protected void FireResetNotifyCollection() { }
    }
}
