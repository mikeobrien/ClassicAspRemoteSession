<%

Set RemoteSession = CreateObject("UltravioletCatastrophe.RemoteSqlSessionState")

If Request.QueryString("command") = "version" Then
    Response.Write(RemoteSession.GetVersion)
    Response.End
End If

RemoteSession.Load Request, Response, Session

Response.ContentType = "application/json"

Function GetTypedValue(value, dataType)
    If LCase(dataType) = "boolean" Then 
        GetTypedValue = CBool(value) 
    End If
    If LCase(dataType) = "byte" Then 
        GetTypedValue = CByte(value) 
    End If
    If LCase(dataType) = "datetime" Then 
        GetTypedValue = CDate(value) 
    End If
    If LCase(dataType) = "double" Then 
        GetTypedValue = CDbl(value) 
    End If
    If LCase(dataType) = "int16" Then 
        GetTypedValue = CInt(value) 
    End If
    If LCase(dataType) = "int32" Then 
        GetTypedValue = CLng(value) 
    End If
    If LCase(dataType) = "float" Then 
        GetTypedValue = CSng(value) 
    End If
    If LCase(dataType) = "string" Then 
        GetTypedValue = CStr(value) 
    End If
End Function

Function GetValueType(value)
    If TypeName(value) = "Boolean" Then 
        GetValueType = "Boolean" 
    End If
    If TypeName(value) = "Byte" Then 
        GetValueType = "Byte" 
    End If
    If TypeName(value) = "Date" Then 
        GetValueType = "DateTime" 
    End If
    If TypeName(value) = "Double" Then 
        GetValueType = "Double" 
    End If
    If TypeName(value) = "Integer" Then 
        GetValueType = "Int16" 
    End If
    If TypeName(value) = "Long" Then 
        GetValueType = "Int32"
    End If
    If TypeName(value) = "Single" Then 
        GetValueType = "Float" 
    End If
    If TypeName(value) = "String" Then 
        GetValueType = "String" 
    End If
End Function

If Request.QueryString("command") = "abandon" Then
    RemoteSession.Abandon Request, Response, Session
End If

If Request.QueryString("command") = "remove" Then
    Session.Contents.Remove(Request.QueryString("key"))
    RemoteSession.Save Request, Response, Session
End If

If Request.QueryString("command") = "edit" Or Request.QueryString("command") = "add" Then
    Session.Contents.Remove(Request.QueryString("key"))
    Session(Request.QueryString("key")) = GetTypedValue(Request.QueryString("value"), Request.QueryString("datatype"))
    RemoteSession.Save Request, Response, Session
End If

Response.Write("[")

Dim firstItem
firstItem = true

For Each Item in Session.Contents
    If firstItem = false Then Response.Write(",")
    Response.Write("{""Key"": """ + Item + """, ""Value"": """ + CStr(Session(Item)) + """, ""DataType"": """ + GetValueType(Session(Item))  + """}")
    firstItem = false
Next

Response.Write("]")

%>