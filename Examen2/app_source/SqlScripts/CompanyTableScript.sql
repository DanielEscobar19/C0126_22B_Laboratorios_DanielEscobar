USE [C02748]
GO

/****** Object:  Table [dbo].[CompanyModel]    Script Date: 9/12/2022 12:10:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CompanyModel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](256) NOT NULL,
	[TipoNegocio] [nvarchar](256) NULL,
	[PaisBase] [nvarchar](256) NULL,
	[ValorEstimado] [decimal](18, 2) NULL,
	[EsTransnacional] [bit] NOT NULL,
 CONSTRAINT [PK_CompanyModel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


