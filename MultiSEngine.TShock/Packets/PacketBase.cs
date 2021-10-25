using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace MultiSEngine.TShock.Packets
{
    public abstract class PacketBase
    {
        public virtual bool PreRecieve(TSPlayer plr)
        {
            return false;
        }
        public virtual void PostRecieve(TSPlayer plr)
        {

        }
        public abstract string PacketName { get; }
        public abstract void InternalWrite(BinaryWriter writer);
        public abstract void InternalRead(BinaryReader reader);
    }
}
