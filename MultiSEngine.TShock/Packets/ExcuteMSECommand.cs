using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSEngine.TShock.Packets
{
    internal class ExcuteMSECommand : PacketBase
    {
        public override string PacketName => "MultiSEngine.ExcuteMSECommand";
        public string Command { get; set; }
        public string PlayerName { get; set; }
        public override void InternalRead(BinaryReader reader)
        {
            PlayerName = reader.ReadString();
            Command = reader.ReadString();
        }

        public override void InternalWrite(BinaryWriter writer)
        {
            writer.Write(PlayerName);
            writer.Write(Command);
        }
    }
}
