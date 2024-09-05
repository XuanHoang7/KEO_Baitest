using System.Linq.Expressions;

namespace KEO_Baitest.Repository.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        T GetById(Guid id);
        List<T> GetAll();
        bool IsSaveChange();
        List<T> Find(Expression<Func<T, bool>> predicate);
        public List<T> ExcuteSQL(string sqlQuery, string keyword);
    }
}
