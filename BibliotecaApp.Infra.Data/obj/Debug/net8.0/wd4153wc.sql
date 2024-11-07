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

CREATE SEQUENCE [dbo].[AssuntoSequence] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

CREATE SEQUENCE [dbo].[AutorSequence] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

CREATE SEQUENCE [dbo].[LivroSequence] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

CREATE TABLE [dbo].[Assunto] (
    [CodAs] int NOT NULL DEFAULT (NEXT VALUE FOR dbo.AssuntoSequence),
    [Descricao] nvarchar(20) NULL,
    CONSTRAINT [PK_Assunto] PRIMARY KEY ([CodAs])
);
GO

CREATE TABLE [dbo].[Autor] (
    [CodAu] int NOT NULL DEFAULT (NEXT VALUE FOR dbo.AutorSequence),
    [Nome] nvarchar(40) NOT NULL,
    CONSTRAINT [PK_Autor] PRIMARY KEY ([CodAu])
);
GO

CREATE TABLE [dbo].[Livro] (
    [Codl] int NOT NULL DEFAULT (NEXT VALUE FOR dbo.LivroSequence),
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

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241105080233_Initial', N'8.0.10');
GO

COMMIT;
GO

