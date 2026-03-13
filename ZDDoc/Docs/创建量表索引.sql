-- =============================================
-- 脚本功能: 为 CheckFile 表的 CompanyId 字段创建索引
-- 适用数据库: SQL Server
-- =============================================

-- 检查索引是否已存在，如果存在则先删除（防止重复执行报错）
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CheckFile_CompanyId')
BEGIN
    DROP INDEX [IX_CheckFile_CompanyId] ON [dbo].[CheckFile];
    PRINT '旧索引已删除，正在重新创建...';
END
GO

-- 创建非聚集索引
-- 命名规范建议：IX_表名_列名
CREATE NONCLUSTERED INDEX [IX_CheckFile_CompanyId]
ON [dbo].[CheckFile] ([CompanyId] ASC)
WITH (
    PAD_INDEX = OFF, 
    STATISTICS_NORECOMPUTE = OFF, 
    SORT_IN_TEMPDB = OFF, 
    DROP_EXISTING = OFF, 
    ONLINE = OFF, -- 如果在生产环境且表很大，可考虑设为 ON (需企业版)
    ALLOW_ROW_LOCKS = ON, 
    ALLOW_PAGE_LOCKS = ON
   --OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY];
GO

PRINT '索引 IX_CheckFile_CompanyId 创建成功！';



-- =============================================
-- 脚本功能: 为 CheckFile 表的 CompanyId 字段创建索引
-- 适用数据库: SQL Server
-- =============================================

-- 检查索引是否已存在，如果存在则先删除（防止重复执行报错）
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CheckFile_AttId')
BEGIN
    DROP INDEX [IX_CheckFile_AttId] ON [dbo].[CheckFile];
    PRINT '旧索引已删除，正在重新创建...';
END
GO

-- 创建非聚集索引
-- 命名规范建议：IX_表名_列名
CREATE NONCLUSTERED INDEX [IX_CheckFile_AttId]
ON [dbo].[CheckFile] ([AttId] ASC)
WITH (
    PAD_INDEX = OFF, 
    STATISTICS_NORECOMPUTE = OFF, 
    SORT_IN_TEMPDB = OFF, 
    DROP_EXISTING = OFF, 
    ONLINE = OFF, -- 如果在生产环境且表很大，可考虑设为 ON (需企业版)
    ALLOW_ROW_LOCKS = ON, 
    ALLOW_PAGE_LOCKS = ON
   --OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY];
GO

PRINT '索引 IX_CheckFile_AttId 创建成功！';


-----------------------------------------------------------------------------------------------
-- =============================================
-- 脚本功能: 为 CheckFile 表的 CompanyId 字段创建索引
-- 适用数据库: SQL Server
-- =============================================

-- 检查索引是否已存在，如果存在则先删除（防止重复执行报错）
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_EnterpriseFile_CompanyId')
BEGIN
    DROP INDEX [IX_EnterpriseFile_CompanyId] ON [dbo].[EnterpriseFile];
    PRINT '旧索引已删除，正在重新创建...';
END
GO

-- 创建非聚集索引
-- 命名规范建议：IX_表名_列名
CREATE NONCLUSTERED INDEX [IX_EnterpriseFile_CompanyId]
ON [dbo].[EnterpriseFile] ([CompanyId] ASC)
WITH (
    PAD_INDEX = OFF, 
    STATISTICS_NORECOMPUTE = OFF, 
    SORT_IN_TEMPDB = OFF, 
    DROP_EXISTING = OFF, 
    ONLINE = OFF, -- 如果在生产环境且表很大，可考虑设为 ON (需企业版)
    ALLOW_ROW_LOCKS = ON, 
    ALLOW_PAGE_LOCKS = ON
   --OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY];
GO

PRINT '索引 IX_EnterpriseFile_CompanyId 创建成功！';



-- =============================================
-- 脚本功能: 为 CheckFile 表的 CompanyId 字段创建索引
-- 适用数据库: SQL Server
-- =============================================

-- 检查索引是否已存在，如果存在则先删除（防止重复执行报错）
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_EnterpriseFile_AttId')
BEGIN
    DROP INDEX [IX_EnterpriseFile_AttId] ON [dbo].[EnterpriseFile];
    PRINT '旧索引已删除，正在重新创建...';
END
GO

-- 创建非聚集索引
-- 命名规范建议：IX_表名_列名
CREATE NONCLUSTERED INDEX [IX_EnterpriseFile_AttId]
ON [dbo].[EnterpriseFile] ([AttId] ASC)
WITH (
    PAD_INDEX = OFF, 
    STATISTICS_NORECOMPUTE = OFF, 
    SORT_IN_TEMPDB = OFF, 
    DROP_EXISTING = OFF, 
    ONLINE = OFF, -- 如果在生产环境且表很大，可考虑设为 ON (需企业版)
    ALLOW_ROW_LOCKS = ON, 
    ALLOW_PAGE_LOCKS = ON
   --OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY];
GO

PRINT '索引 IX_EnterpriseFile_AttId 创建成功！';