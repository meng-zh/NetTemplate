using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Extensions.EF
{
    public static class Query
    {
        /// <summary>
        /// 单表分页查询
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="orderExpression">排序</param>
        /// <param name="pageSize">分页条数</param>
        /// <param name="pageNum">页数</param>
        /// <param name="total">总条数</param>
        /// <param name="descending">是否降序</param>
        /// <returns></returns>
        public static IQueryable<T> QueryPages<T,TKey>(this DbContext db,
            Expression<Func<T, bool>> predicate, Expression<Func<T,TKey>> orderExpression,
            int pageSize, int pageNum,out int total, bool descending= false) where T : class
        {
            int start = pageNum <= 0 ? 0 : (pageNum - 1) * 10;
            int end = start + pageSize;
            total = db.Set<T>().Where(predicate).Count();
            if(descending)
                return db.Set<T>().Where(predicate)
                .OrderBy(orderExpression)
                .Take(start..end).AsQueryable();
            else
                return db.Set<T>().Where(predicate)
               .OrderByDescending(orderExpression)
               .Take(start..end).AsQueryable();
        }
    }
}
