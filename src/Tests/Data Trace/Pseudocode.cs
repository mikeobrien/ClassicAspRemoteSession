/* Stored Procedures 

TempGetAppID 
    @appName varchar(280), 
    @appId   int OUTPUT
    
TempGetStateItemExclusive3
    @id          nvarchar(88),
    @itemShort   varbinary(7000) OUTPUT,
    @locked      bit OUTPUT,
    @lockAge     int OUTPUT,
    @lockCookie  int OUTPUT,
    @actionFlags int OUTPUT
    SELECT itemLong
            
TempReleaseStateItemExclusive
    @id         nvarchar(88),
    @lockCookie int
    
TempInsertStateItemShort
    @id         tSessionId,
    @itemShort  tSessionItemShort,
    @timeout    int

TempInsertStateItemLong
    @id         tSessionId,
    @itemLong   tSessionItemLong,
    @timeout    int
            
TempUpdateStateItemShortNullLong
    @id         nvarchar(88),
    @itemShort  varbinary(7000),
    @timeout    int,
    @lockCookie int
            
TempUpdateStateItemLongNullShort
    @id         nvarchar(88),
    @itemLong   image,
    @timeout    int,
    @lockCookie int
    
TempRemoveStateItem
    @id         nvarchar(88),
    @lockCookie int

*/

private const DefaultSessionData = 0x140000000000FF;
private const DefaultTimeout = 20;

public string GetAppId()
{
    return TempGetAppID(appName);
}

public SessionData ReadSession(string id)
{
    var data = GetSessionData(id);
    if (!data.Empty) TempReleaseStateItemExclusive(id, data.LockCookie);
    return data;
} 

public void WriteSession(id, itemData)
{
    var data = GetSessionData(id);
    if (data.Expired) return;
    if (data.Empty)
    {
        if (itemData.Length <= 7000)
            TempInsertStateItemShort(id, itemData, data.Timeout);
        else
            TempInsertStateItemLong(id, itemData, data.Timeout);
    }
    else
    {
        if (itemData.Length <= 7000)
            TempUpdateStateItemShortNullLong(id, itemData, data.Timeout, data.LockCookie);
        else
            TempUpdateStateItemLongNullShort(id, itemData, data.Timeout, data.LockCookie);
    }
}

public void AbandonSession(string id)
{
    var data = TempGetStateItemExclusive3(id);
    if (!data.Empty) AbandonSession(id, data);
}

private SessionData GetSessionData(string id)
{
    var data = TempGetStateItemExclusive3(id);
    if (!data.Empty && data.Expired) AbandonSession(id, data);
    return data;
}

private void AbandonSession(string id, SessionData data)
{
    TempRemoveStateItem(id, data.LockCookie);
}
