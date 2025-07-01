using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Basecode.Data.Repositories
{
    public class BaseRepository
    {
        protected IUnitOfWork UnitOfWork { get; set; }

        protected AsiBasecodeDBContext Context => (AsiBasecodeDBContext)UnitOfWork.Database;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));
            UnitOfWork = unitOfWork;
        }

        protected virtual DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            return Context.Set<TEntity>();
        }

        protected virtual void SetEntityState(object entity, EntityState entityState)
        {
            Context.Entry(entity).State = entityState;
        }

        protected Task<PaginatedList<T>> GetPaged<T>(
            IQueryable<T> query,
            int pageIndex,
            int pageSize)
            where T : class
        {
            return PaginatedList<T>.Create(query, pageIndex, pageSize);
        }
    }
}
