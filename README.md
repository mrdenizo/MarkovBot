# MarkovBot
Simple Markov chain discord bot.

### what it does?
it learns your discord messages and uses Markov chain to generate new sentences.

### how to use it?
1. install dotnet 6.0 or higher [here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2. download latest release [here](https://github.com/mrdenizo/MarkovBot/releases/latest)
3. start executable file, it will create config.
4. close executable and write your bot token in confg:
```json
{"AutoRead":{},"Token":"write your token here"}
```
5. start executable again.
### comands
comand prefix is bot ping
- learn \<int> - learns \<int> messages.
- learnfrom \<int> \<int> - skips \<int> messages then learns \<int> messages
- autolearn \<bool> - turns on or off autolearn new messages
- generate - generates new message using markov chain.
