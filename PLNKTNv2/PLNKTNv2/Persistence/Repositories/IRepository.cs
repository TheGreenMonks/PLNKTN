using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTNv2.Persistence.Repositories
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Removes entity T from the table.
        /// </summary>
        /// <param name="id">Entity Hash Key to remove.</param>
        Task DeleteByIdAsync(string id);

        /// <summary>
        /// Return all objects in table T.  Performs Scan operation.  Use sparingly.
        /// </summary>
        /// <returns>A <c>List/<T/></c> entities within table T.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Return single object by its ID
        /// </summary>
        /// <param name="id"><c>string</c> representation of object Hash Key</param>
        /// <returns>The entity with the corresponding ID.</returns>
        Task<T> GetByIdAsync(string id);

        /// <summary>
        /// Insert entity T into its corresponding table.
        /// </summary>
        /// <param name="entity">The entity to insert to the table.</param>
        Task InsertAsync(T entity);

        /// <summary>
        /// Insert entities T into its table.
        /// </summary>
        /// <param name="entities">Entities to be inserted.</param>
        Task InsertAsync(ICollection<T> entities);

        /// <summary>
        /// Update list of entities T with new data.
        /// </summary>
        /// <param name="entities">Enumerable entity list with updated data to to insert/replace in the table.</param>
        Task UpdateAllAsync(IEnumerable<T> entities);

        /// <summary>
        /// Update entity T with new data.
        /// </summary>
        /// <param name="entity">Entity with updated data to to insert/replace in the table.</param>
        Task UpdateAsync(T entity);
    }
}