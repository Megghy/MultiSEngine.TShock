using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using TShockAPI;

namespace MultiSEngine.TShock.Packets
{
    public class SyncIP : PacketBase
    {
        public override string PacketName => "MultiSEngine.SyncIP";
        public string PlayerName { get; set; }
        public string IP { get; set; }
        public override void InternalRead(BinaryReader reader)
        {
            PlayerName = reader.ReadString();
            IP = reader.ReadString();
        }

        public override void InternalWrite(BinaryWriter writer)
        {
            writer.Write(PlayerName);
            writer.Write(IP);
        }
        public override void PostRecieve(TSPlayer plr)
        {
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");
            if (rx.IsMatch(IP))
            {
                typeof(TSPlayer).GetField("CacheIP", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(plr, IP);
                TShockAPI.TShock.Log.ConsoleInfo($"[MultiSEngine] change player {plr.Name}'s IP to {IP}.");
            }
            else
            {
                TShockAPI.TShock.Log.ConsoleInfo($"[MultiSEngine] invalid ip address: {IP}.");
            }
        }
    }
}
