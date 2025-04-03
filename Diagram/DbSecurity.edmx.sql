
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/02/2025 15:50:31
-- Generated from EDMX file: C:\Users\AdminSena\Desktop\DbPATH\Diagram\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DbSecurity];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(max)  NOT NULL,
    [ProfilePhotoUrl] nvarchar(max)  NOT NULL,
    [Active] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ProducerSet'
CREATE TABLE [dbo].[ProducerSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Address] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ConsumerSet'
CREATE TABLE [dbo].[ConsumerSet] (
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'FincaSet'
CREATE TABLE [dbo].[FincaSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProducerId] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Location] nvarchar(max)  NOT NULL,
    [Hectares] smallint  NOT NULL,
    [ImageUrl] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'FavoriteSet'
CREATE TABLE [dbo].[FavoriteSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ConsumerId] int  NOT NULL,
    [ProducerId] int  NOT NULL,
    [Date_Added] datetime  NOT NULL
);
GO

-- Creating table 'ProductSet'
CREATE TABLE [dbo].[ProductSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CategoryId] int  NOT NULL,
    [FavoriteId] int  NOT NULL
);
GO

-- Creating table 'CategorySet'
CREATE TABLE [dbo].[CategorySet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ReviewSet'
CREATE TABLE [dbo].[ReviewSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ConsumerId] int  NOT NULL,
    [ProductId] int  NOT NULL,
    [Date] datetime  NOT NULL,
    [Rating] int  NOT NULL,
    [Comment] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ProducerProductSet'
CREATE TABLE [dbo].[ProducerProductSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProducerId] int  NOT NULL,
    [ProductId] int  NOT NULL,
    [Price] float  NOT NULL,
    [Production] nvarchar(max)  NOT NULL,
    [Availability] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [ImageUrl] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'OrderSet'
CREATE TABLE [dbo].[OrderSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ConsumerId] int  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [Note] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'DetailOrderSet'
CREATE TABLE [dbo].[DetailOrderSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OrderId] int  NOT NULL,
    [Price] float  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [Quantity] int  NOT NULL,
    [ProducerProductId] int  NOT NULL
);
GO

-- Creating table 'ModuleSet'
CREATE TABLE [dbo].[ModuleSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'FormSet'
CREATE TABLE [dbo].[FormSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Url] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RolSet'
CREATE TABLE [dbo].[RolSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Active] bit  NOT NULL,
    [CreateAt] datetime  NOT NULL,
    [DeleteAt] datetime  NOT NULL
);
GO

-- Creating table 'PermissionSet'
CREATE TABLE [dbo].[PermissionSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'FormModuleSet'
CREATE TABLE [dbo].[FormModuleSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ModuleId] int  NOT NULL,
    [FormId] int  NOT NULL
);
GO

-- Creating table 'RolFormPermissionSet'
CREATE TABLE [dbo].[RolFormPermissionSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FormId] int  NOT NULL,
    [PermissionId] int  NOT NULL,
    [RolId] int  NOT NULL
);
GO

-- Creating table 'RolUserSet'
CREATE TABLE [dbo].[RolUserSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RolId] int  NOT NULL,
    [UserId] int  NOT NULL
);
GO

