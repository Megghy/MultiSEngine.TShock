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
using ClientApi.Networking;

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
            Hooks.MessageBuffer.GetData += OnGetData;
            PlayerHooks.PlayerCommand += OnExcuteCommand;
        }

        private void OnGetData(object? sender, Hooks.MessageBuffer.GetDataEventArgs e)
        {
            if (e.MessageType == 15)
            {
                try
                {
                    using (var reader = new BinaryReader(new MemoryStream(e.Instance.readBuffer, e.ReadOffset, e.Length)))
                    {
                        PacketManager.Read(reader, TShockAPI.TShock.Players[e.Instance.whoAmI]);
                    }
                }
                catch (Exception ex)
                {
                    TShockAPI.TShock.Log.ConsoleError(ex.ToString());
                }
                e.Result = HookResult.Cancel;
            }
        }
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
    }
}
