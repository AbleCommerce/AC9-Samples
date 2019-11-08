namespace RandomQuotesPlugin
{
    using System;
    using System.Collections.Generic;
    using CommerceBuilder.DomainModel;
    using NHibernate.Criterion;

    /// <summary>
    /// This class provides repository functionality for the Quote
    /// </summary>
    [RegisterFor(typeof(IQuoteRepository))]
    [RegisterFor(typeof(IRepository<Quote>))]
    public class QuoteRepository : Repository<Quote>, IQuoteRepository
    {
        /// <inheritdoc />
        public IList<Quote> LoadQuotesCreatedBy(int userId, string sortExpression = "", int maximumRows = 0, int startRowIndex = 0)
        {
            return NHibernateHelper.CreateCriteria<Quote>(maximumRows, startRowIndex, sortExpression)
               .Add(Restrictions.Eq("CreatedBy.Id", userId))
               .List<Quote>();
        }

        /// <inheritdoc />
        public int CountQuotesCreatedBy(int userId)
        {
            return NHibernateHelper.CreateCriteria<Quote>()
                .Add(Restrictions.Eq("CreatedBy.Id", userId))
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();
        }

        /// <inheritdoc />
        public Quote GetRandomQuote()
        {
            return NHibernateHelper.CreateCriteria<Quote>()
                .AddOrder(new RandomOrder())
                .SetMaxResults(1)
                .UniqueResult<Quote>();
        }
    }
}
