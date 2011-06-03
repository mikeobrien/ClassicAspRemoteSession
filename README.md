Classic ASP Remote Session
=============

This component provides a bridge between classic asp session state and ASP.NET SQL session state.

Usage
---------------------

Creating the remote session object:

    Set RemoteSession = CreateObject("UltravioletCatastrophe.RemoteSqlSessionState")

Loading session state from sql server:

    RemoteSession.Load Request, Response, Session
    
Saving session state to the sql server:

    RemoteSession.Save Request, Response, Session
    
Abandoning a session:

    RemoteSession.Abandon Request, Response, Session
    
Limitations
---------------------

The bridge will only work with the folllowing basic types. Session values of other types will be discarded.

CLR | VBScript
-------|----------
Boolean|Boolean
Byte|Byte
DateTime|Date
Double|Double
Int16|Integer
Int32|Long
Float|Single
String|String

Boolean
Byte
DateTime (Date in VBScript)
Double
Int16 (Integer in VBScript)
Int32 (Long in VBScript)
Float (Single in VBScript)
String