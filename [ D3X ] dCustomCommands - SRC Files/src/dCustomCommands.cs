using CounterStrikeSharp.API; 
using CounterStrikeSharp.API.Core; 
using CounterStrikeSharp.API.Core.Attributes.Registration; 
using CounterStrikeSharp.API.Modules.Admin; 
using CounterStrikeSharp.API.Modules.Commands; 
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;

namespace dCustomCommands; 
 
public class dCustomCommands : BasePlugin 
{ 
    public override string ModuleName => "[CS2] D3X - [ Niestandardowe Komendy ]";
    public override string ModuleAuthor => "D3X";
    public override string ModuleDescription => " Plugin umożliwia stworzenie własnej komendy na serwerze.";
    public override string ModuleVersion => "1.0.0";
    private Dictionary<string, CommandInfo> commands;
    private string configPath;

    public override void Load(bool hotReload)
    {
        configPath = Path.Combine(ModuleDirectory, "Config.json");
        LoadConfig();

        foreach (var commandName in commands.Keys)
        {
            string cssCommandName = $"css_{commandName.ToLower()}";
            AddCommand(cssCommandName, "", (player, info) =>
            {
                ExecuteCommand(player, commandName);
            });
        }
    }

    private void LoadConfig()
    {
        if (!File.Exists(configPath))
        {
            CreateDefaultConfig();
            return;
        }

        string json = File.ReadAllText(configPath);
        var config = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, CommandInfo>>>(json);
        commands = config["Komendy"];
    }

    private void CreateDefaultConfig()
    {
        var defaultCommands = new Dictionary<string, CommandInfo>
        {
            ["Steam"] = new CommandInfo
            {
                Message = "{GREEN}Nasza Grupa Steam: {DEFAULT}https://steamcommunity.com/group/cszjarani",
            },
            ["Discord"] = new CommandInfo
            {
                Message = "{GREEN}Nasz serwer Discord: {DEFAULT}https://discord.gg/SMWD8zn21i",
            }
        };

        var config = new Dictionary<string, Dictionary<string, CommandInfo>> { ["Komendy"] = defaultCommands };
        string json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(configPath, json);
        commands = defaultCommands;
    }

    private void ExecuteCommand(CCSPlayerController player, string commandName)
    {
        if (commands.TryGetValue(commandName, out var commandInfo))
        {
            string prefix = $" {ChatColors.DarkRed}► {ChatColors.Green}[{ChatColors.DarkRed} {commandName.ToUpper()} {ChatColors.Green}] {ChatColors.Green}✔{ChatColors.Lime}";
            string message = ParseMessage(commandInfo.Message, prefix);
            player.PrintToChat(message);
        }
    }

    private string ParseMessage(string message, string prefix)
    {
        var colorMapping = new Dictionary<string, char>
        {
            { "DEFAULT", '\x01' },
            { "WHITE", '\x01' },
            { "DARKRED", '\x02' },
            { "GREEN", '\x04' },
            { "LIGHTYELLOW", '\x09' },
            { "LIGHTBLUE", '\x0B' },
            { "OLIVE", '\x05' },
            { "LIME", '\x06' },
            { "RED", '\x07' },
            { "LIGHTPURPLE", '\x03' },
            { "PURPLE", '\x0E' },
            { "GREY", '\x08' },
            { "YELLOW", '\x09' },
            { "GOLD", '\x10' },
            { "SILVER", '\x0A' },
            { "BLUE", '\x0B' },
            { "DARKBLUE", '\x0C' },
            { "BLUEGREY", '\x0A' },
            { "MAGENTA", '\x0E' },
            { "LIGHTRED", '\x0F' },
            { "ORANGE", '\x10' }
        };

        foreach (var color in colorMapping)
        {
            message = message.Replace($"{{{color.Key}}}", color.Value.ToString());
        }

        message = message.Replace("\n", "\u2029" + prefix + " ");
        return prefix + " " + message;
    }

    private class CommandInfo
    {
        public string Message { get; set; }
    }
} 
