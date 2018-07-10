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
    public struct Hláška
    {
        public Hláška(string hláška, string trigger)
        {
            Ahláška = hláška;
            Atrigger = trigger;
        }

        public string Ahláška { get; private set; }
        public string Atrigger { get; private set; }
    }

    class Program
    {
        public static DiscordClient discord;

        static VoiceNextExtension voice;
        public static ConfigJSON configuration = new ConfigJSON();
        public static string version = "Arpiho1.1.3";
        public static DiscordMember owner;
        public static List<Hláška> arpášoveHlášky = new List<Hláška>() { };

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
            arpášoveHlášky.Add(new Hláška("**Kupujte len kvalitné LED žiarovky PHILIPS®™!!!**", "žiarovka"));
            arpášoveHlášky.Add(new Hláška("**No poď mój k tabuli!!!**", "arpi"));
            arpášoveHlášky.Add(new Hláška("**Napíš tam hore na tabuľu päť!!!**", "tda"));
            arpášoveHlášky.Add(new Hláška("**TDA je najhorší zosilňovač ktorý vymyslela firma PHILIPS®™!!!**", "najlepší"));
            arpášoveHlášky.Add(new Hláška("**Hnusný Kapitalizmus chlapci!!!**", "dobrý kapitalizmus"));
            arpášoveHlášky.Add(new Hláška("**Hodina začne, keď ja poviem že je koniec!**", "končí hodina"));
            arpášoveHlášky.Add(new Hláška("**Predavač nevie čo chceš keď ti rozumie.**", "predavač"));
            arpášoveHlášky.Add(new Hláška("**Alkalické batérie majú nižšiu kapacitu než tie alkalické.**", "baterky"));
            arpášoveHlášky.Add(new Hláška("**Vyvolal si sa sám.**", "ja nejdem"));
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


            discord.ClientErrored += async e =>
            {
                await owner.SendMessageAsync($"```{e.Exception.Message}```");
            };

            discord.MessageCreated += async e =>
            {
                if (!e.Author.IsBot)
                {
                    Random rnd = new Random();
                    if (e.Message.Content.StartsWith("arpi_invite"))
                    {
                        await discord.SendMessageAsync(e.Channel, "***No poď mój k odkazu:*** " + "https://discordapp.com/api/oauth2/authorize?client_id=" + discord.CurrentApplication.Id + "&scope=bot");
                    }
                    else if (!e.Message.Content.StartsWith("."))
                    {

                        for (int i = 1; i < arpášoveHlášky.Count; i++)
                        {
                            if (e.Message.Content.ToLower().Contains(arpášoveHlášky[i].Atrigger))
                            {
                                await discord.SendMessageAsync(e.Channel, arpášoveHlášky[i].Ahláška);
                                return;
                            }
                        }


                        await discord.SendMessageAsync(e.Channel, arpášoveHlášky[rnd.Next(arpášoveHlášky.Count)].Ahláška);
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
