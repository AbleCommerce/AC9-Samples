using System.Web.Mvc;
namespace RandomQuotesPlugin.Controllers
{
    using System.Linq;
    using CommerceBuilder.Common;
    using CommerceBuilder.Utility;
    using CommerceBuilder.Web.Mvc;
    using Models;
    
    public class RQPAdminController : AbleAdminController
    {
        private readonly IQuoteRepository _quoteRepo;

        public RQPAdminController(IQuoteRepository quoteRepo)
        {
            _quoteRepo = quoteRepo;
        }

        public ActionResult Index(string sortExpression = "CreatedDate DESC", int page = 1, int pageSize = 20)
        {
            int pageIndex = page - 1;
            int count = _quoteRepo.CountAll();

            if (pageSize == 0 && count != 0) pageSize = count;
            if (count == 0) pageSize = 20;

            var quotes = _quoteRepo.LoadAll(sortExpression, pageSize, pageIndex)
                .Select(quote => new QuoteModel()
                {
                    Id = quote.Id,
                    CreatedById = quote.CreatedBy != null ? quote.CreatedBy.Id : 0,
                    CreatedByName = quote.CreatedBy != null ? quote.CreatedBy.UserName : string.Empty,
                    Author = quote.Author,
                    Website = quote.Website,
                    Content = quote.Content,
                    CreatedDate = quote.CreatedDate
                }).ToList();

            var model = quotes; //new StaticPagedList<QuoteModel>(quotes, page, pageSize, count);

            return View("~/Plugins/RandomQuotesPlugin/Views/Index.cshtml", model);
        }

        public ActionResult AddEditQuote(int? quoteId)
        {
            Quote quote = null;
            if (quoteId.HasValue && quoteId.Value > 0)
            {
                quote = _quoteRepo.Load(quoteId.Value);
                if (quote == null)
                    return RedirectToAction("Index");
            }

            QuoteModel model = new QuoteModel();
            if (quote != null)
            {
                model.Id = quote.Id;
                model.Author = quote.Author;
                model.Website = quote.Website;
                model.Content = quote.Content;
            }

            return View("~/Plugins/RandomQuotesPlugin/Views/AddEditQuote.cshtml", model);
        }

        [HttpPost]
        public ActionResult AddEditQuote(QuoteModel model, bool saveOnly)
        {
            if (ModelState.IsValid)
            {
                Quote quote = null;
                if (model.Id > 0)
                {
                    quote = _quoteRepo.Load(model.Id);
                    if (quote == null)
                        return RedirectToAction("Index");
                }
                else
                {
                    quote = new Quote();
                }

                if (quote != null)
                {
                    quote.Author = model.Author;
                    quote.Content = model.Content;
                    quote.Website = model.Website;

                    if (quote.Id == 0)
                    {
                        quote.CreatedBy = AbleContext.Current.User;
                        quote.CreatedDate = LocaleHelper.LocalNow;
                    }

                    _quoteRepo.Save(quote);

                    if (saveOnly)
                        return RedirectToAction("AddEditQuote", new { quoteId = quote.Id });
                    else
                        return RedirectToAction("Index");
                }
            }

            return View("~/Plugins/RandomQuotesPlugin/Views/AddEditQuote.cshtml", model);
        }


        public ActionResult DeleteQuote(int quoteId)
        {
            var quote = _quoteRepo.Load(quoteId);
            if (quote != null)
                _quoteRepo.Delete(quote);
            return RedirectToAction("Index");
        }
    }
}
