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