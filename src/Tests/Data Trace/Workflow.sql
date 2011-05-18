-- Initialize:
exec dbo.TempGetAppID @appName='/LM/W3SVC/1/ROOT',@appId=538231025 output

-- Get a session

-- If not exists
exec dbo.TempGetStateItemExclusive3 @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=NULL output,@locked=NULL output,@lockAge=NULL output,@lockCookie=NULL output,@actionFlags=NULL output
exec dbo.TempInsertUninitializedItem @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=0x140000000000FF,@timeout=20
exec dbo.TempGetStateItemExclusive3 @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=0x140000000000FF output,@locked=0 output,@lockAge=0 output,@lockCookie=2 output,@actionFlags=1 output

exec dbo.TempReleaseStateItemExclusive @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@lockCookie=2

-- If Exists
exec dbo.TempGetStateItemExclusive3 @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=0x140000000000FF output,@locked=0 output,@lockAge=0 output,@lockCookie=3 output,@actionFlags=0 output

exec dbo.TempReleaseStateItemExclusive @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@lockCookie=3

-- Write a session

-- If not exists
exec dbo.TempGetStateItemExclusive3 @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=NULL output,@locked=NULL output,@lockAge=NULL output,@lockCookie=NULL output,@actionFlags=NULL output
exec dbo.TempInsertUninitializedItem @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=0x140000000000FF,@timeout=20
exec dbo.TempGetStateItemExclusive3 @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=0x140000000000FF output,@locked=0 output,@lockAge=0 output,@lockCookie=2 output,@actionFlags=1 output

exec dbo.TempUpdateStateItemShort @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=0x1400000001000,@timeout=20,@lockCookie=2

-- If exists
exec dbo.TempGetStateItemExclusive3 @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=0x1400000001000 output,@locked=0 output,@lockAge=0 output,@lockCookie=3 output,@actionFlags=0 output

exec dbo.TempUpdateStateItemShort @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=0x1400000001000,@timeout=20,@lockCookie=3

-- Abandon Session
exec dbo.TempGetStateItemExclusive3 @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@itemShort=0x14000000010003 output,@locked=0 output,@lockAge=0 output,@lockCookie=4 output,@actionFlags=0 output
exec dbo.TempRemoveStateItem @id=N'4m4irghotazmxblpyakfaw1d2014c0f1',@lockCookie=4