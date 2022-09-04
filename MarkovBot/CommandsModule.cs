using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Markov;

namespace GenaBot
{
    class CommandsModule : BaseCommandModule
    {
        [Command("learn"), Description("Learn <count> words from this channel.")]
        public async Task ReadCmd(CommandContext ctx, int count)
        {
            IReadOnlyList<DiscordMessage> messages = await ctx.Channel.GetMessagesAsync(count);
            DiscordMessage msg = await ctx.Channel.SendMessageAsync("Reading + Learning messages: 0\\" + count);
            int i = 0;
            int PreviousI = 0;
            foreach (DiscordMessage message in messages)
            {
                i++;
                if(i > PreviousI && message.Author != ctx.Client.CurrentUser)
                {
                    await msg.ModifyAsync("Reading + Learning messages:" + i + "\\" + count);
                    PreviousI = i + 100;
                }
                if(message.Content != "")
                    Program.LearnWords(ctx.Guild, message.Content);
            }
            await msg.ModifyAsync("success");
        }
        [Command("learnfrom"), Description("Learn <from> to <count> words from this channel.")]
        public async Task ReadFromCmd(CommandContext ctx, int index, int count)
        {
            IReadOnlyList<DiscordMessage> messages = await ctx.Channel.GetMessagesAfterAsync((ulong)index, count);
            DiscordMessage msg = await ctx.Channel.SendMessageAsync("Reading + Learning messages: 0\\" + count);
            int i = 0;
            int PreviousI = 0;
            foreach (DiscordMessage message in messages)
            {
                i++;
                if (i > PreviousI && message.Author != ctx.Client.CurrentUser)
                {
                    await msg.ModifyAsync("Reading + Learning messages:" + i + "\\" + count);
                    PreviousI = i + 100;
                }
                if (message.Content != "")
                    Program.LearnWords(ctx.Guild, message.Content);
            }
            await msg.ModifyAsync("success");
        }
        [Command("autolearn"), Description("Set <bool> for automatycly learn.")]
        public async Task SetAutoLearn(CommandContext ctx, bool b)
        {
            await ctx.Channel.SendMessageAsync("AutoLearn in this guild is now " + b);
            Program.HookAutoRead(ctx.Guild, b);
        }
        [Command("generate"), Description("Generate message.")]
        public async Task SetAutoLearn(CommandContext ctx)
        {
            MarkovChain<string> chain = new MarkovChain<string>(1);
            List<List<string>>? learned = Program.GetLearnedWordsFromGuild(ctx.Guild);
            if (learned != null)
            {
                for(int i = 0; i < learned.Count; i++)
                {
                    chain.Add(learned[i].ToArray());
                }
                IEnumerable<string> raw = chain.Chain(new Random());
                string sentince = string.Join(" ", raw);
                await ctx.Channel.SendMessageAsync(sentince);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("I'm not learned any symbols here.");
            }
        }
    }
}
