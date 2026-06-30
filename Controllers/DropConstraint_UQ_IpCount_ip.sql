-- =============================================
-- Script: Xoá UNIQUE constraint UQ_IpCount_ip trên bảng IpCount
-- Database: SQL Server
-- =============================================

-- Kiểm tra constraint có tồn tại trước khi xoá (tránh lỗi nếu đã xoá rồi)
IF EXISTS (
    SELECT 1
    FROM sys.key_constraints
    WHERE name = 'UQ_IpCount_ip'
      AND parent_object_id = OBJECT_ID('dbo.IpCount')
)
BEGIN
    ALTER TABLE IpCount
        DROP CONSTRAINT UQ_IpCount_ip;
END
GO
