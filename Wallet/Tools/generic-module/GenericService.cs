using Microsoft.EntityFrameworkCore;
using Wallet.Tools.database;

namespace Wallet.Tools.generic_module
{
    public class GenericService<T> : IGenericService<T> where T : class, IGenericModel
    {

        public async Task InsertAsync(T obj, Context context)
        {
            context.Add(obj);
            await context.SaveChangesAsync();
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
