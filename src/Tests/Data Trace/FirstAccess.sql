declare @p1 int
set @p1=10
exec dbo.GetMajorVersion @@ver=@p1 output
select @p1

GO

declare @p2 int
set @p2=538231025
exec dbo.TempGetAppID @appName='/LM/W3SVC/1/ROOT',@appId=@p2 output
select @p2