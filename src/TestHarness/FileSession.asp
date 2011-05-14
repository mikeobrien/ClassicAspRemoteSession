<html>
<body style="font-family:Arial;font-size:10pt">

<h3><a href="FileSession.asp">Classic Asp Session</a></h3>

<p>Session Id: <%= Request.Cookies("ASP.NET_SessionId")%> (<a href="FileSession.asp?abandon=true">Abandon</a>)</p>

<%
Set RemoteSession = CreateObject("UltravioletCatastrophe.RemoteFileSession")

If Request.QueryString("abandon") = "true" Then
    RemoteSession.Abandon request, response, session
End If

RemoteSession.Load request, response, session

If Request.QueryString("remove") = "true" Then
    Session.Contents.Remove(Request.QueryString("key"))
    RemoteSession.Save request, response, session
End If

If Request.QueryString("edit") = "true" Then
    If Request.QueryString("id") <> "*" Then
        Session.Contents.Remove(Request.QueryString("id"))
    End If
    Dim typeId
    Dim textValue
    Dim value
    typeId = Request.QueryString("type")
    textValue = Request.QueryString("value")
    If typeId = "Boolean" Then 
        value = CBool(textValue) 
    End If
    If typeId = "Byte" Then 
        value = CByte(textValue) 
    End If
    If typeId = "Datetime" Then 
        value = CDate(textValue) 
    End If
    If typeId = "Double" Then 
        value = CDbl(textValue) 
    End If
    If typeId = "Int16" Then 
        value = CInt(textValue) 
    End If
    If typeId = "Int32" Then 
        value = CLng(textValue) 
    End If
    If typeId = "Float" Then 
        value = CSng(textValue) 
    End If
    If typeId = "String" Then 
        value = CStr(textValue) 
    End If
    Session(Request.QueryString("key")) = value
    RemoteSession.Save request, response, session
End If
%>

    <form action="filesession.asp" method=get>
        <select name="id">
          <option value="*">New</option>
          <% For Each Item in Session.Contents %>
          <option value="<%=Item %>">Edit <%=Item %></option>
          <% Next %>
        </select>: 
        Key <input name="key" type="text" /> 
        Value <input name="value" type="text"/> 
        Type <select name="type">
               <option value="String">String</option>
               <option value="Boolean">Boolean</option>
               <option value="Byte">Byte</option>
               <option value="Int16">Int16</option>
               <option value="Int32">Int32</option>
               <option value="Double">Double</option>
               <option value="Float">Float</option>
               <option value="Datetime">DateTime</option>
             </select> 
        <input type="submit" value="OK" /> 
        <input type="hidden" name="edit" value="true" />
    </form>

<table border="1">
<tr><th>Key</th><th>Value</th><th>Type</th><th>Action</th></tr>
<%
For Each Item in Session.Contents
%>
<tr><td><%=Item %></td><td><%=Session.Contents(Item) %></td><td><%=TypeName(Session.Contents(Item)) %></td>
<td><a href="filesession.asp?remove=true&key=<%=Item %>">Delete</a></td></tr>
<% Next %>
</table>

</body>
</html>