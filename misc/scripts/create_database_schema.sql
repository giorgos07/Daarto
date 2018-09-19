USE [master]
GO
/****** Object:  Database [DaartoDb]    Script Date: 4/22/2017 6:43:08 PM ******/
CREATE DATABASE [DaartoDb]
GO
ALTER DATABASE [DaartoDb] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DaartoDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DaartoDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DaartoDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DaartoDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DaartoDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DaartoDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [DaartoDb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DaartoDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DaartoDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DaartoDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DaartoDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DaartoDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DaartoDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DaartoDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DaartoDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DaartoDb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DaartoDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DaartoDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DaartoDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DaartoDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DaartoDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DaartoDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DaartoDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DaartoDb] SET RECOVERY FULL 
GO
ALTER DATABASE [DaartoDb] SET  MULTI_USER 
GO
ALTER DATABASE [DaartoDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DaartoDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DaartoDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DaartoDb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DaartoDb] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'DaartoDb', N'ON'
GO
ALTER DATABASE [DaartoDb] SET QUERY_STORE = OFF
GO
USE [DaartoDb]
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [DaartoDb]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 4/22/2017 6:43:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](128) NULL,
	[LastName] [nvarchar](128) NULL,
	[UserName] [nvarchar](64) NOT NULL,
	[NormalizedUserName] [nvarchar](64) NOT NULL,
	[Email] [nvarchar](64) NOT NULL,
	[NormalizedEmail] [nvarchar](64) NOT NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](128) NULL,
	[PhoneNumber] [nvarchar](32) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[PhotoUrl] [nvarchar](128) NULL,
	[Address] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](128) NULL,
	[SecurityStamp] [nvarchar](128) NULL,
	[RegistrationDate] [datetime] NULL,
	[LastLoginDate] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[LockoutEndDateTimeUtc] [datetime] NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[GetTotalNumberOfUsers]    Script Date: 4/22/2017 6:43:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[GetTotalNumberOfUsers]
AS
SELECT COUNT(*) AS TotalNumberOfUsers
FROM     dbo.Users


GO
/****** Object:  Table [dbo].[Roles]    Script Date: 4/22/2017 6:43:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](32) NOT NULL,
	[NormalizedName] [nvarchar](32) NOT NULL,
	[ConcurrencyStamp] [nvarchar](128) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersClaims]    Script Date: 4/22/2017 6:43:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersClaims](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ClaimType] [nvarchar](65) NOT NULL,
	[ClaimValue] [nvarchar](65) NOT NULL,
 CONSTRAINT [PK_UsersClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersLogins]    Script Date: 4/22/2017 6:43:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersLogins](
	[LoginProvider] [nvarchar](32) NOT NULL,
	[ProviderKey] [nvarchar](64) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ProviderDisplayName] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_UsersLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersRoles]    Script Date: 4/22/2017 6:43:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersRoles](
	[UserId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UsersRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Normalized_UserName_Email]    Script Date: 4/22/2017 6:43:09 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Normalized_UserName_Email] ON [dbo].[Users]
(
	[NormalizedUserName] ASC,
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserName_Email]    Script Date: 4/22/2017 6:43:09 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserName_Email] ON [dbo].[Users]
(
	[UserName] ASC,
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserId]    Script Date: 4/22/2017 6:43:09 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[UsersClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_EmailConfirmed]  DEFAULT ((0)) FOR [EmailConfirmed]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_PhoneNumberConfirmed]  DEFAULT ((0)) FOR [PhoneNumberConfirmed]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_LockoutEnabled]  DEFAULT ((0)) FOR [LockoutEnabled]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_TwoFactorEnabled]  DEFAULT ((0)) FOR [TwoFactorEnabled]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_AccessFailedCount]  DEFAULT ((0)) FOR [AccessFailedCount]
GO
ALTER TABLE [dbo].[UsersClaims]  WITH CHECK ADD  CONSTRAINT [FK_UsersClaims_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsersClaims] CHECK CONSTRAINT [FK_UsersClaims_Users]
GO
ALTER TABLE [dbo].[UsersLogins]  WITH CHECK ADD  CONSTRAINT [FK_UsersLogins_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsersLogins] CHECK CONSTRAINT [FK_UsersLogins_Users]
GO
ALTER TABLE [dbo].[UsersRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersRoles_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsersRoles] CHECK CONSTRAINT [FK_UsersRoles_Roles]
GO
ALTER TABLE [dbo].[UsersRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersRoles_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsersRoles] CHECK CONSTRAINT [FK_UsersRoles_Users]
GO
/****** Object:  StoredProcedure [dbo].[GetsUsers]    Script Date: 4/22/2017 6:43:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Manoltzas Georgios
-- Create date: March 22, 2017
-- Description:	A stored procedure to retrieve users from dbo.Users table.
-- =============================================
CREATE PROCEDURE [dbo].[GetsUsers]
	@PageNumber INT = 1,
	@PageSize   INT = 100,
	@SortExpression INT = 0,
	@SortDirection VARCHAR(4) = 'ASC',
	@SearchPhrase VARCHAR(32) = ''
AS
BEGIN
	SET NOCOUNT ON;
	-- SortExpression = 0 => Email
	-- SortExpression = 1 => EmailConfirmed
	-- SortExpression = 2 => PhoneNumber
	-- SortExpression = 3 => LockoutEnabled
	-- SortExpression = 3 => LockoutEndDateTimeUtc
    SELECT Id, Email, EmailConfirmed, PhoneNumber, LockoutEnabled, LockoutEndDateTimeUtc
    FROM dbo.Users
	WHERE Email LIKE '%' + @SearchPhrase + '%' OR
	      PhoneNumber LIKE '%' + @SearchPhrase + '%'
    ORDER BY
		CASE WHEN @SortExpression = 0 AND @SortDirection = 'asc' THEN Email END ASC, 
		CASE WHEN @SortExpression = 0 AND @SortDirection = 'desc' THEN Email END DESC,
		CASE WHEN @SortExpression = 1 AND @SortDirection = 'asc' THEN EmailConfirmed END ASC, 
		CASE WHEN @SortExpression = 1 AND @SortDirection = 'desc' THEN EmailConfirmed END DESC,
		CASE WHEN @SortExpression = 2 AND @SortDirection = 'asc' THEN PhoneNumber END ASC, 
		CASE WHEN @SortExpression = 2 AND @SortDirection = 'desc' THEN PhoneNumber END DESC,
		CASE WHEN @SortExpression = 3 AND @SortDirection = 'asc' THEN LockoutEnabled END ASC, 
		CASE WHEN @SortExpression = 3 AND @SortDirection = 'desc' THEN LockoutEnabled END DESC,
		CASE WHEN @SortExpression = 4 AND @SortDirection = 'asc' THEN LockoutEndDateTimeUtc END ASC, 
		CASE WHEN @SortExpression = 4 AND @SortDirection = 'desc' THEN LockoutEndDateTimeUtc END DESC
    OFFSET @PageSize * (@PageNumber - 1) ROWS
    FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);
END
GO
USE [master]
GO
ALTER DATABASE [DaartoDb] SET  READ_WRITE 
GO
