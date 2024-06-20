using CBS_bot;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace CBSBot.Commands
{
    public class Main : BaseCommandModule
    {
        [Command("announce")]
        [RequireRole(1252183151309230111)] 
        public async Task AnnounceAsync(CommandContext ctx, [RemainingText] string message)
        {
            ulong roleId = 1251096002484768899;

            ulong channelId = 1251058453435125843;

            var botAvatarUrl = ctx.Client.CurrentUser.AvatarUrl;

            var embed = new DiscordEmbedBuilder
            {
                Title = "Announcement",
                Description = message
            }.WithFooter("CustomBot-Studios", botAvatarUrl);

            var role = ctx.Guild.GetRole(roleId);

            var channel = ctx.Guild.GetChannel(channelId) as DiscordChannel;

            await channel.SendMessageAsync(role.Mention, embed: embed);
        }

        [Command("delete")]
        [RequireRole(1252183151309230111)]
        public async Task DeleteMessagesCommand(CommandContext ctx, int count = 0)
        {
            if (count <= 0 || count > 1000)
            {
                await ctx.RespondAsync("Please specify a number between 1 and 100 for messages to delete.");
                return;
            }

            var messagesToDelete = await ctx.Channel.GetMessagesAsync(count + 1);
            await ctx.Channel.DeleteMessagesAsync(messagesToDelete);

            var confirmation = await ctx.RespondAsync($"Deleted {messagesToDelete.Count} message(s).");

            await Task.Delay(5000);
            await confirmation.DeleteAsync();
        }

        [Command("products")]
        [Description("Displays a list of available products.")]
        [RequireRole(1252183151309230111)]
        public async Task ProductsCommand(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Our Products",
                Description = "We provide custom bots for various purposes.",
                Color = DiscordColor.Blue

            }.WithFooter("CustomBot-Studios", ctx.Client.CurrentUser.AvatarUrl);

            embed.AddField("Custom Bots", "We offer custom bot development services for Discord, Telegram, and more.", inline: false);
            embed.AddField("Bot Templates", "Get pre-made bot templates for various use cases, such as customer support or entertainment.", inline: false);
            embed.AddField("Bot Hosting", "We offer reliable bot hosting services to ensure your bot is always online.", inline: false);

            await ctx.RespondAsync(embed: embed.Build());
        }

        [Command("rules")]
        [Description("Displays a list of rules.")]
        [RequireRole(1252183151309230111)]
        public async Task Rules(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Rules",
                Description = "A simple list of rules you need to follow",
                Color = DiscordColor.Blue

            }.WithFooter("CustomBot-Studios", ctx.Client.CurrentUser.AvatarUrl);

            embed.AddField("Rule 1", "Be respectful to all members.", inline: false);
            embed.AddField("Rule 2", "No spamming or self-promotion.", inline: false);
            embed.AddField("Rule 3", "Keep conversations civil and on-topic.", inline: false);
            embed.AddField("Rule 4", "Only request custom bots for personal use, not for resale or distribution.", inline: false);
            embed.AddField("Rule 5", "Do not share or distribute custom bot codes without permission from the creator.", inline: false);
            embed.AddField("Rule 6", "Follow the instructions provided by the bot creators and administrators.", inline: false);
            embed.AddField("Rule 7", "This server is a place where you can get custom bots.", inline: false);

            await ctx.RespondAsync(embed: embed.Build());
        }
    }
}
