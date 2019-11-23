using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace BGS_SitRep
{
    class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var message = s as SocketUserMessage;
            if (message == null) return;

            var context = new SocketCommandContext(_client, message);
            int argPos = 0;

            if (message.HasStringPrefix(Config.bot.cmdPrefix, ref argPos) 
                || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos, null);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }

        }
    }
}
