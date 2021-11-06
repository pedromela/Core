using System.Collections.Generic;

namespace UtilsLib.Utils
{
    public class BulkStore 
    {
        private DBContextProvider[] _providers = null;
        private List<DBModelBase> entitiesToStore = new List<DBModelBase>();
        private List<DBModelBase> entitiesToUpdate= new List<DBModelBase>();
        private List<DBModelBase> entitiesToRemove= new List<DBModelBase>();

        public BulkStore(DBContextProvider[] providers) 
        {
            _providers = providers;
        }

        public void Store(DBModelBase entity) 
        {
            entitiesToStore.Add(entity);
        }

        public void Update(DBModelBase entity)
        {
            entitiesToUpdate.Add(entity);
        }

        public void Remove(DBModelBase entity)
        {
            entitiesToRemove.Add(entity);
        }

        public void SaveChanges() 
        {
            if (entitiesToStore.Count == 0 && entitiesToUpdate.Count == 0 && entitiesToRemove.Count == 0)
            {
                return;
            }
            foreach (var provider in _providers)
            {
                
                using (var context = provider.GetDBContext())
                {
                    context.AddRange(entitiesToStore);
                    context.AddRange(entitiesToUpdate);
                    context.AddRange(entitiesToRemove);
                    context.SaveChanges();
                }
                entitiesToStore.Clear();
                entitiesToUpdate.Clear();
                entitiesToRemove.Clear();
            }
        }


    }
}
