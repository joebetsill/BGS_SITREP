using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace BGS_SitRep.Modules 
{
    public class BGS : ModuleBase<SocketCommandContext>
    {
        [Command("sitrep")]
        [Summary("Returns the situational report for a given system.")]

        public async Task Sitrep([Remainder] string msg)
        {
            var json = new GetInfo().GetSystemInfo(msg).Result;
            SystemData system = JsonConvert.DeserializeObject<SystemData>(json);

            var conFacTitle = new EmbedFieldBuilder()
                .WithName("Controlling Faction:")
                .WithValue(system.controllingFaction.name)
                .WithIsInline(false);

            var embed = new EmbedBuilder()
                .WithTitle("Situational Report for " + system.name)
                .WithColor(255, 0, 0)
                .WithDescription("Url: " + system.url)
                .WithFields(conFacTitle);

            int count = 0;
            int total = 0;
            foreach (var fNum in system.factions)
            {
                if (system.factions[count].influence > 0.00)
                {
                    var factionName = new EmbedFieldBuilder()
                        .WithName("Faction:")
                        .WithValue(system.factions[count].name)
                        .WithIsInline(true);

                    var factionInf = new EmbedFieldBuilder()
                    .WithName("Influence:")
                        .WithValue((100 * system.factions[count].influence).ToString("0.#\\%"))
                        .WithIsInline(true);

                    var factionState = new EmbedFieldBuilder()
                    .WithName("State:")
                        .WithValue(system.factions[count].state)
                        .WithIsInline(true);

                    embed.WithFields(factionName, factionInf, factionState);

                    total++;
                }
                count++;
            }

            if (total < 7)
            {
                var slotOpen = new EmbedFieldBuilder()
                    .WithName("ALERT:")
                    .WithValue("This system has a slot available for expansion!")
                    .WithIsInline(false);

                embed.WithFields(slotOpen);
            }

            await Context.Channel.SendMessageAsync(null, false, embed.Build());

        }

        [Command("traffic")]
        [Summary("Returns a traffic report for a given system.")]

        public async Task Traffic([Remainder] string msg)
        {
            var json = new GetInfo().GetTrafficInfo(msg).Result;
            SystemTraffic traffic = JsonConvert.DeserializeObject<SystemTraffic>(json);

            var dailyTrafficReport = new EmbedFieldBuilder()
                .WithName("Daily Traffic Report:")
                .WithValue(traffic.traffic.day)
                .WithIsInline(false);

            var weeklyTrafficReport = new EmbedFieldBuilder()
                .WithName("Weekly Traffic Report:")
                .WithValue(traffic.traffic.week)
                .WithIsInline(false);

            var totalTrafficReport = new EmbedFieldBuilder()
                .WithName("Total Traffic Report:")
                .WithValue(traffic.traffic.total)
                .WithIsInline(false);

            var embed = new EmbedBuilder()
                .WithTitle("Traffic Report for " + traffic.name)
                .WithColor(255, 0, 0)
                .WithDescription("Url: " + traffic.url)
                .WithFields(dailyTrafficReport, weeklyTrafficReport, totalTrafficReport);

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("exrep")]
        [Summary("Returns the most likely expansion candidate.")]

        public async Task Exrep([Remainder] string msg)
        {
            var json = new GetInfo().GetExpansionInfo(msg).Result;

            //todo: decide if this will be given a faction name or a system name (faction would be more impressive)
            //Todo: Get 20ly radius systems
            //Todo: Get the systems from that list with less than 7 slots filled
            //Todo: weed out systems from the resulting list that already have the faction, if passed a faction
        }

        [Command("setFaction")]
        [Summary("Sets your faction in the configuration file.")]

        public async Task setFaction([Remainder] string msg)
        {
            //TODO: set a faction in the configuration file for use with determining expansion candidates.
        }

        public struct SystemData
        {
            public int id;
            public long id64;
            public string name;
            public string url;
            public ControllingFactionData controllingFaction;
            public IList<FactionData> factions;
            public TrafficData traffic;
        }

        public struct ControllingFactionData
        {
            public int id;
            public string name;
            public string allegiance;
            public string government;
        }

        public struct PredictiveState
        {
            public string state;
            public double trend;
        }

        public struct FactionData
        {
            public int id;
            public string name;
            public string allegiance;
            public string government;
            public double influence;
            public string state;
            public IList<PredictiveState> states;
            public IList<PredictiveState> recoveringStates;
            public IList<PredictiveState> pendingStates;
            public string happiness;
            public bool isplayer;
            public int lastupdate;
        }

        public struct SystemTraffic
        {
            public int id;
            public long id64;
            public string name;
            public string url;
            public DiscoveryData discovery;
            public TrafficData traffic;
//            public string breakdown;

        }

        public struct DiscoveryData
        {
            public string commander;
            public DateTime date;
        }

        public struct TrafficData
        {
            public int total;
            public int week;
            public int day;
        }
    }
}
