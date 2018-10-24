using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Weebot.Modules
{
    public class RoleNameChange : ModuleBase<SocketCommandContext>
    {
        private Timer timer;
        private static int timerInterval = 10000;
        private Random random = new Random();

        private static SocketRole roleToEdit;
        private static List<string> firstStringSet = new List<string>();
        private static List<string> secondStringSet = new List<string>();
        private static List<string> thirdStringSet = new List<string>();

        [Command("Help"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task HelpMessageAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Commands")
                .AddField("Misc",               "`LoadSave`")         
                .AddField("Role commands",      "`SetRole <@role>` " +
                                                "`GetRoles` " +
                                                "`SetRole <ID>` " +
                                                "`GetCurrentRole`")
                .AddField("First String Set",   "`AddToFirstStringSet <string>` " +
                                                "`GetFirstStringSet` " +
                                                "`RemoveFromFirstStringSet <index>`" +
                                                "`WipeFirstStringSet`")
                .AddField("Second String Set",  "`AddToSecondStringSet <string>` " +
                                                "`GetSecondStringSet` " +
                                                "`RemoveFromSecondStringSet <index>`" +
                                                "`WipeSecondStringSet`")
                .AddField("Third String Set",   "`AddToThirdStringSet <string>` " +
                                                "`GetThirdStringSet` " +
                                                "`RemoveFromThirdStringSet <index>`" +
                                                "`WipeThirdStringSet`")
                .AddField("Timer",              "`StartNewTimer` " +
                                                "`SetTimerInterval <time>`")
                .WithColor(Color.Magenta);

            await ReplyAsync("", false, builder.Build());
        }

        #region Misc
        [Command("LoadSave"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task LoadSaveAsync()
        {
            if (DataStorage.PairHasKey("roleToEdit"))
            {
                roleToEdit = Context.Guild.GetRole(Convert.ToUInt64(DataStorage.GetKeyValue("roleToEdit")));
            }

            if (DataStorage.PairHasKey("timerInterval"))
            {
                timerInterval = Convert.ToInt32(DataStorage.GetKeyValue("timerInterval"));
            }

            if (DataStorage.PairHasKey("firstStringSetCount"))
            {
                int count = Convert.ToInt32(DataStorage.GetKeyValue("firstStringSetCount"));
                for (int i = 0; i < count; i++)
                {
                    firstStringSet.Add(DataStorage.GetKeyValue("firstStringSet" + i));
                }
            }

            if (DataStorage.PairHasKey("secondStringSetCount"))
            {
                int count = Convert.ToInt32(DataStorage.GetKeyValue("secondStringSetCount"));
                for (int i = 0; i < count; i++)
                {
                    secondStringSet.Add(DataStorage.GetKeyValue("secondStringSet" + i));
                }
            }

            if (DataStorage.PairHasKey("thirdStringSetCount"))
            {
                int count = Convert.ToInt32(DataStorage.GetKeyValue("thirdStringSetCount"));
                for (int i = 0; i < count; i++)
                {
                    thirdStringSet.Add(DataStorage.GetKeyValue("thirdStringSet" + i));
                }
            }

            await ReplyAsync("Loaded previously set role, timer interval and string sets");
            await Task.CompletedTask;
        }
        #endregion

        #region Roles
        [Command("SetRole"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetRoleAsync(SocketRole role)
        {
            var user = Context.User;
            roleToEdit = role;

            DataStorage.AddPairToStorage("roleToEdit", roleToEdit.Id.ToString());

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

            DataStorage.AddPairToStorage("roleToEdit", roleToEdit.Id.ToString());

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
                    await ReplyAsync($"{roleToEdit.Mention} is currently assigned!");
                }
                else
                {
                    await ReplyAsync($"{roleToEdit.Name} is currently assigned!");
                }
            }
            else
            {
                await ReplyAsync("There is currently no role assigned!");
            }
        }
        #endregion

        #region First String Set
        [Command("AddToFirstStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task AddToFirstSetStringAsync(string name)
        {
            var user = Context.User;
            firstStringSet.Add(name);

            for (int i = 0; i < firstStringSet.Count; i++)
            {
                DataStorage.AddPairToStorage(string.Format("firstStringSet" + i), firstStringSet[i]);
            }

            DataStorage.AddPairToStorage("firstStringSetCount", firstStringSet.Count.ToString());

            await ReplyAsync($"{user.Mention} has added \"{name}\" to be the first part of the role to be edited!");
        }

        [Command("GetFirstStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task GetFirstStringSetAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();
            StringBuilder stringBuilder = new StringBuilder();

            builder.WithTitle("First string set")
                .WithColor(Color.Magenta);

            for (int i = 0; i < firstStringSet.Count; i++)
            {
                stringBuilder.Append($"**{firstStringSet[i]} - ** `{i}`");
                stringBuilder.AppendLine();
            }

            builder.WithDescription(stringBuilder.ToString());

            await ReplyAsync("", false, builder.Build());
        }

        [Command("RemoveFromFirstStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task RemoveFromFirstStringSetAsync(int index)
        {
            EmbedBuilder builder = new EmbedBuilder();

            if (index >= 0 && index < firstStringSet.Count)
            {
                builder.WithTitle($"String \"{firstStringSet[index].ToString()}\" removed")
                .WithColor(Color.Magenta);
                
                // Dirty but it works
                // Remove all pairs
                for (int i = 0; i < firstStringSet.Count; i++)
                {
                    DataStorage.RemovePairFromStorage(string.Format("firstStringSet" + i));
                }

                firstStringSet.RemoveAt(index);

                // Readd all pairs
                for (int i = 0; i < firstStringSet.Count; i++)
                {
                    DataStorage.AddPairToStorage(string.Format("firstStringSet" + i), firstStringSet[i]);
                }

                DataStorage.AddPairToStorage("firstStringSetCount", firstStringSet.Count.ToString());
            }
            else
            {
                builder.WithTitle("Index is not valid, please try again!")
                .WithColor(Color.Magenta);
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("WipeFirstStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task WipeFirstStringSetAsync()
        {
            if (firstStringSet.Count == 0)
            {
                await ReplyAsync("Set is already empty!");
            }

            for (int i = 0; i < firstStringSet.Count; i++)
            {
                DataStorage.RemovePairFromStorage("firstStringSet" + i);
            }

            firstStringSet.Clear();
            DataStorage.RemovePairFromStorage("firstStringSetCount");
            await ReplyAsync("Emptied the set!");
        }
        #endregion

        #region Second String Set
        [Command("AddToSecondStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task AddToSecondStringSetAsync(string name)
        {
            var user = Context.User;
            secondStringSet.Add(name);

            for (int i = 0; i < secondStringSet.Count; i++)
            {
                DataStorage.AddPairToStorage(string.Format("secondStringSet" + i), secondStringSet[i]);
            }

            DataStorage.AddPairToStorage("secondStringSetCount", secondStringSet.Count.ToString());

            await ReplyAsync($"{user.Mention} has added \"{name}\" to be the second part of the role to be edited!");
        }

        [Command("GetSecondStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task GetSecondStringSetAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();
            StringBuilder stringBuilder = new StringBuilder();

            builder.WithTitle("Second string set")
                .WithColor(Color.Magenta);

            for (int i = 0; i < secondStringSet.Count; i++)
            {
                stringBuilder.Append($"**{secondStringSet[i]} - ** `{i}`");
                stringBuilder.AppendLine();
            }

            builder.WithDescription(stringBuilder.ToString());

            await ReplyAsync("", false, builder.Build());
        }

        [Command("RemoveFromSecondStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task RemoveFromSecondStringSetAsync(int index)
        {
            EmbedBuilder builder = new EmbedBuilder();

            if (index >= 0 && index < secondStringSet.Count)
            {
                builder.WithTitle($"String \"{secondStringSet[index].ToString()}\" removed")
                .WithColor(Color.Magenta);
                
                // Dirty but it works
                // Remove all pairs
                for (int i = 0; i < secondStringSet.Count; i++)
                {
                    DataStorage.RemovePairFromStorage(string.Format("secondStringSet" + i));
                }

                secondStringSet.RemoveAt(index);

                // Readd all pairs
                for (int i = 0; i < secondStringSet.Count; i++)
                {
                    DataStorage.AddPairToStorage(string.Format("secondStringSet" + i), secondStringSet[i]);
                }

                DataStorage.AddPairToStorage("secondStringSetCount", secondStringSet.Count.ToString());
            }
            else
            {
                builder.WithTitle("Index is not valid, please try again!")
                .WithColor(Color.Magenta);
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("WipeSecondStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task WipeSecondStringSetAsync()
        {
            if (secondStringSet.Count == 0)
            {
                await ReplyAsync("Set is already empty!");
            }

            for (int i = 0; i < secondStringSet.Count; i++)
            {
                DataStorage.RemovePairFromStorage("secondStringSet" + i);
            }

            secondStringSet.Clear();
            DataStorage.RemovePairFromStorage("secondStringSetCount");
            await ReplyAsync("Emptied the set!");
        }
        #endregion

        #region Third String Set
        [Command("AddToThirdStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task AddToThirdStringSetAsync(string name)
        {
            var user = Context.User;
            thirdStringSet.Add(name);

            for (int i = 0; i < thirdStringSet.Count; i++)
            {
                DataStorage.AddPairToStorage(string.Format("thirdStringSet" + i), thirdStringSet[i]);
            }

            DataStorage.AddPairToStorage("thirdStringSetCount", thirdStringSet.Count.ToString());

            await ReplyAsync($"{user.Mention} has added \"{name}\" to be the third part of the role to be edited!");
        }

        [Command("GetThirdStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task GetThirdStringSetAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();
            StringBuilder stringBuilder = new StringBuilder();

            builder.WithTitle("Third string set")
                .WithColor(Color.Magenta);

            for (int i = 0; i < thirdStringSet.Count; i++)
            {
                stringBuilder.Append($"**{thirdStringSet[i]} - ** `{i}`");
                stringBuilder.AppendLine();
            }

            builder.WithDescription(stringBuilder.ToString());

            await ReplyAsync("", false, builder.Build());
        }

        [Command("RemoveFromThirdStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task RemoveFromThirdStringSetAsync(int index)
        {
            EmbedBuilder builder = new EmbedBuilder();

            if (index >= 0 && index < thirdStringSet.Count)
            {
                builder.WithTitle($"String \"{thirdStringSet[index].ToString()}\" removed")
                .WithColor(Color.Magenta);
                
                // Dirty but it works
                // Remove all pairs
                for (int i = 0; i < thirdStringSet.Count; i++)
                {
                    DataStorage.RemovePairFromStorage(string.Format("thirdStringSet" + i));
                }

                thirdStringSet.RemoveAt(index);

                // Readd all pairs
                for (int i = 0; i < thirdStringSet.Count; i++)
                {
                    DataStorage.AddPairToStorage(string.Format("thirdStringSet" + i), thirdStringSet[i]);
                }

                DataStorage.AddPairToStorage("thirdStringSetCount", thirdStringSet.Count.ToString());
            }
            else
            {
                builder.WithTitle("Index is not valid, please try again!")
                .WithColor(Color.Magenta);
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("WipeThirdStringSet"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task WipeThirdStringSetAsync()
        {
            if (thirdStringSet.Count == 0)
            {
                await ReplyAsync("Set is already empty!");
            }

            for (int i = 0; i < thirdStringSet.Count; i++)
            {
                DataStorage.RemovePairFromStorage("thirdStringSet" + i);
            }

            thirdStringSet.Clear();
            DataStorage.RemovePairFromStorage("thirdStringSetCount");
            await ReplyAsync("Emptied the set!");
        }
        #endregion

        #region Timer
        [Command("StartNewTimer"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartNewTimerAsync()
        {
            if (firstStringSet.Count > 0 && roleToEdit != null)
            {
                timer = new Timer();
                timer.Interval = timerInterval; // 12 hour interval
                timer.Elapsed += OnTimerElapsed;
                timer.AutoReset = true;
                timer.Enabled = true;
                await ReplyAsync("A new timer has been started.");
            }
            else if (firstStringSet.Count == 0 && roleToEdit != null)
            {
                await ReplyAsync("Please add some names in the first set. \"owo.help\" for more info.");
            }
            else if (firstStringSet.Count > 0 && roleToEdit == null)
            {
                await ReplyAsync("Please set a role to be edited. \"owo.help\" for more info.");
            }
            else if (firstStringSet.Count == 0 && roleToEdit == null)
            {
                await ReplyAsync("Please add some names in the first set and set a role to be edited. \"owo.help\" for more info.");
            }
        }

        [Command("SetTimerInterval"), RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetTimerIntervalAsync(int value)
        {
            var user = Context.User;
            timerInterval = value;

            DataStorage.AddPairToStorage("timerInterval", timerInterval.ToString());

            await ReplyAsync($"{user.Mention} set the timer interval set to {timerInterval} ms.");
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            string first, second, third;
            StringBuilder builder = new StringBuilder();

            first = firstStringSet[random.Next(0, firstStringSet.Count)].ToString();
            builder.Append(first);

            if (secondStringSet.Count > 0)
            {
                second = secondStringSet[random.Next(0, secondStringSet.Count)].ToString();
                builder.Append(second);
            }

            if (thirdStringSet.Count > 0)
            {
                third = thirdStringSet[random.Next(0, thirdStringSet.Count)].ToString();
                builder.Append(third);
            }

            await ReplyAsync($"Role name: {builder.ToString()}");

            await roleToEdit.ModifyAsync(role => role.Name = builder.ToString());
        }
        #endregion
    }
}
