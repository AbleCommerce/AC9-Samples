using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceBuilder.DomainModel;

namespace RandomQuotesPlugin
{
    /// <summary>
    /// Interface for Quote entities
    /// </summary>
    public interface IQuoteRepository : IRepository<Quote>
    {
        /// <param name="userId">Given user id</param>
        /// <param name="sortExpression">The sort expression to use for sorting the loaded objects.</param>
        /// <param name="maximumRows">Maximum number of rows to retrieve.</param>
        /// <param name="startRowIndex">Starting index from where to start retrieving.</param>
        /// <returns>List of quotes created by given user id</returns>
        IList<Quote> LoadQuotesCreatedBy(int userId, string sortExpression = "", int maximumRows = 0, int startRowIndex = 0);

        /// <summary>
        /// Counts the quotes created by given user id.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>number of qoutes created by given user id</returns>
        int CountQuotesCreatedBy(int userId);

        /// <summary>
        /// Gets the random quote.
        /// </summary>
        /// <returns>A randomly loaded quote object</returns>
        Quote GetRandomQuote();
    }
}
