using CBS_bot;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace CBSBot.Commands
{
    public class TicketSystem : BaseCommandModule
    {
        private static ConcurrentDictionary<string, int> ticketCounters = new ConcurrentDictionary<string, int>();
        private static ConcurrentDictionary<ulong, DiscordChannel> activeSupportTickets = new ConcurrentDictionary<ulong, DiscordChannel>();
        private static ConcurrentDictionary<ulong, DiscordChannel> activeOrderTickets = new ConcurrentDictionary<ulong, DiscordChannel>();

        [Command("ticket")]
        [RequireRole(1251094814204760096)]
        public async Task TicketCommand(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Ticket System")
                .WithDescription("Please select a ticket type:")
                .WithColor(DiscordColor.Blue)
                .AddField("Support", "Create a support ticket for assistance.", inline: true)
                .AddField("Order", "Create an order ticket for making custom bot orders.", inline: true);

            var supportButton = new DiscordButtonComponent(ButtonStyle.Primary, "support", "Support");
            var orderButton = new DiscordButtonComponent(ButtonStyle.Secondary, "order", "Order");

            await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder()
                .AddEmbed(embed)
                .AddComponents(supportButton, orderButton));
        }

        public class TicketInteractions
        {
            private static readonly ulong SupportCategoryId = 1251149160938475601;
            private static readonly ulong OrderCategoryId = 1251149267146379355;
            private static readonly ulong SupportRoleId = 1251095769214222409;
            private static readonly ulong OrderRoleId = 1251095842736181298;

            public async Task HandleButtonInteraction(DiscordClient sender, ComponentInteractionCreateEventArgs e)
            {
                try
                {
                    ulong categoryId = 0;
                    ulong roleId = 0;
                    string channelNamePrefix = "";
                    string roleMention = "";

                    if (e.Id == "support")
                    {
                        categoryId = SupportCategoryId;
                        roleId = SupportRoleId;
                        channelNamePrefix = "support";
                        roleMention = $"<@&{SupportRoleId}>";

                        if (activeSupportTickets.ContainsKey(e.User.Id))
                        {
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder()
                                    .WithContent("You already have an active support ticket.")
                                    .AsEphemeral(true));
                            return;
                        }
                    }
                    else if (e.Id == "order")
                    {
                        categoryId = OrderCategoryId;
                        roleId = OrderRoleId;
                        channelNamePrefix = "order";
                        roleMention = $"<@&{OrderRoleId}>";

                        if (activeOrderTickets.ContainsKey(e.User.Id))
                        {
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder()
                                    .WithContent("You already have an active order ticket.")
                                    .AsEphemeral(true));
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }

                    int ticketNumber = ticketCounters.AddOrUpdate(channelNamePrefix, 1, (key, oldValue) => oldValue + 1);
                    var channelName = $"{channelNamePrefix}-{ticketNumber:D3}";

                    var category = e.Guild.GetChannel(categoryId);
                    var channel = await e.Guild.CreateChannelAsync(channelName, ChannelType.Text, category);

                    var everyoneRole = e.Guild.EveryoneRole;
                    var member = (DiscordMember)e.User;
                    await channel.AddOverwriteAsync(everyoneRole, Permissions.None, Permissions.AccessChannels | Permissions.SendMessages);
                    await channel.AddOverwriteAsync(member, Permissions.All, Permissions.None);

                    var embed = new DiscordEmbedBuilder()
                        .WithTitle("Ticket Actions")
                        .WithDescription($"Please choose an action. Only {roleMention} can claim this ticket.")
                        .WithColor(DiscordColor.Blurple);

                    var claimButton = new DiscordButtonComponent(ButtonStyle.Primary, "claim", "Claim");
                    var closeButton = new DiscordButtonComponent(ButtonStyle.Danger, "close", "Close");

                    var ticketMessage = await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(claimButton, closeButton));

                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .WithContent($"Your {channelNamePrefix} ticket has been created in {channel.Mention}.")
                            .AsEphemeral(true));

                    if (channelNamePrefix == "support")
                    {
                        activeSupportTickets[e.User.Id] = channel;
                    }
                    else if (channelNamePrefix == "order")
                    {
                        activeOrderTickets[e.User.Id] = channel;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in HandleButtonInteraction: {ex.Message}");
                    throw;
                }
            }
        }

        public class ClaimCloseInteractions
        {
            private static readonly ulong ClosedTicketsCategoryId = 1253340663882317824;
            private static readonly ulong SupportRoleId = 1251095769214222409;
            private static readonly ulong OrderRoleId = 1251095842736181298;

            public async Task HandleClaimCloseInteraction(DiscordClient sender, ComponentInteractionCreateEventArgs e)
            {
                try
                {
                    var channel = e.Channel;

                    if (e.Id == "claim")
                    {
                        var requiredRoleId = channel.Name.StartsWith("support") ? SupportRoleId : OrderRoleId;
                        var member = await channel.Guild.GetMemberAsync(e.User.Id);

                        if (member.Roles.Any(role => role.Id == requiredRoleId))
                        {
                            var claimEmbed = new DiscordEmbedBuilder()
                                .WithTitle("Ticket Claimed")
                                .WithDescription($"This ticket has been claimed by {e.User.Mention}.")
                                .WithColor(DiscordColor.Green);
                            await channel.SendMessageAsync(embed: claimEmbed);

                            var message = await channel.GetMessageAsync(e.Message.Id);
                            var newComponents = message.Components.Select(component =>
                            {
                                if (component is DiscordActionRowComponent actionRow)
                                {
                                    var newActionRow = new DiscordActionRowComponent(
                                        actionRow.Components.Select(c =>
                                        {
                                            if (c is DiscordButtonComponent button && button.CustomId == "claim")
                                            {
                                                return new DiscordButtonComponent(ButtonStyle.Primary, "claim", "Claim", true);
                                            }
                                            return c;
                                        }).ToArray()
                                    );
                                    return newActionRow;
                                }
                                return component;
                            }).ToArray();

                            var builder = new DiscordMessageBuilder()
                                .WithContent(message.Content)
                                .AddEmbeds(message.Embeds)
                                .AddComponents(newComponents.OfType<DiscordActionRowComponent>());

                            await message.ModifyAsync(builder);

                            var ticketCreatorId = channel.PermissionOverwrites.FirstOrDefault(perm => perm.Type == OverwriteType.Member)?.Id;
                            if (ticketCreatorId.HasValue)
                            {
                                var ticketCreator = await channel.Guild.GetMemberAsync(ticketCreatorId.Value);
                                await channel.AddOverwriteAsync(ticketCreator, Permissions.AccessChannels | Permissions.SendMessages, Permissions.None);
                                await channel.AddOverwriteAsync(member, Permissions.AccessChannels | Permissions.SendMessages, Permissions.None);
                                await channel.AddOverwriteAsync(channel.Guild.EveryoneRole, Permissions.None, Permissions.AccessChannels | Permissions.SendMessages);
                            }

                            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                        }
                        else
                        {
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder()
                                    .WithContent("You do not have the required role to claim this ticket.")
                                    .AsEphemeral(true));
                        }
                    }
                    else if (e.Id == "close")
                    {
                        var closeEmbed = new DiscordEmbedBuilder()
                            .WithTitle("Ticket Closed")
                            .WithDescription($"This ticket has been closed by {e.User.Mention}.")
                            .WithColor(DiscordColor.Red);
                        await channel.SendMessageAsync(embed: closeEmbed);
                        await Task.Delay(5000);

                        var ticketCreatorId = channel.PermissionOverwrites.FirstOrDefault(perm => perm.Type == OverwriteType.Member)?.Id;
                        if (ticketCreatorId.HasValue)
                        {
                            if (channel.Name.StartsWith("support"))
                            {
                                activeSupportTickets.TryRemove(ticketCreatorId.Value, out _);
                            }
                            else if (channel.Name.StartsWith("order"))
                            {
                                activeOrderTickets.TryRemove(ticketCreatorId.Value, out _);
                            }
                        }

                        await channel.DeleteAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in HandleClaimCloseInteraction: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
