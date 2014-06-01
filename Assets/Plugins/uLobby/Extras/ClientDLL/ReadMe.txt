This folder contains a client-side version of the uLobby DLL, which only includes the client-side interface of uLobby (that is, the interface used by game clients and game servers) but not the lobby-side interface. It is thus smaller than the standard DLL which contains both interfaces, and also does not depend on uGameDB being present in the project. You can use the client DLL if you want to save space when building a game client or game server.

To use the client DLL, you need to:
* Remove the standard uLobby DLL (Plugins/uLobby/Assembly/uLobby.dll) or rename it to something not ending in ".dll", so that it is not included in the built Unity project.
* Rename the client DLL (Plugins/uLobby/Extras/ClientDLL/uLobby) to "uLobby.dll".
* Optionally remove the Jboy and Plugins/uGameDB folders.
