
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace UtilsLib.Utils
{
    public class DBModelBase
    {
        protected DBContextProvider[] _providers = null;
        public DBModelBase(DBContextProvider[] providers)
        {
            _providers = providers;
        }

        public void Debug(MyDbContext context) 
        {
            var name = GetType().Name;
            if (context.RetryDict.ContainsKey(name))
            {
                context.RetryDict[name].Add(this);
            }
            UtilsLib.DebugMessage(
                String.Format(
                    "{0}.{1} has {2}/{3} contexts active. {4}",
                    context.GetType(),
                    GetType(),
                    context.ContextCount,
                    context.CurrentContexts.Count,
                    context.Database.GetConnectionString()));
        }

        public virtual void Store()
        {
            try
            {
                foreach (var provider in _providers)
                {
                    using (var context = provider.GetDBContext())
                    {
                        try
                        {
                            context.Add(this);
                            context.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            UtilsLib.DebugMessage(e.Message);
                            Debug(context);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
        }

        public virtual async Task<int> StoreAsync()
        {
            int saves = 0;
            try
            {
                foreach (var provider in _providers)
                {
                    using (var context = provider.GetDBContext())
                    {
                        try
                        {
                            context.Add(this);
                            await context.SaveChangesAsync();
                            saves++;
                        }
                        catch (Exception e)
                        {
                            UtilsLib.DebugMessage(e.Message);
                            Debug(context);
                        }
                        //context.Database.CloseConnection();
                    }
                }
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
            return saves;
        }

        public virtual void Update()
        {
            try
            {
                foreach (var provider in _providers)
                {
                    using (var context = provider.GetDBContext())
                    {
                        try
                        {
                            context.Update(this);
                            context.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            UtilsLib.DebugMessage(e.Message);
                            Debug(context);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
        }

        public virtual async Task<int> UpdateAsync()
        {
            int saves = 0;
            try
            {
                foreach (var provider in _providers)
                {
                    using (var context = provider.GetDBContext())
                    {
                        try
                        {
                            context.Update(this);
                            await context.SaveChangesAsync();
                            saves++;
                        }
                        catch (Exception e)
                        {
                            UtilsLib.DebugMessage(e.Message);
                            Debug(context);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
            return saves;
        }

        public virtual void Delete()
        {
            try
            {
                foreach (var provider in _providers)
                {
                    using (var context = provider.GetDBContext())
                    {
                        try
                        {
                            context.Remove(this);
                            context.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            UtilsLib.DebugMessage(e.Message);
                            Debug(context);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
        }

        public virtual async Task<int> DeleteAsync()
        {
            int saves = 0;
            try
            {
                foreach (var provider in _providers)
                {
                    using (var context = provider.GetDBContext())
                    {
                        try
                        {
                            context.Remove(this);
                            await context.SaveChangesAsync();
                            saves++;
                        }
                        catch (Exception e)
                        {
                            UtilsLib.DebugMessage(e.Message);
                            Debug(context);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
            return saves;
        }

        public DBModelBase Find(params object[] parameters)
        {
            try
            {
                foreach (var provider in _providers)
                {
                    using (var context = provider.GetDBContext())
                    {
                        try
                        {
                            return (DBModelBase) context.Find(GetType(), parameters);
                        }
                        catch (Exception e)
                        {
                            UtilsLib.DebugMessage(e.Message);
                            Debug(context);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
            return null;
        }

    }
}
