using MultiSEngine.TShock.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TShockAPI;

namespace MultiSEngine.TShock
{
    public class PacketManager
    {
        private static readonly Dictionary<string, Type> Packets = new();
        internal static void Init()
        {
            RegisterPackets<SyncIP>();
            RegisterPackets<ExcuteMSECommand>();
        }
        public static void RegisterPackets<T>() where T : PacketBase
        {
            var name = (Activator.CreateInstance(typeof(T)) as PacketBase).PacketName;
            if (Packets.ContainsKey(name))
            {
                Console.WriteLine($"[warn] packet: {name} already exist.");
                return;
            }
            Packets.Add(name, typeof(T));
        }
        public static PacketBase Read(BinaryReader reader, TSPlayer plr)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (plr is null)
                throw new ArgumentNullException(nameof(plr));
            var id = reader.ReadString();
            var token = reader.ReadString();
            if (Config.Instance.Token != token)
            {
                TShockAPI.TShock.Log.ConsoleInfo($"[MultiSEngine] Recieve invalid token: {token}.");
                return null;
            }
            if (Packets.FirstOrDefault(p => p.Key == id) is { } p)
            {
                var packet = Activator.CreateInstance(p.Value) as Packets.PacketBase;
                if (!packet.PreRecieve(plr))
                {
                    packet.InternalRead(reader);
                    packet.PostRecieve(plr);
                }
                return packet;
            }
            else
                TShockAPI.TShock.Log.ConsoleInfo($"[MultiSEngine] packet: {id} not defined, ignore.");
            return null;
        }
        public static byte[] Write(PacketBase packet)
        {
            if (packet is null)
                throw new ArgumentNullException(nameof(packet));
            var data = new RawDataBuilder(15).PackString(packet.PacketName).PackString(Config.Instance.Token);
            packet.InternalWrite(data.writer);
            return data.GetByteData();
        }
    }
}
