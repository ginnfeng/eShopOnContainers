////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/28/2011 5:11:58 PM 
// Description: FileSourceProviderBase.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Support.Serializer;
using Common.DataContract;
using System.IO;
using Support.Net.Util;
using System.Linq;
using Support;
using Support.DataTransaction;

namespace Common.DataCore
{
    public abstract partial class FileSourceProviderBase<TTransfer> : IEntityTableSource, IEntityTableSourceTransMgr,IDisposable
    {
        abstract protected void CreateDirectory(string directory);
        abstract protected FileStream TakeFileStream(string filePath, FileMode mode, FileAccess access);
        abstract protected bool DirectoryExists(string path);
        abstract protected bool FileExists(string path);
        abstract protected void DeleteFile(string path);
        abstract protected void DeleteDirectories();
        abstract protected string[] GetDirectoryNames();
        abstract protected string[] GetFileNames(string dir, string searchPattern);
        abstract protected void Close();
        abstract protected IDisposable CreateLocker(bool isReadLocker);
        public event Action<EntityCatalog> EntityCatalogChangedEvent; 
        public string StoreDirectory { get; set; }
        
        static readonly string indexDirName = "Index";
        
        private TTransfer transfer;
        private EntityTableTransaction entityTransaction = new EntityTableTransaction();


        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass of this type implements a finalizer.
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: Add resource.Dispose() logic here
                    Close();
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed; 
    }

