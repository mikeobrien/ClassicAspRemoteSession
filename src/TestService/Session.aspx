<%@ Page language="C#" %>

<%
    Response.ContentType = "application/json";
    
    if (Request.QueryString["command"] == "abandon") Session.Abandon();

    if (Request.QueryString["command"] == "edit" || 
        Request.QueryString["command"] == "remove") Session.Remove(Request.QueryString["key"]);

    if (Request.QueryString["command"] == "edit" || 
        Request.QueryString["command"] == "add") 
        Session[Request.QueryString["key"]] = Convert.ChangeType(Request.QueryString["value"], Type.GetType("System." + Request.QueryString["datatype"]));
    
    Response.Write("[");

    for (var x = 0; x < Session.Count; x++)
    {
        if (x > 0) Response.Write(",");
        Response.Write(string.Format("{{\"Key\": \"{0}\", \"Value\": \"{1}\", \"DataType\": \"{2}\"}}", Session.Keys[x], Session[x], Session[x].GetType().Name));
    }

    Response.Write("]");
%>