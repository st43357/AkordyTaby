
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/18/2019 20:10:13
-- Generated from EDMX file: C:\Users\krajt\Desktop\DiplomPrace\DiplomovaPrace\Models\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DefaultConnection];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_PrispevekHodnoceniPrispevku]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HodnoceniPrispevku] DROP CONSTRAINT [FK_PrispevekHodnoceniPrispevku];
GO
IF OBJECT_ID(N'[dbo].[FK_PrispevekUzivatel]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Prispevky] DROP CONSTRAINT [FK_PrispevekUzivatel];
GO
IF OBJECT_ID(N'[dbo].[FK_UzivatelKomentar]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Komentare] DROP CONSTRAINT [FK_UzivatelKomentar];
GO
IF OBJECT_ID(N'[dbo].[FK_UzivatelKomentar1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Komentare] DROP CONSTRAINT [FK_UzivatelKomentar1];
GO
IF OBJECT_ID(N'[dbo].[FK_UzivatelZprava]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Zpravy] DROP CONSTRAINT [FK_UzivatelZprava];
GO
IF OBJECT_ID(N'[dbo].[FK_UzivatelZprava1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Zpravy] DROP CONSTRAINT [FK_UzivatelZprava1];
GO
IF OBJECT_ID(N'[dbo].[FK_UzivatelZadost]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Zadosti] DROP CONSTRAINT [FK_UzivatelZadost];
GO
IF OBJECT_ID(N'[dbo].[FK_UzivatelZadost1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Zadosti] DROP CONSTRAINT [FK_UzivatelZadost1];
GO
IF OBJECT_ID(N'[dbo].[FK_UzivatelClanek]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Clanky] DROP CONSTRAINT [FK_UzivatelClanek];
GO
IF OBJECT_ID(N'[dbo].[FK_InterpretSpravceInterpreta]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpravciInterpretu] DROP CONSTRAINT [FK_InterpretSpravceInterpreta];
GO
IF OBJECT_ID(N'[dbo].[FK_UzivatelSpravceInterpreta]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpravciInterpretu] DROP CONSTRAINT [FK_UzivatelSpravceInterpreta];
GO
IF OBJECT_ID(N'[dbo].[FK_PrispevekInterpret]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Prispevky] DROP CONSTRAINT [FK_PrispevekInterpret];
GO
IF OBJECT_ID(N'[dbo].[FK_KomentarClanek]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Komentare] DROP CONSTRAINT [FK_KomentarClanek];
GO
IF OBJECT_ID(N'[dbo].[FK_HodnoceniPrispevkuUzivatel]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HodnoceniPrispevku] DROP CONSTRAINT [FK_HodnoceniPrispevkuUzivatel];
GO
IF OBJECT_ID(N'[dbo].[FK_ZadostInterpret]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Zadosti] DROP CONSTRAINT [FK_ZadostInterpret];
GO
IF OBJECT_ID(N'[dbo].[FK_PrispevekZadost]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Zadosti] DROP CONSTRAINT [FK_PrispevekZadost];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Prispevky]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Prispevky];
GO
IF OBJECT_ID(N'[dbo].[HodnoceniPrispevku]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HodnoceniPrispevku];
GO
IF OBJECT_ID(N'[dbo].[Komentare]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Komentare];
GO
IF OBJECT_ID(N'[dbo].[Zpravy]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Zpravy];
GO
IF OBJECT_ID(N'[dbo].[Zadosti]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Zadosti];
GO
IF OBJECT_ID(N'[dbo].[Interpreti]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Interpreti];
GO
IF OBJECT_ID(N'[dbo].[SpravciInterpretu]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SpravciInterpretu];
GO
IF OBJECT_ID(N'[dbo].[Clanky]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clanky];
GO
IF OBJECT_ID(N'[dbo].[Uzivatele]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Uzivatele];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Prispevky'
CREATE TABLE [dbo].[Prispevky] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NazevPisne] nvarchar(max)  NOT NULL,
    [Obsah] nvarchar(max)  NOT NULL,
    [DatumPridani] datetime  NOT NULL,
    [TypPrispevku] int  NOT NULL,
    [UzivatelId] int  NOT NULL,
    [InterpretId] int  NOT NULL,
    [Schvalen] bit  NOT NULL
);
GO

-- Creating table 'HodnoceniPrispevku'
CREATE TABLE [dbo].[HodnoceniPrispevku] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Hodnoceni] int  NOT NULL,
    [PrispevekId] int  NOT NULL,
    [UzivatelId] int  NOT NULL
);
GO

-- Creating table 'Komentare'
CREATE TABLE [dbo].[Komentare] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [TypKomentare] int  NOT NULL,
    [UzivatelOdId] int  NOT NULL,
    [UzivatelKomuId] int  NOT NULL,
    [ClanekId] int  NOT NULL,
    [DatumPridani] datetime  NOT NULL
);
GO

-- Creating table 'Zpravy'
CREATE TABLE [dbo].[Zpravy] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CasOdeslani] datetime  NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [UzivatelOdId] int  NOT NULL,
    [UzivatelKomuId] int  NOT NULL
);
GO

-- Creating table 'Zadosti'
CREATE TABLE [dbo].[Zadosti] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Komentar] nvarchar(max)  NULL,
    [TypZadosti] int  NOT NULL,
    [StavZadosti] int  NOT NULL,
    [UzivatelOdId] int  NOT NULL,
    [UzivatelKomuId] int  NOT NULL,
    [InterpretId] int  NOT NULL,
    [PrispevekId] int  NOT NULL
);
GO

-- Creating table 'Interpreti'
CREATE TABLE [dbo].[Interpreti] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Nazev] nvarchar(max)  NOT NULL,
    [DatumVytvoreni] datetime  NOT NULL
);
GO

-- Creating table 'SpravciInterpretu'
CREATE TABLE [dbo].[SpravciInterpretu] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [InterpretId] int  NOT NULL,
    [UzivatelId] int  NOT NULL
);
GO

-- Creating table 'Clanky'
CREATE TABLE [dbo].[Clanky] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Titulek] nvarchar(max)  NOT NULL,
    [Popisek] nvarchar(max)  NOT NULL,
    [DatumVytvoreni] datetime  NOT NULL,
    [Obsah] nvarchar(max)  NOT NULL,
    [Kategorie] int  NOT NULL,
    [UzivatelId] int  NOT NULL
);
GO

-- Creating table 'Uzivatele'
CREATE TABLE [dbo].[Uzivatele] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Prezdivka] nvarchar(max)  NOT NULL,
    [Heslo] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Jmeno] nvarchar(max)  NOT NULL,
    [Prijmeni] nvarchar(max)  NOT NULL,
    [Mesto] nvarchar(max)  NOT NULL,
    [PosledniAktivita] datetime  NOT NULL,
    [Avatar] nvarchar(max)  NOT NULL,
    [DatumNarozeni] datetime  NOT NULL,
    [DatumRegistrace] datetime  NOT NULL,
    [Role] int  NOT NULL,
    [Hodnost] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Prispevky'
ALTER TABLE [dbo].[Prispevky]
ADD CONSTRAINT [PK_Prispevky]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'HodnoceniPrispevku'
ALTER TABLE [dbo].[HodnoceniPrispevku]
ADD CONSTRAINT [PK_HodnoceniPrispevku]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Komentare'
ALTER TABLE [dbo].[Komentare]
ADD CONSTRAINT [PK_Komentare]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Zpravy'
ALTER TABLE [dbo].[Zpravy]
ADD CONSTRAINT [PK_Zpravy]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Zadosti'
ALTER TABLE [dbo].[Zadosti]
ADD CONSTRAINT [PK_Zadosti]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Interpreti'
ALTER TABLE [dbo].[Interpreti]
ADD CONSTRAINT [PK_Interpreti]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SpravciInterpretu'
ALTER TABLE [dbo].[SpravciInterpretu]
ADD CONSTRAINT [PK_SpravciInterpretu]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Clanky'
ALTER TABLE [dbo].[Clanky]
ADD CONSTRAINT [PK_Clanky]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Uzivatele'
ALTER TABLE [dbo].[Uzivatele]
ADD CONSTRAINT [PK_Uzivatele]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [PrispevekId] in table 'HodnoceniPrispevku'
ALTER TABLE [dbo].[HodnoceniPrispevku]
ADD CONSTRAINT [FK_PrispevekHodnoceniPrispevku]
    FOREIGN KEY ([PrispevekId])
    REFERENCES [dbo].[Prispevky]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PrispevekHodnoceniPrispevku'
CREATE INDEX [IX_FK_PrispevekHodnoceniPrispevku]
ON [dbo].[HodnoceniPrispevku]
    ([PrispevekId]);
GO

-- Creating foreign key on [UzivatelId] in table 'Prispevky'
ALTER TABLE [dbo].[Prispevky]
ADD CONSTRAINT [FK_PrispevekUzivatel]
    FOREIGN KEY ([UzivatelId])
    REFERENCES [dbo].[Uzivatele]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PrispevekUzivatel'
CREATE INDEX [IX_FK_PrispevekUzivatel]
ON [dbo].[Prispevky]
    ([UzivatelId]);
GO

-- Creating foreign key on [UzivatelOdId] in table 'Komentare'
ALTER TABLE [dbo].[Komentare]
ADD CONSTRAINT [FK_UzivatelKomentar]
    FOREIGN KEY ([UzivatelOdId])
    REFERENCES [dbo].[Uzivatele]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UzivatelKomentar'
CREATE INDEX [IX_FK_UzivatelKomentar]
ON [dbo].[Komentare]
    ([UzivatelOdId]);
GO

-- Creating foreign key on [UzivatelKomuId] in table 'Komentare'
ALTER TABLE [dbo].[Komentare]
ADD CONSTRAINT [FK_UzivatelKomentar1]
    FOREIGN KEY ([UzivatelKomuId])
    REFERENCES [dbo].[Uzivatele]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UzivatelKomentar1'
CREATE INDEX [IX_FK_UzivatelKomentar1]
ON [dbo].[Komentare]
    ([UzivatelKomuId]);
GO

-- Creating foreign key on [UzivatelOdId] in table 'Zpravy'
ALTER TABLE [dbo].[Zpravy]
ADD CONSTRAINT [FK_UzivatelZprava]
    FOREIGN KEY ([UzivatelOdId])
    REFERENCES [dbo].[Uzivatele]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UzivatelZprava'
CREATE INDEX [IX_FK_UzivatelZprava]
ON [dbo].[Zpravy]
    ([UzivatelOdId]);
GO

-- Creating foreign key on [UzivatelKomuId] in table 'Zpravy'
ALTER TABLE [dbo].[Zpravy]
ADD CONSTRAINT [FK_UzivatelZprava1]
    FOREIGN KEY ([UzivatelKomuId])
    REFERENCES [dbo].[Uzivatele]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UzivatelZprava1'
CREATE INDEX [IX_FK_UzivatelZprava1]
ON [dbo].[Zpravy]
    ([UzivatelKomuId]);
GO

-- Creating foreign key on [UzivatelOdId] in table 'Zadosti'
ALTER TABLE [dbo].[Zadosti]
ADD CONSTRAINT [FK_UzivatelZadost]
    FOREIGN KEY ([UzivatelOdId])
    REFERENCES [dbo].[Uzivatele]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UzivatelZadost'
CREATE INDEX [IX_FK_UzivatelZadost]
ON [dbo].[Zadosti]
    ([UzivatelOdId]);
GO

-- Creating foreign key on [UzivatelKomuId] in table 'Zadosti'
ALTER TABLE [dbo].[Zadosti]
ADD CONSTRAINT [FK_UzivatelZadost1]
    FOREIGN KEY ([UzivatelKomuId])
    REFERENCES [dbo].[Uzivatele]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UzivatelZadost1'
CREATE INDEX [IX_FK_UzivatelZadost1]
ON [dbo].[Zadosti]
    ([UzivatelKomuId]);
GO

-- Creating foreign key on [UzivatelId] in table 'Clanky'
ALTER TABLE [dbo].[Clanky]
ADD CONSTRAINT [FK_UzivatelClanek]
    FOREIGN KEY ([UzivatelId])
    REFERENCES [dbo].[Uzivatele]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UzivatelClanek'
CREATE INDEX [IX_FK_UzivatelClanek]
ON [dbo].[Clanky]
    ([UzivatelId]);
GO

-- Creating foreign key on [InterpretId] in table 'SpravciInterpretu'
ALTER TABLE [dbo].[SpravciInterpretu]
ADD CONSTRAINT [FK_InterpretSpravceInterpreta]
    FOREIGN KEY ([InterpretId])
    REFERENCES [dbo].[Interpreti]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_InterpretSpravceInterpreta'
CREATE INDEX [IX_FK_InterpretSpravceInterpreta]
ON [dbo].[SpravciInterpretu]
    ([InterpretId]);
GO

-- Creating foreign key on [UzivatelId] in table 'SpravciInterpretu'
ALTER TABLE [dbo].[SpravciInterpretu]
ADD CONSTRAINT [FK_UzivatelSpravceInterpreta]
    FOREIGN KEY ([UzivatelId])
    REFERENCES [dbo].[Uzivatele]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UzivatelSpravceInterpreta'
CREATE INDEX [IX_FK_UzivatelSpravceInterpreta]
ON [dbo].[SpravciInterpretu]
    ([UzivatelId]);
GO

-- Creating foreign key on [InterpretId] in table 'Prispevky'
ALTER TABLE [dbo].[Prispevky]
ADD CONSTRAINT [FK_PrispevekInterpret]
    FOREIGN KEY ([InterpretId])
    REFERENCES [dbo].[Interpreti]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PrispevekInterpret'
CREATE INDEX [IX_FK_PrispevekInterpret]
ON [dbo].[Prispevky]
    ([InterpretId]);
GO

-- Creating foreign key on [ClanekId] in table 'Komentare'
ALTER TABLE [dbo].[Komentare]
ADD CONSTRAINT [FK_KomentarClanek]
    FOREIGN KEY ([ClanekId])
    REFERENCES [dbo].[Clanky]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_KomentarClanek'
CREATE INDEX [IX_FK_KomentarClanek]
ON [dbo].[Komentare]
    ([ClanekId]);
GO

-- Creating foreign key on [UzivatelId] in table 'HodnoceniPrispevku'
ALTER TABLE [dbo].[HodnoceniPrispevku]
ADD CONSTRAINT [FK_HodnoceniPrispevkuUzivatel]
    FOREIGN KEY ([UzivatelId])
    REFERENCES [dbo].[Uzivatele]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_HodnoceniPrispevkuUzivatel'
CREATE INDEX [IX_FK_HodnoceniPrispevkuUzivatel]
ON [dbo].[HodnoceniPrispevku]
    ([UzivatelId]);
GO

-- Creating foreign key on [InterpretId] in table 'Zadosti'
ALTER TABLE [dbo].[Zadosti]
ADD CONSTRAINT [FK_ZadostInterpret]
    FOREIGN KEY ([InterpretId])
    REFERENCES [dbo].[Interpreti]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ZadostInterpret'
CREATE INDEX [IX_FK_ZadostInterpret]
ON [dbo].[Zadosti]
    ([InterpretId]);
GO

-- Creating foreign key on [PrispevekId] in table 'Zadosti'
ALTER TABLE [dbo].[Zadosti]
ADD CONSTRAINT [FK_PrispevekZadost]
    FOREIGN KEY ([PrispevekId])
    REFERENCES [dbo].[Prispevky]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PrispevekZadost'
CREATE INDEX [IX_FK_PrispevekZadost]
ON [dbo].[Zadosti]
    ([PrispevekId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------