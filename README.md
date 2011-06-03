Classic ASP Remote Session
=============

This component provides a bridge between classic asp in-process session state and ASP.NET SQL session state.

Usage
---------------------

Creating the remote session object:

    Set remoteSession = CreateObject("UltravioletCatastrophe.RemoteSqlSessionState")

Loading session state from sql server:

    remoteSession.Load Request, Response, Session
    
Saving session state to the sql server:

    remoteSession.Save Request, Response, Session
    
Abandoning a session:

    remoteSession.Abandon Request, Response, Session
    
Limitations
---------------------

The bridge will only work with the folllowing basic types. Session values of other types will be discarded.

**CLR**|**VBScript**
--------|--------
Boolean|Boolean
Byte|Byte
DateTime|Date
Double|Double
Int16|Integer
Int32|Long
Float|Single
String|String

