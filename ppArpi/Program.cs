using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Net;
using System.IO;
using System.Net.Http;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Xml;
using System.Collections.Generic;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using System.Linq;

namespace Morbot
{
    class Program
    {
        public static DiscordClient discord;

        static VoiceNextExtension voice;
        public static ConfigJSON configuration = new ConfigJSON();
        public static string version = "1.1.3";
        public static DiscordMember owner;
        public static List<String> arpášoveHlášky = new List<string>() { };

        public class ConfigJSON
        {
            public string DiscordBotToken { get; set; }
        }

        static void Main(string[] args)
        {
            initArpi();
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            discord.DisconnectAsync();


        }

        private static void initArpi()
        {
            arpášoveHlášky.Add("**Kupujte len kvalitné LED žiarovky PHILIPS®™!!!**");
            arpášoveHlášky.Add("**No poď mój k tabuli!!!**");
            arpášoveHlášky.Add("**Napíš tam hore na tabuľu päť!!!**");
            arpášoveHlášky.Add("**TDA je najhorší zosilňovač ktorý vymyslela firma PHILIPS®™!!!**");
            arpášoveHlášky.Add("**Hnusný Kapitalizmus chlapci!!!**");
            arpášoveHlášky.Add("**Hodina začne, keď ja poviem že je koniec!**");
            arpášoveHlášky.Add("**Predavač nevie čo chceš keď ti rozumie.**");
            arpášoveHlášky.Add("**Alkalické batérie majú nižšiu kapacitu než tie alkalické.**");
            arpášoveHlášky.Add("**Vyvolal si sa sám.**");
            //arpášoveHlášky.Add("****");
            //arpášoveHlášky.Add("****");
            //arpášoveHlášky.Add("****");
            //arpášoveHlášky.Add("****");
            ///arpášoveHlášky.Add("****");
        }

        static async Task MainAsync(string[] args)
        {


            if (!File.Exists("config.json"))
            {
                File.Create("config.json");
            }
            else
            {
                var json = "";
                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = await sr.ReadToEndAsync();
                configuration = JsonConvert.DeserializeObject<ConfigJSON>(json);
            }

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = configuration.DiscordBotToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            });

            voice = discord.UseVoiceNext(new VoiceNextConfiguration
            {
                VoiceApplication = DSharpPlus.VoiceNext.Codec.VoiceApplication.LowLatency
            });

            discord.ClientErrored += async e =>
            {
                await owner.SendMessageAsync($"```{e.Exception.Message}```");
            };

            discord.MessageCreated += async e =>
            {
                if (!e.Author.IsBot)
                {
                    if (e.Message.Content.StartsWith(".")) { }
                    else
                    {
                        Random rnd = new Random();
                        await discord.SendMessageAsync(e.Channel, arpášoveHlášky[rnd.Next(arpášoveHlášky.Count)]);
                    }
                }
            };
            discord.Ready += async e =>
                {
                    DiscordActivity game = new DiscordActivity
                    {
                        Name = "No poď mój k tabuli",
                        ActivityType = ActivityType.Playing
                    };
                    await discord.UpdateStatusAsync(game);

                };
            await discord.ConnectAsync();
            discord.GuildAvailable += async e =>
                    {

                        await e.Guild.GetDefaultChannel().SendMessageAsync("**Zdravím vás, sadnite si.**");
                        foreach (DiscordMember member in e.Guild.Members)
                        {
                            if (member.Id == e.Client.CurrentApplication.Owner.Id)
                            {
                                owner = member;
                                await owner.SendMessageAsync("Arpi beží! ©Madagaskar");
                            }
                        }

                    };
            await Task.Delay(-1);

        }
    }
}