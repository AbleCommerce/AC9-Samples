/****** Object:  Table [dbo].[custom_Quotes]    Script Date: 9/4/2019 4:02:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[custom_Quotes](
	[QuoteId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Author] [nvarchar](255) NOT NULL,
	[Website] [nvarchar](255) NULL,
	[Content] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_custom_Quotes] PRIMARY KEY CLUSTERED 
(
	[QuoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[custom_Quotes]  WITH CHECK ADD  CONSTRAINT [FK_custom_Quotes_ac_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[ac_Users] ([UserId])
ON DELETE SET NULL
GO

ALTER TABLE [dbo].[custom_Quotes] CHECK CONSTRAINT [FK_custom_Quotes_ac_Users]
GO