-- Creating table 'PersonSet'
CREATE TABLE [dbo].[PersonSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [PhoneNumber] nvarchar(max)  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProducerSet'
ALTER TABLE [dbo].[ProducerSet]
ADD CONSTRAINT [PK_ProducerSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ConsumerSet'
ALTER TABLE [dbo].[ConsumerSet]
ADD CONSTRAINT [PK_ConsumerSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FincaSet'
ALTER TABLE [dbo].[FincaSet]
ADD CONSTRAINT [PK_FincaSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FavoriteSet'
ALTER TABLE [dbo].[FavoriteSet]
ADD CONSTRAINT [PK_FavoriteSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProductSet'
ALTER TABLE [dbo].[ProductSet]
ADD CONSTRAINT [PK_ProductSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CategorySet'
ALTER TABLE [dbo].[CategorySet]
ADD CONSTRAINT [PK_CategorySet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ReviewSet'
ALTER TABLE [dbo].[ReviewSet]
ADD CONSTRAINT [PK_ReviewSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProducerProductSet'
ALTER TABLE [dbo].[ProducerProductSet]
ADD CONSTRAINT [PK_ProducerProductSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderSet'
ALTER TABLE [dbo].[OrderSet]
ADD CONSTRAINT [PK_OrderSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DetailOrderSet'
ALTER TABLE [dbo].[DetailOrderSet]
ADD CONSTRAINT [PK_DetailOrderSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ModuleSet'
ALTER TABLE [dbo].[ModuleSet]
ADD CONSTRAINT [PK_ModuleSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FormSet'
ALTER TABLE [dbo].[FormSet]
ADD CONSTRAINT [PK_FormSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RolSet'
ALTER TABLE [dbo].[RolSet]
ADD CONSTRAINT [PK_RolSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PermissionSet'
ALTER TABLE [dbo].[PermissionSet]
ADD CONSTRAINT [PK_PermissionSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FormModuleSet'
ALTER TABLE [dbo].[FormModuleSet]
ADD CONSTRAINT [PK_FormModuleSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RolFormPermissionSet'
ALTER TABLE [dbo].[RolFormPermissionSet]
ADD CONSTRAINT [PK_RolFormPermissionSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RolUserSet'
ALTER TABLE [dbo].[RolUserSet]
ADD CONSTRAINT [PK_RolUserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PersonSet'
ALTER TABLE [dbo].[PersonSet]
ADD CONSTRAINT [PK_PersonSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ProducerId] in table 'FincaSet'
ALTER TABLE [dbo].[FincaSet]
ADD CONSTRAINT [FK_ProducerFinca]
    FOREIGN KEY ([ProducerId])
    REFERENCES [dbo].[ProducerSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProducerFinca'
CREATE INDEX [IX_FK_ProducerFinca]
ON [dbo].[FincaSet]
    ([ProducerId]);
GO

-- Creating foreign key on [ProducerId] in table 'ProducerProductSet'
ALTER TABLE [dbo].[ProducerProductSet]
ADD CONSTRAINT [FK_ProducerProducerProduct]
    FOREIGN KEY ([ProducerId])
    REFERENCES [dbo].[ProducerSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProducerProducerProduct'
CREATE INDEX [IX_FK_ProducerProducerProduct]
ON [dbo].[ProducerProductSet]
    ([ProducerId]);
GO

-- Creating foreign key on [ProductId] in table 'ProducerProductSet'
ALTER TABLE [dbo].[ProducerProductSet]
ADD CONSTRAINT [FK_ProductProducerProduct]
    FOREIGN KEY ([ProductId])
    REFERENCES [dbo].[ProductSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductProducerProduct'
CREATE INDEX [IX_FK_ProductProducerProduct]
ON [dbo].[ProducerProductSet]
    ([ProductId]);
GO

-- Creating foreign key on [ConsumerId] in table 'FavoriteSet'
ALTER TABLE [dbo].[FavoriteSet]
ADD CONSTRAINT [FK_ConsumerFavorite]
    FOREIGN KEY ([ConsumerId])
    REFERENCES [dbo].[ConsumerSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConsumerFavorite'
CREATE INDEX [IX_FK_ConsumerFavorite]
ON [dbo].[FavoriteSet]
    ([ConsumerId]);
GO

-- Creating foreign key on [ConsumerId] in table 'ReviewSet'
ALTER TABLE [dbo].[ReviewSet]
ADD CONSTRAINT [FK_ConsumerReview]
    FOREIGN KEY ([ConsumerId])
    REFERENCES [dbo].[ConsumerSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConsumerReview'
CREATE INDEX [IX_FK_ConsumerReview]
ON [dbo].[ReviewSet]
    ([ConsumerId]);
GO

-- Creating foreign key on [ConsumerId] in table 'OrderSet'
ALTER TABLE [dbo].[OrderSet]
ADD CONSTRAINT [FK_ConsumerOrder]
    FOREIGN KEY ([ConsumerId])
    REFERENCES [dbo].[ConsumerSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConsumerOrder'
CREATE INDEX [IX_FK_ConsumerOrder]
ON [dbo].[OrderSet]
    ([ConsumerId]);
GO

-- Creating foreign key on [OrderId] in table 'DetailOrderSet'
ALTER TABLE [dbo].[DetailOrderSet]
ADD CONSTRAINT [FK_OrderDetailOrder]
    FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[OrderSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderDetailOrder'
CREATE INDEX [IX_FK_OrderDetailOrder]
ON [dbo].[DetailOrderSet]
    ([OrderId]);
GO

-- Creating foreign key on [ProductId] in table 'ReviewSet'
ALTER TABLE [dbo].[ReviewSet]
ADD CONSTRAINT [FK_ProductReview]
    FOREIGN KEY ([ProductId])
    REFERENCES [dbo].[ProductSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductReview'
CREATE INDEX [IX_FK_ProductReview]
ON [dbo].[ReviewSet]
    ([ProductId]);
GO

-- Creating foreign key on [CategoryId] in table 'ProductSet'
ALTER TABLE [dbo].[ProductSet]
ADD CONSTRAINT [FK_CategoryProduct]
    FOREIGN KEY ([CategoryId])
    REFERENCES [dbo].[CategorySet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CategoryProduct'
CREATE INDEX [IX_FK_CategoryProduct]
ON [dbo].[ProductSet]
    ([CategoryId]);
GO

-- Creating foreign key on [ProducerProductId] in table 'DetailOrderSet'
ALTER TABLE [dbo].[DetailOrderSet]
ADD CONSTRAINT [FK_ProducerProductDetailOrder]
    FOREIGN KEY ([ProducerProductId])
    REFERENCES [dbo].[ProducerProductSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProducerProductDetailOrder'
CREATE INDEX [IX_FK_ProducerProductDetailOrder]
ON [dbo].[DetailOrderSet]
    ([ProducerProductId]);
GO

-- Creating foreign key on [FavoriteId] in table 'ProductSet'
ALTER TABLE [dbo].[ProductSet]
ADD CONSTRAINT [FK_FavoriteProduct]
    FOREIGN KEY ([FavoriteId])
    REFERENCES [dbo].[FavoriteSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FavoriteProduct'
CREATE INDEX [IX_FK_FavoriteProduct]
ON [dbo].[ProductSet]
    ([FavoriteId]);
GO

-- Creating foreign key on [ModuleId] in table 'FormModuleSet'
ALTER TABLE [dbo].[FormModuleSet]
ADD CONSTRAINT [FK_ModuleFormModule]
    FOREIGN KEY ([ModuleId])
    REFERENCES [dbo].[ModuleSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ModuleFormModule'
CREATE INDEX [IX_FK_ModuleFormModule]
ON [dbo].[FormModuleSet]
    ([ModuleId]);
GO

-- Creating foreign key on [FormId] in table 'FormModuleSet'
ALTER TABLE [dbo].[FormModuleSet]
ADD CONSTRAINT [FK_FormFormModule]
    FOREIGN KEY ([FormId])
    REFERENCES [dbo].[FormSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormFormModule'
CREATE INDEX [IX_FK_FormFormModule]
ON [dbo].[FormModuleSet]
    ([FormId]);
GO

-- Creating foreign key on [FormId] in table 'RolFormPermissionSet'
ALTER TABLE [dbo].[RolFormPermissionSet]
ADD CONSTRAINT [FK_FormRolFormPermission]
    FOREIGN KEY ([FormId])
    REFERENCES [dbo].[FormSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormRolFormPermission'
CREATE INDEX [IX_FK_FormRolFormPermission]
ON [dbo].[RolFormPermissionSet]
    ([FormId]);
GO

-- Creating foreign key on [PermissionId] in table 'RolFormPermissionSet'
ALTER TABLE [dbo].[RolFormPermissionSet]
ADD CONSTRAINT [FK_PermissionRolFormPermission]
    FOREIGN KEY ([PermissionId])
    REFERENCES [dbo].[PermissionSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PermissionRolFormPermission'
CREATE INDEX [IX_FK_PermissionRolFormPermission]
ON [dbo].[RolFormPermissionSet]
    ([PermissionId]);
GO

-- Creating foreign key on [RolId] in table 'RolFormPermissionSet'
ALTER TABLE [dbo].[RolFormPermissionSet]
ADD CONSTRAINT [FK_RolRolFormPermission]
    FOREIGN KEY ([RolId])
    REFERENCES [dbo].[RolSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RolRolFormPermission'
CREATE INDEX [IX_FK_RolRolFormPermission]
ON [dbo].[RolFormPermissionSet]
    ([RolId]);
GO

-- Creating foreign key on [RolId] in table 'RolUserSet'
ALTER TABLE [dbo].[RolUserSet]
ADD CONSTRAINT [FK_RolRolUser]
    FOREIGN KEY ([RolId])
    REFERENCES [dbo].[RolSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RolRolUser'
CREATE INDEX [IX_FK_RolRolUser]
ON [dbo].[RolUserSet]
    ([RolId]);
GO

-- Creating foreign key on [User_Id] in table 'PersonSet'
ALTER TABLE [dbo].[PersonSet]
ADD CONSTRAINT [FK_PersonUser]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PersonUser'
CREATE INDEX [IX_FK_PersonUser]
ON [dbo].[PersonSet]
    ([User_Id]);
GO

-- Creating foreign key on [UserId] in table 'RolUserSet'
ALTER TABLE [dbo].[RolUserSet]
ADD CONSTRAINT [FK_UserRolUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRolUser'
CREATE INDEX [IX_FK_UserRolUser]
ON [dbo].[RolUserSet]
    ([UserId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------