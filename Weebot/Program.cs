using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Timers;
using System.Threading.Tasks;

namespace Weebot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient discordClient;
        private CommandService commands;
        private IServiceProvider serviceProvider;

        public async Task RunBotAsync()
        {
            discordClient = new DiscordSocketClient();
            commands = new CommandService();

            serviceProvider = new ServiceCollection()
                .AddSingleton(discordClient)
                .AddSingleton(commands)
                .BuildServiceProvider();

            string botToken = "NTA0Mjg0OTM4ODU4NjU5ODYy.DrC1IQ.wwrxQs1WCHbrEdR42S2Kh6ihgTY";

            discordClient.Log += Log;

            await RegisterCommandsAsync();

            await discordClient.LoginAsync(TokenType.Bot, botToken);
            await discordClient.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            discordClient.MessageReceived += HandleCommandAsync;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot)
                return;

            int argPosition = 0;

            if (message.HasStringPrefix("owo.", ref argPosition) || message.HasMentionPrefix(discordClient.CurrentUser, ref argPosition))
            {
                var context = new SocketCommandContext(discordClient, message);

                var result = await commands.ExecuteAsync(context, argPosition, serviceProvider);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
