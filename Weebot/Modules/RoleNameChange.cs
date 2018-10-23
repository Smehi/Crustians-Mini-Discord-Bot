using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Weebot.Modules
{
    public class RoleNameChange : ModuleBase<SocketCommandContext>
    {
        private SocketRole roleToEdit;

        private List<String> firstPart = new List<string>();
        private List<String> secondPart = new List<string>();
        private List<String> thirdPart = new List<string>();

        [Command("Help"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task HelpMessageAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Roles")
                .AddField("SetRole <@role>", "Set the role that is going to get the name changed.")
                .AddField("GetRoles", "Get the roles in the server.")
                .AddField("SetRole <ID>", "Set the role, with the index from the role list, that is going to get the name changed.")
                .AddField("GetCurrentRole", "Gets the current role that is assigned.")
                .WithColor(Color.Magenta);
            await ReplyAsync("", false, builder.Build());
            builder = new EmbedBuilder();

            builder.WithTitle("First Part")
                .AddField("AddFirstString", "Add string to first list.")
                .AddField("GetFirstStringList", "Get first list.")
                .AddField("RemoveFirstStringList <index>", "Remove from first list at index.")
                .WithColor(Color.Magenta);
            await ReplyAsync("", false, builder.Build());
            builder = new EmbedBuilder();

            builder.WithTitle("Second Part")
                .AddField("AddSecondString", "(Optional) Add string to second list.")
                .AddField("GetSecondStringList", "Get second list.")
                .AddField("RemoveSecondStringList <index>", "Remove from second list at index.")
                .WithColor(Color.Magenta);
            await ReplyAsync("", false, builder.Build());
            builder = new EmbedBuilder();

            builder.WithTitle("Third Part")
                .AddField("AddThirdString", "(Optional) Add string to third list.")
                .AddField("GetThirdStringList", "Get third list.")
                .AddField("RemoveThirdStringList <index>", "Remove from third list at index.")
                .WithColor(Color.Magenta);
            await ReplyAsync("", false, builder.Build());
        }

        #region Roles
        [Command("SetRole"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetRoleAsync(SocketRole role)
        {
            var user = Context.User;
            roleToEdit = role;

            await ReplyAsync($"{user.Mention} has set {roleToEdit.Mention} to be edited!");
        }

        [Command("GetRoles"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task GetRolesAsync()
        {
            var guild = Context.Guild;

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Roles in this server")
                .WithColor(Color.Magenta);

            foreach (var role in guild.Roles)
            {
                builder.AddField($"{role.Name}", $"{role.Id}");
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("SetRole"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetRoleIndexAsync(ulong roleID)
        {
            var user = Context.User;
            var guild = Context.Guild;
            roleToEdit = guild.GetRole(roleID);

            if (roleToEdit.IsMentionable)
            {
                await ReplyAsync($"{user.Mention} has set {roleToEdit.Mention} to be edited!");
            }
            else
            {
                await ReplyAsync($"{user.Mention} has set a role to be edited!");
            }
        }

        [Command("GetCurrentRole"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task GetCurrentRoleAsync()
        {
            if (roleToEdit != null)
            {
                if (roleToEdit.IsMentionable)
                {
                    await ReplyAsync(roleToEdit.Mention);
                }
                else
                {
                    await ReplyAsync(roleToEdit.Name);
                } 
            }
            else
            {
                await ReplyAsync("There is currently no role assigned!");
            }
        }
        #endregion

        #region First Part
        [Command("AddFirstString"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task AddFirstStringAsync([Remainder] String name)
        {
            var user = Context.User;
            firstPart.Add(name);

            await ReplyAsync($"{user.Mention} has added {name} to be the first part of the role to be edited!");
        }

        [Command("GetFirstStringList"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task GetFirstStringListAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("First part")
                .WithColor(Color.Magenta);

            for (int i = 0; i < firstPart.Count; i++)
            {
                builder.AddField($"{firstPart[i]}", $"{i}");
            }

            await ReplyAsync("", false, builder.Build());
        }
        #endregion

        #region Second Part
        [Command("AddSecondString"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task AddSecondStringAsync([Remainder] String name)
        {
            var user = Context.User;
            secondPart.Add(name);

            await ReplyAsync($"{user.Mention} has added {name} to be the first part of the role to be edited!");
        }

        [Command("GetSecondStringList"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task GetSecondStringListAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Second part")
                .WithColor(Color.Magenta);

            for (int i = 0; i < firstPart.Count; i++)
            {
                builder.AddField($"{secondPart[i]}", $"{i}");
            }

            await ReplyAsync("", false, builder.Build());
        }
        #endregion

        #region Third Part
        [Command("AddThirdString"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task AddThirdStringAsync([Remainder] String name)
        {
            var user = Context.User;
            thirdPart.Add(name);

            await ReplyAsync($"{user.Mention} has added {name} to be the first part of the role to be edited!");
        }

        [Command("GetThirdStringList"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task GetThirdStringListAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Third part")
                .WithColor(Color.Magenta);

            for (int i = 0; i < firstPart.Count; i++)
            {
                builder.AddField($"{thirdPart[i]}", $"{i}");
            }

            await ReplyAsync("", false, builder.Build());
        }
        #endregion
    }
}
