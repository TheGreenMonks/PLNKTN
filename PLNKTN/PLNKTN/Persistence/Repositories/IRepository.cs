using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLNKTN.Persistence.Repositories
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Removes entity T from the table.
        /// </summary>
        /// <param name="id">Entity Hash Key to remove.</param>
        BatchWrite<T> DeleteById(string id, BatchWrite<T> batchWrite = null);

        /// <summary>
        /// Return all objects in table T.  Performs Scan operation.  Use sparingly.
        /// </summary>
        /// <returns>A <c>List<T></c> entities within table T.</returns>
        Task<IList<T>> GetAllAsync();

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
        /// <returns>The entity T inserted into the table.</returns>
        BatchWrite<T> Insert(T entity, BatchWrite<T> batchWrite = null);

        /// <summary>
        /// Updates entity T with new data.
        /// </summary>
        /// <param name="entity">Entity with updated data to to insert/replace in the table.</param>
        BatchWrite<T> Update(T entity, BatchWrite<T> batchWrite = null);

        /// <summary>
        /// Updates collection of entities T with new data.
        /// </summary>
        /// <param name="entity">Collection of Entities with updated data to to insert/replace in the table.</param>
        BatchWrite<T> Update(ICollection<T> entities, BatchWrite<T> batchWrite = null);
    }
}