IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [dbo].[Assunto] (
    [CodAs] int NOT NULL IDENTITY,
    [Descricao] nvarchar(20) NULL,
    CONSTRAINT [PK_Assunto] PRIMARY KEY ([CodAs])
);
GO

CREATE TABLE [dbo].[Autor] (
    [CodAu] int NOT NULL IDENTITY,
    [Nome] nvarchar(40) NOT NULL,
    CONSTRAINT [PK_Autor] PRIMARY KEY ([CodAu])
);
GO

CREATE TABLE [dbo].[Livro] (
    [Codl] int NOT NULL IDENTITY,
    [Titulo] nvarchar(40) NOT NULL,
    [Editora] nvarchar(40) NULL,
    [Edicao] int NULL,
    [AnoPublicacao] nvarchar(4) NULL,
    CONSTRAINT [PK_Livro] PRIMARY KEY ([Codl])
);
GO

CREATE TABLE [dbo].[LivroAssunto] (
    [LivroCodl] int NOT NULL,
    [AssuntoCodAs] int NOT NULL,
    CONSTRAINT [PK_LivroAssunto] PRIMARY KEY ([LivroCodl], [AssuntoCodAs]),
    CONSTRAINT [FK_LivroAssunto_Assunto_AssuntoCodAs] FOREIGN KEY ([AssuntoCodAs]) REFERENCES [dbo].[Assunto] ([CodAs]) ON DELETE CASCADE,
    CONSTRAINT [FK_LivroAssunto_Livro_LivroCodl] FOREIGN KEY ([LivroCodl]) REFERENCES [dbo].[Livro] ([Codl]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[LivroAutor] (
    [LivroCodl] int NOT NULL,
    [AutorCodAu] int NOT NULL,
    CONSTRAINT [PK_LivroAutor] PRIMARY KEY ([LivroCodl], [AutorCodAu]),
    CONSTRAINT [FK_LivroAutor_Autor_AutorCodAu] FOREIGN KEY ([AutorCodAu]) REFERENCES [dbo].[Autor] ([CodAu]) ON DELETE CASCADE,
    CONSTRAINT [FK_LivroAutor_Livro_LivroCodl] FOREIGN KEY ([LivroCodl]) REFERENCES [dbo].[Livro] ([Codl]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[PrecoLivro] (
    [Codp] int NOT NULL IDENTITY,
    [LivroCodl] int NOT NULL,
    [TipoCompra] int NOT NULL,
    [Valor] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_PrecoLivro] PRIMARY KEY ([Codp]),
    CONSTRAINT [FK_PrecoLivro_Livro_LivroCodl] FOREIGN KEY ([LivroCodl]) REFERENCES [dbo].[Livro] ([Codl]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Assunto_CodAs] ON [dbo].[Assunto] ([CodAs]);
GO

CREATE UNIQUE INDEX [IX_Autor_CodAu] ON [dbo].[Autor] ([CodAu]);
GO

CREATE UNIQUE INDEX [IX_Livro_Codl] ON [dbo].[Livro] ([Codl]);
GO

CREATE INDEX [IX_LivroAssunto_AssuntoCodAs] ON [dbo].[LivroAssunto] ([AssuntoCodAs]);
GO

CREATE INDEX [IX_LivroAssunto_LivroCodl] ON [dbo].[LivroAssunto] ([LivroCodl]);
GO

CREATE INDEX [IX_LivroAutor_AutorCodAu] ON [dbo].[LivroAutor] ([AutorCodAu]);
GO

CREATE INDEX [IX_LivroAutor_LivroCodl] ON [dbo].[LivroAutor] ([LivroCodl]);
GO

CREATE INDEX [IX_PrecoLivro_LivroCodl] ON [dbo].[PrecoLivro] ([LivroCodl]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241106180221_Initial', N'8.0.10');
GO

COMMIT;
GO

