namespace K4System
{
	using System.Text;
	using CounterStrikeSharp.API;
	using CounterStrikeSharp.API.Core;
	using CounterStrikeSharp.API.Modules.Commands;
	using CounterStrikeSharp.API.Modules.Utils;

	public sealed partial class Plugin : BasePlugin
	{
		public void Initialize_Commands()
		{
			AddCommand("css_k4", "More informations about K4-System",
				[CommandHelper(0, whoCanExecute: CommandUsage.CLIENT_ONLY)] (player, info) =>
			{
				if (player == null || !player.IsValid || player.PlayerPawn.Value == null)
					return;

				info.ReplyToCommand($" {Localizer["k4.general.prefix"]} {ChatColors.Lime}Available Commands:");

				CommandSettings commands = Config.CommandSettings;

				Dictionary<string, List<string>> commandCategories = new Dictionary<string, List<string>>
				{
					{ "Rank Commands", new List<string>() },
					{ "Stat Commands", commands.StatCommands },
					{ "Time Commands", commands.TimeCommands },
				};

				commandCategories["Rank Commands"].AddRange(commands.RankCommands);
				commandCategories["Rank Commands"].AddRange(commands.TopCommands);
				commandCategories["Rank Commands"].AddRange(commands.ResetMyCommands);
				commandCategories["Rank Commands"].AddRange(commands.RanksCommands);

				StringBuilder messageBuilder = new StringBuilder();

				foreach (var category in commandCategories)
				{
					if (category.Value.Count > 0)
					{
						messageBuilder.AppendLine($"--- {ChatColors.Silver}{category.Key}: {ChatColors.Lime}{category.Value[0]}{ChatColors.Silver}");

						for (int i = 1; i < category.Value.Count; i++)
						{
							if (i > 0 && i % 6 == 0)
							{
								info.ReplyToCommand(messageBuilder.ToString());
								messageBuilder.Clear();
							}

							if (messageBuilder.Length > 0)
							{
								messageBuilder.Append($"{ChatColors.Silver}, ");
							}

							messageBuilder.Append($" {ChatColors.Lime}{category.Value[i]}");
						}

						if (messageBuilder.Length > 0)
						{
							info.ReplyToCommand(messageBuilder.ToString());
							messageBuilder.Clear();
						}
					}
				}
			});
		}

		public void Initialize_Events()
		{
			RegisterEventHandler((EventRoundStart @event, GameEventInfo info) =>
			{
				if (!Config.GeneralSettings.SpawnMessage)
					return HookResult.Continue;

				List<CCSPlayerController> players = Utilities.GetPlayers();

				foreach (CCSPlayerController player in players)
				{
					if (player is null || !player.IsValid || !player.PlayerPawn.IsValid)
						continue;

					if (player.IsBot || player.IsHLTV)
						continue;

					player.PrintToChat($" {Localizer["k4.general.prefix"]} {ChatColors.Silver}The server is using the {ChatColors.Lime}K4-System {ChatColors.Silver}plugin. Type {ChatColors.Lime}!k4 {ChatColors.Silver}for more information!");
				}

				return HookResult.Continue;
			});
		}
	}
}