using CBS_bot.other;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace CBS_bot.commands
{
    public class TestCommands : BaseCommandModule
    {
        [Command("test")]
        public async Task MyFirstCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Hello {ctx.User.Mention}");
        }

        [Command("add")]
        public async Task Add(CommandContext ctx, int number1, int number2)
        {
            int result = number1 + number2;
            await ctx.Channel.SendMessageAsync($"I have calculated your issue and your answer is: {result}");
        }

        [Command("subtract")]
        public async Task Subtract(CommandContext ctx, int number1, int number2)
        {
            int result = number1 - number2;
            await ctx.Channel.SendMessageAsync($"I have calculated your issue and your answer is: {result}");
        }

        [Command("multiply")]
        public async Task Multiply(CommandContext ctx, int number1, int number2)
        {
            int result = number1 *number2;
            await ctx.Channel.SendMessageAsync($"I have calculated your issue and your answer is: {result}");
        }

        [Command("devide")]
        public async Task Devide(CommandContext ctx, int number1, int number2)
        {
            int result = number1 / number2;
            await ctx.Channel.SendMessageAsync($"I have calculated your issue and your answer is: {result}");
        }

        [Command("embed")]
        public async Task EmbedMessage(CommandContext ctx)
        {
            var message = new DiscordEmbedBuilder
            {
                Title = "This is an embed.",
                Description = $"This embed has been created by {ctx.User.Mention}"
            };

            await ctx.Channel.SendMessageAsync(embed: message);
        }

        [Command("cardgame")]
        public async Task CardGame(CommandContext ctx)
        {
            var userCard = new CardSystem();

            var userCardEmbed = new DiscordEmbedBuilder
            {
                Title = $"Your card is {userCard.SelectedCard}",
                Color = DiscordColor.Lilac
            };

            await ctx.Channel.SendMessageAsync(embed: userCardEmbed);

            var botCard = new CardSystem();

            var botCardEmbed = new DiscordEmbedBuilder
            {
                Title = $"The Bot drew a {botCard.SelectedCard}",
                Color = DiscordColor.Orange
            };

            await ctx.Channel.SendMessageAsync(embed: botCardEmbed);

            if (userCard.SelectedNumber > botCard.SelectedNumber)
            {
                var winMessage = new DiscordEmbedBuilder
                {
                    Title = "Congratulations, You Win!!!",
                    Color = DiscordColor.Green
                };

                await ctx.Channel.SendMessageAsync(embed: winMessage);
            }

            else
            {
                var loseMessage = new DiscordEmbedBuilder
                {
                    Title = "You Lost The Game!!",
                    Color = DiscordColor.Red
                };

                await ctx.Channel.SendMessageAsync(embed: loseMessage);
            }
        }
    }
}
