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

        public virtual async Task AlterarAsync(TModelo objeto)
        {
            Acao(_context, objeto, EntityState.Modified);
            await _context.SaveChangesAsync();
        }

        public virtual async Task AlterarAsync(List<TModelo> listObjetos)
        {
            foreach (var objeto in listObjetos) { await AlterarAsync(objeto); }
        }

        public virtual async Task ExcluirAsync(string id)
        {
            TModelo objContext = await _context.Set<TModelo>().FindAsync(id);
            if (objContext == null) { return; }
            _context.Set<TModelo>().Remove(objContext);
            await _context.SaveChangesAsync();
        }

        public virtual async Task ExcluirAsync(List<string> ids)
        {
            foreach (var id in ids) { await ExcluirAsync(id); }
        }

        public virtual async Task ExcluirAsync(TModelo objeto)
        {
            Acao(_context, objeto, EntityState.Deleted);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(List<TModelo> objetos)
        {
            foreach (var objeto in objetos) { await ExcluirAsync(objeto); }
        }

        public virtual async Task IncluirAsync(TModelo objeto)
        {
            _context.Set<TModelo>().Add(objeto);
            await _context.SaveChangesAsync();
        }

        public async Task IncluirAsync(TModelo[] objeto)
        {
            _context.Set<TModelo>().AddRange(objeto);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TModelo> AsQueryable() { return _context.Set<TModelo>().AsQueryable<TModelo>(); }
    }
}
