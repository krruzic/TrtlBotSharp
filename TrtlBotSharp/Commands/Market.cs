using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using ConsoleTables;
namespace TrtlBotSharp
{
    public partial class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("price")]
        public async Task PriceAsync([Remainder]string Remainder = "")
        {
            // Get current coin price
            JObject LTCPrice_TO = Request.GET(TrtlBotSharp.marketEndpoint["TradeOgre_LTC"]);
            if (LTCPrice_TO.Count < 1)
            {
                await ReplyAsync("Failed to connect to market endpoint");
                return;
            }

            // Get current BTC price
            JObject BTCPrice_TO = Request.GET(TrtlBotSharp.marketEndpoint["TradeOgre_BTC"]);
            if (BTCPrice_TO.Count < 1)
            {
                await ReplyAsync("Failed to connect to market endpoint");
                return;
            }

            // Get current coin price
            JObject ETHPrice_B = Request.GET(TrtlBotSharp.marketEndpoint["Bilaxy_ETH"]);
            if (ETHPrice_B.Count < 1)
            {
                await ReplyAsync("Failed to connect to market endpoint");
                return;
            }

            // Get current BTC price
            JObject BTCPrice_B = Request.GET(TrtlBotSharp.marketEndpoint["Bilaxy_BTC"]);
            if (BTCPrice_B.Count < 1)
            {
                await ReplyAsync("Failed to connect to market endpoint");
                return;
            }

            // Begin building a response

            var to_table = new ConsoleTable(new ConsoleTableOptions
				    {
				     Columns = new [] {"---", "BTC/TRTL", "LTC/TRTL"},
				     EnableCount = false
				    });
	    
            to_table.AddRow("Bid", BTCPrice_TO["bid"], LTCPrice_TO["bid"])
                    .AddRow("Ask", BTCPrice_TO["ask"], LTCPrice_TO["ask"])
                    .AddRow("Last", BTCPrice_TO["price"], LTCPrice_TO["price"])
                    .AddRow("Volume", BTCPrice_TO["volume"], LTCPrice_TO["volume"]);

            var table_bilaxy = new ConsoleTable(new ConsoleTableOptions
				    {
				     Columns = new [] {"---", "BTC/TRTL", "ETH/TRTL"},
				     EnableCount = false
				    });
            table_bilaxy.AddRow("Bid", BTCPrice_B["data"]["buy"], ETHPrice_B["data"]["buy"])
                        .AddRow("Ask", BTCPrice_B["data"]["sell"], ETHPrice_B["data"]["sell"])
                        .AddRow("Last", BTCPrice_B["data"]["last"], ETHPrice_B["data"]["last"])
                        .AddRow("Volume", BTCPrice_B["data"]["vol"], ETHPrice_B["data"]["vol"]);

            // Send reply
            var Response = String.Format(
			    "TRTL Price:\n" +
			    String.Format("TradeOgre Market Data: ```{0}```\n", to_table) + 
			    String.Format("Bilaxy Market Data: ```{0}```", table_bilaxy));
            if (Context.Guild != null && TrtlBotSharp.marketDisallowedServers.Contains(Context.Guild.Id))
            {
                try { await Context.Message.DeleteAsync(); }
                catch { }
                await Context.Message.Author.SendMessageAsync(Response);
            }
            else await ReplyAsync(Response);
        }

        [Command("mcap")]
        public async Task MarketCapAsync([Remainder]string Remainder = "")
        {
            // Get current coin price
            JObject CoinPrice = Request.GET(TrtlBotSharp.marketEndpoint["TradeOgre_BTC"]);
            if (CoinPrice.Count < 1)
            {
                await ReplyAsync("Failed to connect to " + TrtlBotSharp.marketSource);
                return;
            }

            // Get current BTC price
            JObject BTCPrice = Request.GET(TrtlBotSharp.marketBTCEndpoint);
            if (BTCPrice.Count < 1)
            {
                await ReplyAsync("Failed to connect to " + TrtlBotSharp.marketBTCEndpoint);
                return;
            }

            // Begin building a response
            string Response = string.Format("{0}'s market cap is **{1:c}** USD", TrtlBotSharp.coinName,
                (decimal)CoinPrice["price"] * (decimal)BTCPrice["data"]["quotes"]["USD"]["price"] * TrtlBotSharp.GetSupply());

            // Send reply
            if (Context.Guild != null && TrtlBotSharp.marketDisallowedServers.Contains(Context.Guild.Id))
            {
                try { await Context.Message.DeleteAsync(); }
                catch { }
                await Context.Message.Author.SendMessageAsync(Response);
            }
            else await ReplyAsync(Response);
        }
    }
}
