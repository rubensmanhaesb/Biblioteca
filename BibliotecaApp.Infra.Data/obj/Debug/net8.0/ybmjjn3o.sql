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

CREATE TABLE [Assunto] (
    [CodAs] int NOT NULL DEFAULT (NEXT VALUE FOR dbo.AssuntoSequence),
    [Descricao] nvarchar(20) NULL,
    CONSTRAINT [PK_Assunto] PRIMARY KEY ([CodAs])
);
GO

CREATE TABLE [Autor] (
    [CodAu] int NOT NULL DEFAULT (NEXT VALUE FOR dbo.AutorSequence),
    [Nome] nvarchar(40) NOT NULL,
    CONSTRAINT [PK_Autor] PRIMARY KEY ([CodAu])
);
GO

CREATE TABLE [Livro] (
    [Codl] int NOT NULL DEFAULT (NEXT VALUE FOR dbo.LivroSequence),
    [Titulo] nvarchar(40) NOT NULL,
    [Editora] nvarchar(40) NULL,
    [Edicao] int NULL,
    [AnoPublicacao] nvarchar(4) NULL,
    CONSTRAINT [PK_Livro] PRIMARY KEY ([Codl])
);
GO

CREATE TABLE [LivroAssunto] (
    [LivroCodl] int NOT NULL,
    [AssuntoCodAs] int NOT NULL,
    CONSTRAINT [PK_LivroAssunto] PRIMARY KEY ([LivroCodl], [AssuntoCodAs]),
    CONSTRAINT [FK_LivroAssunto_Assunto_AssuntoCodAs] FOREIGN KEY ([AssuntoCodAs]) REFERENCES [Assunto] ([CodAs]) ON DELETE CASCADE,
    CONSTRAINT [FK_LivroAssunto_Livro_LivroCodl] FOREIGN KEY ([LivroCodl]) REFERENCES [Livro] ([Codl]) ON DELETE CASCADE
);
GO

CREATE TABLE [LivroAutor] (
    [LivroCodl] int NOT NULL,
    [AutorCodAu] int NOT NULL,
    CONSTRAINT [PK_LivroAutor] PRIMARY KEY ([LivroCodl], [AutorCodAu]),
    CONSTRAINT [FK_LivroAutor_Autor_AutorCodAu] FOREIGN KEY ([AutorCodAu]) REFERENCES [Autor] ([CodAu]) ON DELETE CASCADE,
    CONSTRAINT [FK_LivroAutor_Livro_LivroCodl] FOREIGN KEY ([LivroCodl]) REFERENCES [Livro] ([Codl]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Assunto_CodAs] ON [Assunto] ([CodAs]);
GO

CREATE UNIQUE INDEX [IX_Autor_CodAu] ON [Autor] ([CodAu]);
GO

CREATE UNIQUE INDEX [IX_Livro_Codl] ON [Livro] ([Codl]);
GO

CREATE INDEX [IX_LivroAssunto_AssuntoCodAs] ON [LivroAssunto] ([AssuntoCodAs]);
GO

CREATE INDEX [IX_LivroAssunto_LivroCodl] ON [LivroAssunto] ([LivroCodl]);
GO

CREATE INDEX [IX_LivroAutor_AutorCodAu] ON [LivroAutor] ([AutorCodAu]);
GO

CREATE INDEX [IX_LivroAutor_LivroCodl] ON [LivroAutor] ([LivroCodl]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241105050229_Initial', N'8.0.10');
GO

COMMIT;
GO

