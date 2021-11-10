using System.Collections.Generic;
using OTAPI;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Net.Sockets;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace MultiSEngine.TShock
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public Plugin(Main game) : base(game)
        {
        }
        public override string Name => "MultiSEngine.TShock";
        public override string Description => "MultiSEngine 的TShock兼容插件";
        public override string Author => "Megghy";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public override void Initialize()
        {
            PacketManager.Init();
            OldGetDataHandler = Hooks.Net.ReceiveData;
            Hooks.Net.ReceiveData = OnReceiveData;
            OldSendDataHandler = Hooks.Net.SendBytes;
            Hooks.Net.SendBytes = OnSendData;
            GeneralHooks.ReloadEvent += _ => Config._instance = null;
            PlayerHooks.PlayerCommand += OnExcuteCommand;
        }

        internal Hooks.Net.ReceiveDataHandler OldGetDataHandler;
        internal Hooks.Net.SendBytesHandler OldSendDataHandler;
        private void OnExcuteCommand(PlayerCommandEventArgs args)
        {
            if (args.CommandName.ToLower() == "mse")
            {
                args.Handled = true;
                args.Player.SendRawData(new Packets.ExcuteMSECommand()
                {
                    PlayerName = args.Player.Name,
                    Command = args.CommandText
                }.ToBytes());
            }
        }
        public HookResult OnSendData(ref int remoteClient, ref byte[] data, ref int offset, ref int size, ref SocketSendCallback callback, ref object state)
        {
            if (TShockAPI.TShock.VersionNum < new Version(4, 5, 0) || data[offset + 2] != 20)
                return OldSendDataHandler.Invoke(ref remoteClient, ref data, ref offset, ref size, ref callback, ref state);
            using (var reader = new BinaryReader(new MemoryStream(data, offset + 3, size - 3)))
            {
                short tileX = reader.ReadInt16();
                short tileY = reader.ReadInt16();
                ushort width = reader.ReadByte();
                ushort length = reader.ReadByte();
                byte changeType = reader.ReadByte();
                ushort header = Math.Max(width, length);
                if (width != length)
                {
                    TShockAPI.TShock.Log.Info($"[MultiSEngine] modified tile square <{tileX}, {tileY}: {header}> for [{TShockAPI.TShock.Players[remoteClient]?.Name}].");
                    TShockAPI.TShock.Players[remoteClient].SendTileSquare(tileX, tileY, (byte)header);
                    return HookResult.Cancel;
                }
                else
                    return HookResult.Continue;
            }
        }
        public HookResult OnReceiveData(MessageBuffer buffer, ref byte packetid, ref int readoffset, ref int start, ref int length)
        {
            if (packetid == 15)
            {
                try
                {
                    using (var reader = new BinaryReader(new MemoryStream(buffer.readBuffer, readoffset, length)))
                    {
                        PacketManager.Read(reader, TShockAPI.TShock.Players[buffer.whoAmI]);
                    }
                }
                catch (Exception ex)
                {
                    TShockAPI.TShock.Log.ConsoleError(ex.ToString());
                }
                return HookResult.Cancel;
            }
            else
                return OldGetDataHandler(buffer, ref packetid, ref readoffset, ref start, ref length);
        }
    }
}
