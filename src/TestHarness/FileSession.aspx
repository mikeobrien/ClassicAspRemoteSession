<%@ Page language="C#" %>
<%@ Import namespace="RemoteSession" %>
<%@ Import namespace="System.IO" %>

<html>
<body style="font-family:Arial;font-size:10pt">

<h3><a href="FileSession.aspx">Session File Editor</a></h3>

<%
    if (Request.QueryString["createsession"] == "true") Session["test"] = "test";
%>

<p>Session Id: <%= Request.Cookies["ASP.NET_SessionId"].Value%> (<a href="filesession.aspx?createsession=true">Create Session</a>)</p>

<%

var context = new Context(Request.ServerVariables["APPL_MD_PATH"], Request.Cookies["ASP.NET_SessionId"].Value);
var provider = new FileSessionProvider();

if (Request.QueryString["create"] == "true") provider.Save(context, new Dictionary<string,object>());
if (Request.QueryString["delete"] == "true") provider.Abandon(context);

if (!File.Exists(provider.GetSessionFilePath(context))) {
    %> Session file not found. (<a href="filesession.aspx?create=true">Create File</a>) <%
} else {

    var values = provider.Load(context);

    if (Request.QueryString["remove"] == "true")
    {
        values.Remove(Request.QueryString["key"]);
        provider.Save(context, values);
        values = provider.Load(context);
    }

    if (Request.QueryString["edit"] == "true")
    {
        if (Request.QueryString["id"] != "*") values.Remove(Request.QueryString["id"]);
        object value = null;
        switch (Request.QueryString["type"])
        {
            case "String": value = Request.QueryString["value"]; break;
            case "Boolean": value = bool.Parse(Request.QueryString["value"]); break;
            case "Byte": value = byte.Parse(Request.QueryString["value"]); break;
            case "Int16": value = short.Parse(Request.QueryString["value"]); break;
            case "Int32": value = int.Parse(Request.QueryString["value"]); break;
            case "Double": value = Double.Parse(Request.QueryString["value"]); break;
            case "Float": value = float.Parse(Request.QueryString["value"]); break;
            case "DateTime": value = DateTime.Parse(Request.QueryString["value"]); break;
        }
        values.Add(Request.QueryString["key"], value);
        provider.Save(context, values);
        values = provider.Load(context);
    }

    %>

    Session file exists. (<a href="filesession.aspx?delete=true">Delete</a>)<br /><br />

    <form action="filesession.aspx" method=get>
        <select name="id">
          <option value="*">New</option>
          <% foreach (var value in values) { %>
          <option value="<%=value.Key %>">Edit <%=value.Key %></option>
          <% } %>
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

    <table border ="1">
        <tr><th>Key</th><th>Value</th><th>Type</th><th>Action</th></tr>
        <%

        foreach (var value in values) { %>
            <tr><td><%=value.Key %></td><td><%=value.Value %></td><td><%=value.Value != null ? value.Value.GetType().Name : "null" %></td>
            <td><a href="filesession.aspx?remove=true&key=<%=value.Key %>">Delete</a></td></tr>
        <% } %>
    </table>
<% } %>

</body>
</html>