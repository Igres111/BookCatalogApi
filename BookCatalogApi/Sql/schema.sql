IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL DROP TABLE dbo.Books;
IF OBJECT_ID('dbo.Authors', 'U') IS NOT NULL DROP TABLE dbo.Authors;
GO

CREATE TABLE dbo.Authors
(
    ID   INT            NOT NULL CONSTRAINT PK_Authors PRIMARY KEY,
    Name NVARCHAR(200)  NOT NULL
);
GO

CREATE TABLE dbo.Books
(
    ID              INT            NOT NULL CONSTRAINT PK_Books PRIMARY KEY,
    Title           NVARCHAR(300)  NOT NULL,
    AuthorId       INT            NOT NULL,
    PublicationYear INT            NOT NULL
        CONSTRAINT CK_Books_PublicationYear CHECK (PublicationYear BETWEEN 1 AND YEAR(GETDATE())),
    CONSTRAINT FK_Books_Authors
        FOREIGN KEY (AuthorID) REFERENCES dbo.Authors(ID)
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE INDEX IX_Books_AuthorID ON dbo.Books(AuthorID);
GO