using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;

namespace CBSBot.Commands
{
    public class ReviewModule : BaseCommandModule
    {
        [Command("review")]
        public async Task ReviewCommand(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync();

            var modal = new DiscordMessageBuilder()
                .WithContent("Please answer the following questions:")
                .AddComponents(new TextInputComponent("Question 1", "q1", placeholder: "Enter your response here"),
                               new TextInputComponent("Question 2", "q2", placeholder: "Enter your response here"),
                               new TextInputComponent("Question 3", "q3", placeholder: "Enter your response here"));

            var interactivity = ctx.Client.GetInteractivity();
            var response = await interactivity.WaitForMessageAsync(
                x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id,
                TimeSpan.FromMinutes(2));

            if (response.TimedOut)
            {
                await ctx.RespondAsync("You took too long to respond.");
                return;
            }

            var q1Response = response.Result.Content; 
            var q2Response = "User response to question 2";
            var q3Response = "User response to question 3";

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Review Results")
                .AddField("Question 1", q1Response)
                .AddField("Question 2", q2Response)
                .AddField("Question 3", q3Response)
                .WithColor(DiscordColor.Blurple)
                .Build();

            await ctx.Channel.SendMessageAsync(embed);
        }
    }
}
