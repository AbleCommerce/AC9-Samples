using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace RandomQuotesPlugin
{
    public class RandomOrder : NHibernate.Criterion.Order
    {
        public RandomOrder() : base("", true)
        {
        }

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            return new SqlString("NEWID()");
        }
    }
}
