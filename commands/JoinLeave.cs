using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;

namespace CBSBot.Commands
{
    public class JoinLeave
    {
        private readonly ulong welcomeChannelId = 1251833573305225217;
        private readonly ulong goodbyeChannelId = 1251833604657381397;
        private readonly ulong joinRoleId = 1251096002484768899;
        private readonly DiscordClient client;

        public JoinLeave(DiscordClient client)
        {
            this.client = client;
            client.GuildMemberAdded += Client_GuildMemberAdded;
            client.GuildMemberRemoved += Client_GuildMemberRemoved;
        }

        private async Task Client_GuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            try
            {
                var welcomeChannel = await sender.GetChannelAsync(welcomeChannelId) as DiscordChannel;
                if (welcomeChannel == null)
                    return;

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Welcome to the Server!",
                    Description = $"Welcome, {e.Member.Mention}, to our server!",
                    Color = DiscordColor.Green
                };
                embed.WithThumbnail(e.Member.AvatarUrl);
                embed.WithFooter("CustomBot-Studios", client.CurrentUser.AvatarUrl);

                await welcomeChannel.SendMessageAsync(embed: embed);

                var guild = await sender.GetGuildAsync(e.Guild.Id);
                var role = guild.GetRole(joinRoleId);
                if (role != null)
                    await e.Member.GrantRoleAsync(role);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Client_GuildMemberAdded: {ex.Message}");
            }
        }

        private async Task Client_GuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {
            try
            {
                var goodbyeChannel = await sender.GetChannelAsync(goodbyeChannelId) as DiscordChannel;
                if (goodbyeChannel == null)
                    return;

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Goodbye!",
                    Description = $"Goodbye, {e.Member.Username}, we will miss you!",
                    Color = DiscordColor.Red
                };
                embed.WithThumbnail(e.Member.AvatarUrl);
                embed.WithFooter("CustomBot-Studios", client.CurrentUser.AvatarUrl); 

                await goodbyeChannel.SendMessageAsync(embed: embed);

                var guild = await sender.GetGuildAsync(e.Guild.Id);
                var role = guild.GetRole(joinRoleId);
                if (role != null)
                    await e.Member.RevokeRoleAsync(role);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Client_GuildMemberRemoved: {ex.Message}");
            }
        }
    }
}
