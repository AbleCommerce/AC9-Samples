namespace RandomQuotesPlugin
{
    using System;
    using CommerceBuilder.DomainModel;
    using CommerceBuilder.Users;

    /// <summary>
    /// Quote object for NHibernate mapped table 'custom_Quotes'.
    /// </summary>
    public class Quote : Entity
    {
        /// <summary>
        /// Gets or sets the identifier.
        public override int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public virtual string Author { get; set; }

        /// <summary>
        /// Gets or sets the website.
        /// </summary>
        public virtual string Website { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public virtual string Content { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        public virtual User CreatedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }
    }
}
