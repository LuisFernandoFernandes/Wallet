using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Wallet.Tools.database;
using Wallet.Tools.entity_framework;

namespace Wallet.Tools.generic_module
{
    public class GenericService<T> : /*EntityFrameworkService<T>,*/ IGenericService<T> where T : class, IGenericModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        #region Events
        public Func<T, Task> BeforeInsert = null;
        public Func<T, Task> AfterInsert = null;
        public Func<T, Task> BeforeUpdate = null;
        public Func<T, Task> AfterUpdate = null;
        public Func<T, Task> BeforeDelete = null;
        public Func<T, Task> AfterDelete = null;

        #endregion

        //public string GetLoggedInUserId()
        //{
        //    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    return userId;
        //}

        public async Task InsertAsync(T obj, Context context)
        {
            if (BeforeInsert != null) await BeforeInsert.Invoke(obj);

            context.Add(obj);
            await context.SaveChangesAsync();

            if (AfterInsert != null) await AfterInsert.Invoke(obj);
        }

        public async Task InsertAsync(T[] obj, Context context)
        {
            context.Add(obj);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(List<T> obj, Context context)
        {
            context.Update(obj);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T obj, Context context)
        {
            if (obj == context.Set<T>().AsQueryable().Where(a => a.Id == obj.Id).FirstAsync()) return;
            context.Update(obj);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(List<string> ids, Context context)
        {
            context.RemoveRange(ids);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(List<T> objs, Context context)
        {
            context.RemoveRange(objs);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id, Context context)
        {
            context.Remove(id);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T obj, Context context)
        {
            context.Remove(obj);
            await context.SaveChangesAsync();
        }

        public IQueryable<T> AsQueryable()
        {
            throw new NotImplementedException();
        }

        public void Detach()
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }


        public void SetContext(Context context)
        {
            throw new NotImplementedException();
        }

    }
}
