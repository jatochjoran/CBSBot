using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using CBSBot.Commands;
using CBSBot.TicketSystem;
using System;
using System.Threading.Tasks;
using CBS_bot.commands;
using static CBS_bot.commands.TicketSystem;

namespace CBSBot
{
    internal class Program
    {
        private static DiscordClient Client;
        private static CommandsNextExtension Commands;
        private static JoinLeave JoinLeaveHandler;

        static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            var discordConfig = new DiscordConfiguration
            {
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.All
            };

            Client = new DiscordClient(discordConfig);

            JoinLeaveHandler = new JoinLeave(Client);

            Client.Ready += Client_Ready;
            Client.ComponentInteractionCreated += Client_ComponentInteractionCreated;

            var interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<TestCommands>();
            Commands.RegisterCommands<Main>();
            Commands.RegisterCommands<TicketSystem>();
            Commands.RegisterCommands<ReviewModule>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            sender.Logger.LogInformation("Bot is connected and ready!");
            return Task.CompletedTask;
        }

        private static async Task Client_ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            if (e.Id == "support" || e.Id == "order")
            {
                var ticketSystem = new TicketInteractions();
                await ticketSystem.HandleButtonInteraction(sender, e);
            }
            else if (e.Id == "claim" || e.Id == "close")
            {
                var claimCloseInteractions = new ClaimCloseInteractions();
                await claimCloseInteractions.HandleClaimCloseInteraction(sender, e);
            }
        }
    }
}