    public abstract partial class FileSourceProviderBase<TTransfer> 
        where TTransfer : BaseTransfer, new()
    {
        public FileSourceProviderBase(string storeDirectory)
        {
            transfer = new TTransfer();
            StoreDirectory = storeDirectory;           
        }

        #region IEntityTableSource Members
        public bool TryGetTable<TEntity>(EntityRelation entityRelation, out EntityTableProxy<TEntity> tableProxy)
        {
            EntityTable table;
            if (TryGetTable(entityRelation, out  table))
            {
                tableProxy = new EntityTableProxy<TEntity>(table, this);
                return true;
            }
            tableProxy = null;
            return false;
        }

        public bool TryGetTable(EntityRelation entityRelation, out EntityTable table)
        {
            table = null;
            //改成還是回傳一個空的EntityTable, by Feng (Policy builder)
            //if (entityRelation.IsEmptyKeys())         
            //    return false;
            
            EntityRelation relation = entityRelation.Clone();           

            EntityTable srcTable;
            //先找找記憶體中有否
            if (entityTransaction.TryGetRegistTable(relation, out srcTable))
            {
                if (TryGetTable(srcTable, ref relation, out table))
                {
                    //Add by Feng, For PolicyBuilder, 2011/11/09
                    this.Regist(table, OPStatus.Steady);
                    return true;
                }
            }

            //到File中找
            string filePath = TakeFilePath(relation.Namespace, relation.TableName, typeof(TTransfer));
            if (!FileExists(filePath)) return false;
            srcTable = RetriveEntity<EntityTable>(filePath);
            EntityTable selectedTable;
            if (TryGetTable(srcTable, ref relation, out selectedTable))
            {
                this.Regist(selectedTable, OPStatus.Steady);
                if (table == null)
                    table = selectedTable;
                else
                    table.Rows.AddRange(selectedTable.Rows);
                table.RebindRows();
                return true;
            }
            //找不到時回傳false但out table為一空的EntityTable而非null,如此Policy Editor可以用此empty EntityTable新增EntityRow
            this.Regist(table, OPStatus.Steady);
            return false;
        }
        #endregion
        public void Regist(EntityTable entityTable, OPStatus opStatus)
        {
            entityTransaction.Regist(entityTable, opStatus);
        }
        public void Regist(EntityTable entityTable, EntityRow row, OPStatus opStatus)
        {
            entityTransaction.Regist(entityTable, row, opStatus);
        }
        private bool TryGetTable(EntityTable srcTable, ref EntityRelation entityRelation, out EntityTable table)
        {            
            table = null;
            //srcTable.RebindRows();
            var attris = srcTable.GetAttributesEntity();
            if (entityRelation.IsSelectAll())
            {                
                if (attris.IsPartial)    return false;
                table = srcTable;
                attris.IsPartial = false;
                return true;
            }            
            var selectedTable = srcTable.Clone(false);
            selectedTable.GetAttributesEntity().IsPartial = true;
            if (!attris.IsPartial&&srcTable.Rows.Count == 0)
            {
                table = selectedTable; 
                return true;
            }

            EntityRelation relation = entityRelation;
            bool isFound = false;
            Action<object, EntityRow> action = delegate(object key, EntityRow row)
            {
                if (key!=null&&relation.MatchKey(key.ToString(),row))
                {
                    selectedTable.Rows.Add(row);
                    relation.Keys.Remove(key.ToString());
                    isFound = true;
                }
            };
            srcTable.ForeachRowByKey(action);
            entityRelation = relation;
            table = selectedTable;
            //return entityRelation.IsEmptyKeys();
            return attris.IsPartial ? entityRelation.IsEmptyKeys() : isFound;
        }
        /// Add by Sean Hsu. 2011/3/21
        public bool TryGetTable(EntityTable entityTable, out EntityTable table)
        {
            EntityRelation entityRelation = new EntityRelation() { TableName = entityTable.TableName, Namespace = entityTable.Namespace };
            entityRelation.SetAsSelectAll();
            return this.TryGetTable(entityRelation, out table);
        }

        /// Add by Sean Hsu. 2011/3/21
        public bool TryGetNSTables(string nameSpace, ref List<EntityTable> tables)
        {           
            string dir = TakeDirectory(nameSpace);
            if (!DirectoryExists(dir))
            {
                CreateDirectory(dir);
                return false;
            }
            /// suppose Only One TTransfer Type file Exist.
            string[] files = this.GetFileNames(dir, "*.*");
            EntityTable table = null;
            EntityRelation entityRelation = new EntityRelation() { Namespace = nameSpace };
            entityRelation.SetAsSelectAll();

            foreach (string file in files)
            {
                entityRelation.TableName = Path.GetFileNameWithoutExtension(file);
                if (this.TryGetTable(entityRelation, out table))
                    tables.Add(table);
            }
            return true;
        }


        /// Add By Sean Hsu. 2011/3/21
        public void SaveTableData(EntityTable saveTable, bool updateCatalog = true)
        {  
            var path = TakeFilePath(saveTable.Namespace, saveTable.TableName, typeof(TTransfer));
            var saveTableAttris = saveTable.GetAttributesEntity();
            saveTableAttris.PreStatus = saveTableAttris.Status;
            var cloneTable = saveTable;// EntityTableHelper.CloneUnbindEntityTable(saveTable);
            var tableAttris = cloneTable.GetAttributesEntity();
            bool renewVersion = (tableAttris.Status != OPStatus.Steady);
            switch (tableAttris.Status)
            {
                case OPStatus.Delete:
                    this.DeleteFile(path);
                    entityTransaction.Unregist(saveTable, true);
                    if (updateCatalog) UpdateEntityCatalog(new EntityTable[] { saveTable });
                    return;      
                case OPStatus.Update://schema異動
                case OPStatus.Copy:
                    bool isNamingChanged = EntityTableHelper.IsNamingChanged(cloneTable);
                    if (isNamingChanged)
                    {
                        var oldPath = TakeFilePath(saveTable.OldSchema.Namespace, saveTable.OldSchema.TableName, typeof(TTransfer));
                        cloneTable = RetriveEntity<EntityTable>(oldPath);
                        if (OPStatus.Update == tableAttris.Status)
                            this.DeleteFile(oldPath);
                    }
                    else
                    {
                        cloneTable = RetriveEntity<EntityTable>(path);
                    }
                    UpdateTableSchema(ref cloneTable, saveTable);                   
                    entityTransaction.Reregist(cloneTable,OPStatus.Steady);
                    break;                        
                default:                    
                    if (tableAttris.IsPartial)
                    {
                        //在client端此case為Tenant不存在IsolatedStorageFile中
                        if (this.FileExists(path))
                        {
                            cloneTable = RetriveEntity<EntityTable>(path);
                            UpdateTableData(ref cloneTable, saveTable);                            
                        }
                        else
                        {
                            tableAttris.IsPartial = false;
                        }
                        renewVersion = true;
                    }
                    break;
            }
                        
            foreach (var row in cloneTable.Rows)
            {
                var rowAttris = row.GetAttributesEntity();
                if (rowAttris.Status != OPStatus.Steady)  renewVersion = true;               
                rowAttris.Status = OPStatus.Steady;
            }
            tableAttris.Status = OPStatus.Steady;
            if (renewVersion)
            {  //Client update新版table不需renewVersion
                cloneTable.UpDateVersion();
                saveTable.Version = cloneTable.Version;
                entityTransaction.FindRegistTableDo(cloneTable.Namespace, cloneTable.TableName,tbl => tbl.Version = cloneTable.Version);                
            }
            PersistEntity(path, cloneTable);            
            saveTableAttris.Status = OPStatus.Steady;
            saveTable.Rows.ForEach(row => row.GetAttributesEntity().Status = OPStatus.Steady);
            if (updateCatalog) UpdateEntityCatalog(new EntityTable[] { saveTable });
        }

        public EntityTable CreateTable<TEntity>()
        {
            var type=typeof(TEntity);
            return CreateTable<TEntity>(type.Namespace, type.Name);
        }
        public EntityTableProxy<TEntity> CreateTableProxy<TEntity>()
        {
            var type = typeof(TEntity);
            return CreateTableProxy<TEntity>(type.Namespace, type.Name);
        }

        public EntityTable CreateTable(EntityTable schema)
        {
            return DoCreateTable(schema.Namespace, schema.TableName, () => EntityTableHelper<IDictionaryAccess>.CreateDataTable(schema));
        }

        public void UpdateTableSchema(EntityTable schema, EntityTable oldSchema,OPStatus opStatus)
        {
            schema.OldSchema = oldSchema;
            entityTransaction.Regist(schema, opStatus);
        }

        private EntityTable DoCreateTable(string ns, string tableName, Func<EntityTable> createDataTableFunc)
        {
            EntityTable table;
            if (!entityTransaction.TryGetRegistTable(ns, tableName, out table))
            {
                string dir = StoreDirectory + "\\" + ns;
                if (!Directory.Exists(dir)) CreateDirectory(dir);
                table = createDataTableFunc();
                table.Namespace = ns;
                table.TableName = tableName;
                entityTransaction.Regist(table, OPStatus.Add);
            }
            else
            {
                if (!table.Namespace.Equals(ns) || !table.TableName.Equals(tableName))
                    throw new Exception("FileSourceProviderBase.CreateTable()");
            }
            return table;
        }

        

        #region IEntityTableSourceMgr Members

        public EntityTable CreateTable<TEntity>(string ns, string tableName)
        {
            return DoCreateTable(ns, tableName, () => EntityTableHelper<TEntity>.CreateDataTable());            
        }

        public EntityTableProxy<TEntity> CreateTableProxy<TEntity>(string ns, string tableName)
        {
            EntityTable table = CreateTable<TEntity>(ns, tableName);
            return new EntityTableProxy<TEntity>(table) { SourceProvider = this };
        }
        public void SaveAll()
        {
            IEnumerable<EntityTable> updatedTables = (Transaction.State != DataTransactionState.Commit)
                ? entityTransaction.GetAllTable()
                : GenEntityTableChangeChangeDetails();
            SaveAll(updatedTables);
            entityTransaction.ClearAllTable();
        }
        public void CleanAll()
        {
            this.DeleteDirectories();
        }
        #endregion
        public void SaveAll(IEnumerable<EntityTable> tables)
        {
            if (tables != null && tables.Count()>0)
            {                
                tables.ForEach(table => SaveTableData(table, false));
                UpdateEntityCatalog(tables.ToArray());
            }
        }
        public void UpdateEntityCatalog(EntityTable[] tables)
        {
            var tableGroupMap = tables.GroupBy(table => table.Namespace);
            foreach (IGrouping<string, EntityTable> tableGroup in tableGroupMap)
            {
                var ns = tableGroup.Key;
                EntityCatalog orginlCatalog = TakeEntityCatalog(ns);
                foreach (EntityTable entityTable in tableGroup.ToList())
                {
                    var digest = orginlCatalog.Digests.Find(it => it.Key == entityTable.TableName);
                    if (digest != null) digest.Version = entityTable.Version;
                    OPStatus status=entityTable.GetAttributesEntity().PreStatus;
                    switch (status)
                    {
                        case OPStatus.Delete:
                            if (digest != null)
                            {
                                orginlCatalog.Digests.Remove(digest);
                                digest = null;
                            }
                            break;
                        case OPStatus.Update:
                            if (EntityTableHelper.IsNamingChanged(entityTable))
                            {
                                digest = entityTable.GenEntityDigest();
                                var oldDigest = orginlCatalog.Digests.Find(it => it.Key == entityTable.OldSchema.TableName);
                                orginlCatalog.Digests.Remove(oldDigest);
                                orginlCatalog.Digests.Add(digest);
                            }
                            break;
                        default:
                            if (digest == null)
                            {
                                digest = entityTable.GenEntityDigest();
                                orginlCatalog.Digests.Add(digest);
                            }                           
                            break;                            
                    }                                 
                    PersistEntity(TakeCatalogFilePath(ns), orginlCatalog);
                    if (EntityCatalogChangedEvent != null)
                        EntityCatalogChangedEvent(orginlCatalog);
                }
            }
        }
        private string TakeCatalogFilePath(string ns)
        {
            return TakeFilePath(indexDirName, ns + "." + typeof(TTransfer).Name, typeof(TTransfer));
        }

        public List<EntityCatalog> TakeAllEntityCatalog(bool autoRebuild)
        {
            var entityCatalogs = new List<EntityCatalog>();
            ForeachFile(indexDirName, path => entityCatalogs.Add(RetriveEntity<EntityCatalog>(path)));
            if (entityCatalogs.Count == 0 && autoRebuild)
            {
                return RebuildAllEntityCatalog();
            }
            return entityCatalogs;
        }
        public EntityCatalog TakeEntityCatalog(string ns)
        {
            EntityCatalog catalog;
            var path = TakeCatalogFilePath(ns);
            if (!FileExists(path))  return new EntityCatalog() { Name = ns };
            catalog=RetriveEntity<EntityCatalog>(path);            
            return (ns == catalog.Name)?catalog:  new EntityCatalog() { Name = ns };           
        }

        public List<EntityCatalog> RebuildAllEntityCatalog()
        {
            ForeachFile(indexDirName, path => this.DeleteFile(path));            
            var dirNames = this.GetDirectoryNames();
            List<EntityTable> tables = new List<EntityTable>();
            foreach (var dirName in dirNames)
            {
                if (indexDirName.Equals(dirName))
                    continue;
                string dirPath = TakeDirectory(dirName);
                ForeachFile(dirName, path => tables.Add(RetriveEntity<EntityTable>(path)));
            }
            var catalogs=tables.GenEntityCatalogs();
            catalogs.ForEach(catalog => this.PersistEntity<EntityCatalog>(TakeCatalogFilePath(catalog.Name), catalog));
            return new List<EntityCatalog>(catalogs);
        }
        private void ForeachFile(string dirName,Action<string> FilePathAction)
        {
            string dir = TakeDirectory(dirName);
            string[] files = GetFileNames(dir, "*.*");
            foreach (var file in files)
            {
                string path = dir + "\\" + file;
                FilePathAction(path);
            }
        }

        public IDataTransactionBasic Transaction { get { return entityTransaction; } }

        public List<EntityTable> GenEntityTableChangeChangeDetails()
        {
            return entityTransaction.GenEntityTableChangeChangeDetails();
        }
        private void UpdateTableSchema(ref EntityTable targetTable, EntityTable schema)
        {
            targetTable.Namespace = schema.Namespace;
            targetTable.TableName = schema.TableName;
            if (!EntityTableHelper.IsColumnChanged(schema)) return;
            var schemaProxy=new EntityTableProxy<IEntitySchema>(schema);           
            for (int i = schema.Rows.Count - 1; i >= 0; i--)
            {
                var colDefRow=schema.Rows[i];
                switch (colDefRow.GetAttributesEntity().Status)
                {        
                    case OPStatus.Delete:
                        targetTable.ShrinkColumnAt(i);
                        break;
                    case OPStatus.Add:
                        IEntitySchema colDef = new EntityRowProxy<IEntitySchema>(colDefRow).Entity;
                        var newColumn = new EntityColumn();
                        colDef.CopyTo(newColumn);
                        targetTable.ExpandColumn(newColumn);
                        break;
                    case OPStatus.Update:
                        colDef = new EntityRowProxy<IEntitySchema>(colDefRow).Entity;
                        colDef.CopyTo(targetTable.Columns[i]);
                        break;                    
                    default:
                        break;
                }               
            }
            
        }
        private void UpdateTableData(ref EntityTable targetTable, EntityTable deltaTable)
        {
            targetTable.GetAttributesEntity().KeyCount = deltaTable.GetAttributesEntity().KeyCount;
            foreach (var row in deltaTable.Rows)
            {
                var attris = row.GetAttributesEntity();
                object key;
                switch (attris.Status)
                {
                    case OPStatus.Add:
                        targetTable.Append(row.ItemArray);//避免Status被複製
                        break;
                    case OPStatus.Update:
                        key = row[deltaTable.PrimaryKey];
                        var orignalRow = targetTable.Find(key.ToString());                      
                        row.CopyItermArrayTo(orignalRow);
                        break;
                    case OPStatus.Delete:
                        key = row[deltaTable.PrimaryKey];
                        targetTable.Remove(key.ToString());
                        break;
                    default:
                        break;
                }
            }
            
        }
        private string TakeDirectory(string ns)
        {
            return StoreDirectory + "\\" + ns;
        }
        private string TakeFilePath(string ns, string tableName, Type transferType)
        {
            string dir =TakeDirectory( ns);
            if (!Directory.Exists(dir)) CreateDirectory(dir);
            return TakeFilePath(StoreDirectory, ns, tableName, transferType);
        }

        static public string TakeFilePath(string storeDirectory, string ns, string tableName, Type transferType)
        {
            return CommonExtension.StringFormat(
                "{0}/{1}/{2}.{3}"
                , storeDirectory
                , ns
                , tableName
                , typeof(BaseXmlTransfer).IsAssignableFrom(transferType) ? "xml" : "txt");
        }
        public void PersistEntity<T>(string path,T tbl)
        {
            lock (this)
            {
                using (CreateLocker(false))
                using (FileStream outFileStream = this.TakeFileStream(path, FileMode.Create, FileAccess.Write))
                {
                    transfer.Serialize(tbl, outFileStream);
                    outFileStream.Close();
                }
            }
        }
        public T RetriveEntity<T>(string path)
        {
            //WIN8 IE行為不一樣,需用Lock(this) thread safe,否則常在此產生未知exception
            lock (this)
            {
                using (CreateLocker(true))
                using (FileStream fileStream = this.TakeFileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var it = transfer.Deserialize<T>(fileStream);
                    fileStream.Close();
                    return it;
                }
            }
            
        }
        
    }
}
