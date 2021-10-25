using Rests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSEngine.TShock
{
    public static class Utils
    {
        public static bool TryGetParam(this RestRequestArgs args, string param, out string result)
        {
            result = args.Parameters[param];
            if(string.IsNullOrEmpty(result))
                return false;
            return true;
        }
        public static byte[] ToBytes(this Packets.PacketBase p) => PacketManager.Write(p);
    }
}
