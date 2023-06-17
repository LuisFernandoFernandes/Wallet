using Microsoft.EntityFrameworkCore;
using Wallet.Tools.database;
using Wallet.Tools.generic_module;
using Wallet.Tools.injector;

namespace Wallet.Tools.entity_framework
{
    public class EntityFrameworkService<TModelo> : IEntityFrameworkService<TModelo> where TModelo : class, IGenericModel
    {
        private Context _context;

        public void Detach()
        {
            var local = _context.Set<TModelo>().Local;
            local.ToList().ForEach(p => _context.Entry(p).State = EntityState.Detached);
        }

        public EntityFrameworkService() { _context = Factory.InstanceOf<Context>(); }

        public void SetContext(Context context)
        {
            _context = context;
        }

        private void Acao(Context context, TModelo objeto, EntityState estado)
        {
            context.Set<TModelo>().Attach(objeto);
            context.Entry(objeto).State = estado;
        }

        public virtual async Task UpdateAsync(TModelo objeto)
        {
            Acao(_context, objeto, EntityState.Modified);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(List<TModelo> listObjetos)
        {
            foreach (var objeto in listObjetos) { await UpdateAsync(objeto); }
        }

        public virtual async Task DeleteAsync(string id)
        {
            TModelo objContext = await _context.Set<TModelo>().FindAsync(id);
            if (objContext == null) { return; }
            _context.Set<TModelo>().Remove(objContext);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(List<string> ids)
        {
            foreach (var id in ids) { await DeleteAsync(id); }
        }

        public virtual async Task DeleteAsync(TModelo objeto)
        {
            Acao(_context, objeto, EntityState.Deleted);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(List<TModelo> objetos)
        {
            foreach (var objeto in objetos) { await DeleteAsync(objeto); }
        }

        public virtual async Task InsertAsync(TModelo objeto)
        {
            _context.Set<TModelo>().Add(objeto);
            await _context.SaveChangesAsync();
        }

        public async Task InsertAsync(TModelo[] objeto)
        {
            _context.Set<TModelo>().AddRange(objeto);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TModelo> AsQueryable() { return _context.Set<TModelo>().AsQueryable<TModelo>(); }

        public Task UpdateAsync(List<TModelo> obj, Context context)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TModelo obj, Context context)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(List<string> ids, Context context)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(List<TModelo> objs, Context context)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, Context context)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TModelo obj, Context context)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(TModelo obj, Context context)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(TModelo[] obj, Context context)
        {
            throw new NotImplementedException();
        }
    }
}
