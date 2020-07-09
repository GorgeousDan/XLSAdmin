# XLS Admin Plugin
Enables multiplayer server hosts to use administrative commands from chat, ridding the need to use your command prompt.


### Prerequisites
Silentbaws' XLMultiplayerServer v0.9.2. - [silentbaws/XLMultiplayer](https://github.com/silentbaws/XLMultiplayer/releases/tag/0.9.2)

## Installing
```
1. Download and Unzip XLSAdmin.zip
2. Drop unzipped folder in server's `/Plugins/` directory.
```

## Settings
In the unzipped folder you will find a file named `Config.json`, in this file you will see the configuration settings below.
#### Admin:
This is a list of IPs allowed to use admin commands. If hosting on a VPS, or other machine besides your own; you will need to add your IP to this list.
### Add your IP:
```
1. Unless you know your IP by memory, go-to https://canyouseeme.org and copy your IP.

2. Select the second entry in Config.json (should say "0.0.0.0").

2. Replace with your IP (leave the quotes "").

4. Done! Save the file.
```

### Add more IP's:
```
1. Add a comma after the last IP entry and then put the new IP wrapped in quotes.

Essentially it should look like this:
Admin: [
"127.0.0.1",
"12.34.56.78",
"other.player.ip.address"
]
```

## If hosting from the same machine you're playing on, you should ***NOT*** have to add your own IP.

# Commands:
Commands are simple, just type `/` followed by the command to be used.
```
Example: /kick 2
This will kick the player assigned to this ID#.
(You can see player's ID#s in the F2 menu.)
```
## Command List:

### /kick
```
Use: /kick {playerID}

Kick the player associated with the given ID# from the server.
```

### /ban
```
Use: /ban {playerID}

Add player's IP to ban list and removes player from server.
```

### /reloadmaps, /rlm
```
Use: /rlm

Reload the current map list.
(Useful when adding new maps to the server without needing to restart.)
```

### /maplist, /ml, /maps
```
Use: /ml

List all the currently loaded maps, along with their ID. 
(You can use this ID to force change the map with the /changemap command.)
```

### /changemap
```
Use: /changemap

Change the current map. 
(Get a list IDs from using the /maplist command.)
```
# Note: THIS PLUGIN WILL CREATE A "RESERVED SLOT" IN THE SERVER...

#### This means 1 spot will remain open until a player from the admin list joins.
```
Example: With MaxPlayer set to `20`, when the server is at `19/20 players`;
anyone who tries to join will not be able to connect unless they're on the list.
Will try to update this with better functionality.
```




## Acknowledgments

* Shouts out to [silentbaws](https://github.com/silentbaws) and [m4cs](https://github.com/M4cs).
