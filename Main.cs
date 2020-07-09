using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using XLMultiplayerServer;

namespace XLSAdmin {
    public class Main {
        private static Plugin pluginInfo;

        [JsonProperty("Admin")]
        private static List<string> AdminArray;

        private static List<string> ShitList;

        public static List<string> BanList = new List<string>();

        public static Dictionary<string, string> MapChangeList { get; private set; } = new Dictionary<string, string>();

        private static string NextMapHash = null;

        public static void Load(Plugin info) {
            pluginInfo = info;
            pluginInfo.OnToggle = OnToggle;
            pluginInfo.OnConnect = OnConnection;
            pluginInfo.ReceiveUsername = OnUsername;
            pluginInfo.PlayerCommand = PlayerCommand;
        }

        private static void OnToggle(bool enabled) {
            if (enabled) {
                if (File.Exists(Path.Combine(pluginInfo.path, "Config.json"))) {
                    JsonConvert.DeserializeObject<Main>(File.ReadAllText(Path.Combine(pluginInfo.path, "Config.json")));
                    CreateMapList();
                } else {
                    pluginInfo.LogMessage("[XLSAdmin Error] Could not find server config file", ConsoleColor.Red);
                    Console.In.Read();
                    return;
                }
            }
        }

        private static bool OnConnection(string ip) {
            if (pluginInfo.playerList.Count == (pluginInfo.maxPlayers - 1)) {
                pluginInfo.LogMessage("Server almost full... Checking player IP for reserved slot. Online: " + pluginInfo.playerList.Count, ConsoleColor.DarkRed);
                if (ip != "127.0.0.1" && !AdminArray.Contains(ip)) {
                    pluginInfo.LogMessage("No slots reserved for " + ip + ", closing connection....", ConsoleColor.Red);
                    return false;
                } else {
                    pluginInfo.LogMessage("Found slot reserved for " + ip + ", proceeding with connection.", ConsoleColor.DarkGreen);
                }
            }

            if (File.Exists(Path.Combine("./", "ipban.txt"))) {
                string ipBanList = File.ReadAllText(Path.Combine("./", "ipban.txt"));
                string[] readBannedIPs = ipBanList.Split('\n');
                BanList.AddRange(readBannedIPs);
                if (BanList.Contains(ip)) {
                    pluginInfo.LogMessage("Ban player attempted to connect to the server, IP: " + ip, ConsoleColor.DarkRed);
                    return false;
                }
            }

            return true;
        }

        private static void OnUsername(PluginPlayer player, string name) {
            if (AdminArray.Contains(player.GetIPAddress())) {
                pluginInfo.SendServerAnnouncement("Aye, it's " + name + "!", 10, "0f0");
            }
        }

