using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace CBS_bot
{
    public class RequireRoleAttribute : CheckBaseAttribute
    {
        public ulong RoleId { get; }

        public RequireRoleAttribute(ulong roleId)
        {
            RoleId = roleId;
        }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);
            return member.Roles.Any(role => role.Id == RoleId);
        }
    }
}
