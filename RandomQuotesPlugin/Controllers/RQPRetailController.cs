using System.Web.Mvc;
namespace RandomQuotesPlugin.Controllers
{
    using CommerceBuilder.Web;
    using CommerceBuilder.Web.Mvc;
    using Models;

    public class EPPRetailController : AbleController
    {
        private readonly IQuoteRepository _quoteRepo;

        public EPPRetailController(IQuoteRepository quoteRepo)
        {
            _quoteRepo = quoteRepo;
        }

        [RegisterWidget(Category = CommerceBuilder.CMS.WidgetCategory.Sidebar, DisplayName = "Random Quotes Box")]
        public ActionResult RandomQuotesBox(RandomQuotesBoxParams parameters)
        {
            var quote = _quoteRepo.GetRandomQuote();
            QuoteModel model = null;
            if (quote != null)
            {
                model = new QuoteModel()
                {
                    Id = quote.Id,
                    CreatedById = quote.CreatedBy != null ? quote.CreatedBy.Id : 0,
                    CreatedByName = quote.CreatedBy != null ? quote.CreatedBy.UserName : string.Empty,
                    Author = quote.Author,
                    Content = quote.Content,
                    Website = quote.Website,
                    CreatedDate = quote.CreatedDate
                };
            }

            ViewBag.Caption = parameters.Caption;
            ViewBag.ShowWebsiteLink = parameters.ShowWebsiteLink;

            return PartialView("~/Plugins/RandomQuotesPlugin/Views/Retail/_RandomQuotesBox.cshtml", model);
        }
    }
}