        private static bool PlayerCommand(string message, PluginPlayer sender) {
            if (AdminArray.Contains(sender.GetIPAddress().ToString())) {
                if (message.ToLower().StartsWith("/prefix ")) {
                    string prefix = message.Remove(0, "/prefix ".Length);
                    pluginInfo.SendServerAnnouncement("Prefix = " + prefix, 10, "f0f");
                    return true;
                } else if (message.ToLower().StartsWith("/rlm") || message.ToLower().StartsWith("/reloadmaps") || message.ToLower().StartsWith("/reload maps")) {
                    pluginInfo.ReloadMapList();
                    pluginInfo.SendServerAnnouncement(sender.username + "[" + sender.playerID + "]" + " has reloaded the map list.", 10, "0f0");
                    CreateMapList();
                    return true;
                } else if (message.ToLower().StartsWith("/ban ")) {
                    string IDinput = message.Remove(0, "/ban ".Length);
                    PluginPlayer NewBanID = pluginInfo.playerList.Find(p => p.playerID.ToString().Contains(IDinput));
                    if (NewBanID != null) {
                        BanMethod(NewBanID, sender);
                    } else {
                        pluginInfo.SendImportantMessageToPlayer("No match for playerID: " + IDinput, 10, "f00", sender.GetPlayer());
                    }
                    return true;
                } else if (message.ToLower().StartsWith("/kick ")) {
                    string KickInput = message.Remove(0, "/kick ".Length);
                    PluginPlayer KickID = pluginInfo.playerList.Find(p => p.playerID.ToString().Contains(KickInput));
                    if (KickID != null) {
                        KickMethod(KickID, sender);
                    } else {
                        pluginInfo.SendImportantMessageToPlayer("No match for playerID: " + KickInput, 10, "f00", sender.GetPlayer());
                    }
                    return true;
                } else if (message.ToLower().StartsWith("/maplist") || message.ToLower().StartsWith("/ml") || message.ToLower().StartsWith("/maps")) {
                    ListMaps(sender);
                    return true;
                } else if (message.ToLower().StartsWith("/changemap ")) {
                    string mapKeyInput = message.Remove(0, "/changemap ".Length);
                    if (MapChangeList.ContainsKey(mapKeyInput)) {
                        NextMapHash = MapChangeList[mapKeyInput];
                        pluginInfo.SendServerAnnouncement("Changing map to: " + pluginInfo.mapList[NextMapHash] + ".", 10, "fff");
                        pluginInfo.ChangeMap(NextMapHash);
                    } else {
                        pluginInfo.SendImportantMessageToPlayer("Sorry, no match for Map ID: " + mapKeyInput + ".", 10, "f00", sender.GetPlayer());
                    }
                    return true;
                }
            } else {
                pluginInfo.SendImportantMessageToPlayer("Sorry, but you don't have Administrative Priveleges" + sender.username + "[" + sender.playerID + "].", 10, "f00", sender.GetPlayer());
                return false;
            }
            return false;
        }

        private static void BanMethod(PluginPlayer player, PluginPlayer admin) {
            // Figure this one out... Is it possible even?
            pluginInfo.SendImportantMessageToPlayer("You have been banned.", 10, "f00", player.GetPlayer());
            pluginInfo.SendServerAnnouncement(admin.username + "[" + admin.playerID + "]" + " has banned: " + player.username + "[" + player.playerID + "]!", 10, "fff");
            pluginInfo.LogMessage("Administrator: " + admin.username + ", has used the ban command on: " + player.username + "[" + player.GetIPAddress() + "]", ConsoleColor.White);
            BanList.Add(player.GetIPAddress());
            pluginInfo.DisconnectPlayer(player.GetPlayer());
            string ipBanString = "";
            foreach (string ip in BanList) {
                ipBanString += ip + "\n";
            }
            File.WriteAllText(Path.Combine("./", "ipban.txt"), ipBanString);
        }

        private static void KickMethod(PluginPlayer player, PluginPlayer admin) {
            pluginInfo.SendImportantMessageToPlayer("You have been kicked.", 10, "f00", player.GetPlayer());
            pluginInfo.SendServerAnnouncement(admin.username + "[" + admin.playerID + "]" + " has kicked: " + player.username + "[" + player.playerID + "]!", 10, "fff");
            pluginInfo.LogMessage("Administrator: " + admin.username + ", has used the ban command on: " + player.username + "[" + player.GetIPAddress() + "]", ConsoleColor.White);
            pluginInfo.DisconnectPlayer(player.GetPlayer());
        }


        private static async void ListMaps(PluginPlayer CmdUser) {
            CreateMapList();
            int Index = 0;
            string CurrentMapHash = pluginInfo.currentMap;
            foreach (var map in pluginInfo.mapList) {
                string mapHash = map.Key;
                string mapName = map.Value;
                await Task.Run(() => pluginInfo.SendImportantMessageToPlayer("Key: " + (Index++) + " - Map: " + mapName, 10, "fff", CmdUser.GetPlayer()));
            }
        }

        private static async void CreateMapList() {
            MapChangeList.Clear();
            int Index = 0;
            foreach (var map in pluginInfo.mapList) {
                int NewMapHash = Index++;
                string OGMapHash = map.Key;
                await Task.Run(() => MapChangeList.Add(NewMapHash.ToString(), OGMapHash));
            }
        }
    }
}
