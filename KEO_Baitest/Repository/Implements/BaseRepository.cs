using KEO_Baitest.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace KEO_Baitest.Repository.Implements
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        protected BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Update(entity); 
        }

        public T? GetById(Guid id)
        {
            return _context.Set<T>().Find(id);
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public bool IsSaveChange()
        {
            return _context.SaveChanges() > 0;
        }

        public List<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).ToList();
        }
        public List<T> ExcuteSQL(string sqlQuery, string keyword)
        {
            var tableName = _context.Model.FindEntityType(typeof(T))?.GetTableName();
            return _context.Set<T>().FromSqlRaw(sqlQuery,
                new SqlParameter("@TableName", tableName),
                new SqlParameter("@Keyword","%"+ keyword + "%"))
                .AsEnumerable()
                .ToList();
        }
    }
}
