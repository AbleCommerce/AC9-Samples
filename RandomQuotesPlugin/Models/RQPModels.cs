using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommerceBuilder.Web;

namespace RandomQuotesPlugin.Models
{
    public class QuoteModel
    {
        public int Id { get; set; }

        public int CreatedById { get; set; }

        public string CreatedByName { get; set; }
        
        [Required]
        public string Author { get; set; }

        [Required]
        public string Website { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class RandomQuotesBoxParams : WidgetParams
    {
        public RandomQuotesBoxParams()
        {
            Caption = "Random Quote";
            ShowWebsiteLink = true;
        }
        
        [DisplayName("Caption")]
        [DefaultValue("Random Quote")]
        [Description("Allows you to change random quotes box caption if you have enabled to show caption")]
        public string Caption { get; set; }
        
        [DisplayName("Show Website Link")]
        [DefaultValue(true)]
        [Description("Allows you show website link or not if present for a quote")]
        public bool ShowWebsiteLink { get; set; }
    }
}
