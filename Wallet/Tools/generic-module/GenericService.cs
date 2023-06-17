using Microsoft.EntityFrameworkCore;
using Wallet.Tools.database;

namespace Wallet.Tools.generic_module
{
    public class GenericService<T> : IGenericService<T> where T : class, IGenericModel
    {

        private Context _context;

        public async Task UpdateAsync(List<T> obj, Context context)
        {

            context.Update(obj);
            await context.SaveChangesAsync();
            var x = await _context.User.AsQueryable().ToListAsync();
        }

        public async Task UpdateAsync(T obj, Context context)
        {
            context.Update(obj);
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

        public void SetContext(Context context)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(List<string> ids, Context context)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(List<T> objs, Context context)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, Context context)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T obj, Context context)
        {
            throw new NotImplementedException();
        }
    }
}
