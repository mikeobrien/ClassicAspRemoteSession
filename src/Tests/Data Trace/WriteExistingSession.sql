declare @p2 varbinary(7000)
set @p2=0x14000000010003000000FFFFFFFF046E616D650361676505707269636504000000090000001200000001024564022C000000090000000000404140FF
declare @p3 bit
set @p3=0
declare @p4 int
set @p4=0
declare @p5 int
set @p5=3
declare @p6 int
set @p6=0
exec dbo.TempGetStateItemExclusive3 @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=@p2 output,@locked=@p3 output,@lockAge=@p4 output,@lockCookie=@p5 output,@actionFlags=@p6 output
select @p2, @p3, @p4, @p5, @p6

GO

exec dbo.TempUpdateStateItemShort @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=0x14000000010003000000FFFFFFFF046E616D650361676505707269636504000000090000001200000001024564022C000000090000000000404140FF,@timeout=20,@lockCookie=3