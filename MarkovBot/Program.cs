using System;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Text.Json;
using System.Text;

namespace GenaBot
{
    class Config
    {
        [System.Text.Json.Serialization.JsonInclude]
        public Dictionary<ulong, bool> AutoRead { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string Token { get; private set; }
        public Config(Dictionary<ulong, bool> autoRead)
        {
            AutoRead = autoRead;
            Token = "";
        }
    }
    class Program
    {
        //key: guild ID
        static Dictionary<ulong, bool> AutoRead = new Dictionary<ulong, bool>();
        static Config config;
        static bool Works;
        public static List<List<string>>? GetLearnedWordsFromGuild(DiscordGuild guild)
        {
            if (File.Exists(guild.Id.ToString()))
            {
                return JsonSerializer.Deserialize<List<List<string>>>(File.ReadAllText(guild.Id.ToString()));
            }
            else
            {
                return null;
            }
        }
        public static void LearnWords(DiscordGuild guild, string sentince)
        {
            if (File.Exists(guild.Id.ToString()))
            {
                List<List<string>>? words = JsonSerializer.Deserialize<List<List<string>>>(File.ReadAllText(guild.Id.ToString()))?.ToList();
                if(words != null)
                {
                    words.Add(sentince.Split('\n', ' ').ToList());
                    FileStream fs = File.Open(guild.Id.ToString(), FileMode.Create);
                    fs.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(words.ToArray())));
                    fs.Close();
                }
            }
            else
            {
                FileStream fs = File.Open(guild.Id.ToString(), FileMode.Create);
                fs.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new List<List<string>>() { sentince.Split('\n', ' ', '\t').ToList() })));
                fs.Close();
            }
        }
        public static void HookAutoRead(DiscordGuild guild, bool b)
        {
            if (AutoRead.ContainsKey(guild.Id))
            {
                AutoRead[guild.Id] = b;
            }
        }
        static void Main()
        {
            Console.WriteLine("Markov chain bot host;\nPress any key to close server.\nNote: dont use Ctrl+C.");
            if (!File.Exists("config.json"))
            {
                StreamWriter sw = File.CreateText("config.json");
                sw.Write(JsonSerializer.Serialize(new Config(new Dictionary<ulong, bool>())));
                sw.Close();
                Console.WriteLine("Config not found;\nnew config was created, write your bot token in \"Token\":\"\" json property;\npress any key to exit.");
                Console.ReadKey();
                return;
            }
            else
            {
                try
                {
                    Config? cfg = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
                    if (cfg != null)
                        config = cfg;
                    else
                    {
                        Console.WriteLine("Failed deserealise config (cfg is null).");
                        Console.ReadKey();
                        return;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message + "\npress any key to exit.");
                    Console.ReadKey();
                    return;
                }
            }
            AutoRead = config.AutoRead;
            DiscordClient client;
            try
            {
                client = new DiscordClient(new DiscordConfiguration() { Token = config.Token, TokenType = TokenType.Bot });
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message + "\npress any key to exit.");
                Console.ReadKey();
                return;
            }
            CommandsNextExtension commands = client.UseCommandsNext(new CommandsNextConfiguration() { EnableMentionPrefix = true });
            commands.RegisterCommands<CommandsModule>();
            Works = true;
            Task.Run(() => MainAsync(client));
            Console.ReadKey();
            Works = false;
        }
        static async Task MainAsync(DiscordClient client)
        {
            try
            {
                await client.ConnectAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message + "\npress any key to exit.");
                return;
            }
            client.MessageCreated += (sender, args) =>
            {
                if (AutoRead.ContainsKey(args.Guild.Id) && AutoRead[args.Guild.Id] && args.Author != sender.CurrentUser && args.MentionedUsers.All(sender.Equals))
                {
                    LearnWords(args.Guild, args.Message.Content);
                }
                else if(!AutoRead.ContainsKey(args.Guild.Id))
                {
                    AutoRead.Add(args.Guild.Id, false);
                }
                return Task.CompletedTask;
            };
            while (Works) Thread.Sleep(450);
            await client.DisconnectAsync();
        }
    }
}