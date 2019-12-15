using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            // Check for bad results
            if (string.IsNullOrEmpty(system.name))
            {
                await Context.Channel.SendMessageAsync("`System " + msg + " was not found!`");
                return;
            }


            var conFacTitle = new EmbedFieldBuilder()
                .WithName("Controlling Faction:")
                .WithValue(system.controllingFaction.name)
                .WithIsInline(false);

            var embed = new EmbedBuilder()
                .WithTitle("Situational Report for " + system.name)
                .WithColor(255, 0, 0)
                .WithDescription("Url: " + system.url)
                .WithFields(conFacTitle);

            int total = 0;
            foreach (var fNum in system.factions)
            {
                if (fNum.influence > 0.00)
                {
                    var factionName = new EmbedFieldBuilder()
                        .WithName("Faction:")
                        .WithValue(fNum.name)
                        .WithIsInline(true);

                    var factionInf = new EmbedFieldBuilder()
                        .WithName("Influence:")
                        .WithValue((100 * fNum.influence).ToString("0.#\\%"))
                        .WithIsInline(true);

                    var factionState = new EmbedFieldBuilder()
                        .WithName("State:")
                        .WithValue(fNum.state)
                        .WithIsInline(true);

                    embed.WithFields(factionName, factionInf, factionState);

                    total++;
                }
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

            if (string.IsNullOrEmpty(traffic.name))
            {
                await Context.Channel.SendMessageAsync("`System " + msg + " was not found!`");
                return;
            }

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

            JArray sphereObj;

            try
            {
                sphereObj = JArray.Parse(json);
            }
            catch (Exception e)
            {
                Console.WriteLine("System " + msg + " was not found!");
                return;
            }
           

            List<SphereData> systems = new List<SphereData>();

            foreach (var result in sphereObj)
            {
                SphereData sphereData = result.ToObject<SphereData>();
                systems.Add(sphereData);
            }

            // Check to ensure a faction has already been set
            if (string.IsNullOrEmpty(Config.Bot.Faction))
            {
                await Context.Channel.SendMessageAsync(
                    "`No faction is set! Contact the bot maintainer to configure this bot for a faction.`");
            }

            await Context.Channel.SendMessageAsync(
                "`This can take some time; please be patient!`");

            // Sort by distance
            var sorted =
                from system in systems
                orderby system.distance
                select system;

            foreach (var s in sorted)
            {
                // Grab system data
                var systemJson = new GetInfo().GetSystemInfo(s.name).Result;
                SystemData system;

                // Check for bad results
                try
                {
                    system = JsonConvert.DeserializeObject<SystemData>(systemJson);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                //Check the the number of factions and if the configured faction occupies the system
                int total = 0;
                int occupiedAlready = 0;
//                Console.WriteLine("System Name: " + s.name);
//                Console.WriteLine("Distance: " + s.distance);
//                Console.WriteLine("Factions:");
                foreach (var f in system.factions)
                {
//                    Console.WriteLine(f.name);
                    if (f.influence > 0.00)
                    {
                        total++;
                    }

                    if (f.name == Config.Bot.Faction)
                    {
//                        Console.WriteLine("Occupied by " + Config.Bot.Faction);
                        occupiedAlready = 1;
                    }
                }

//                Console.WriteLine("Total Factions: " + total);
//                Console.WriteLine("-------");
                if (total < 7 && total > 0 && occupiedAlready != 1 
                    & s.name != "Sol" & s.name != "Sirius") // Some special systems don't follow the rules.
                {
                    await Context.Channel.SendMessageAsync("`" + s.name.ToUpper() + " is the most likely expansion candidate for " + msg.ToUpper() + ".`");
                    await Context.Channel.SendMessageAsync("`Distance: " + s.distance + "ly`");
                    await Sitrep(s.name);
                    return; // Since we're sorted by distance, stop at the first hit.
                }

            }
            // This will only trigger if no systems are found that are available for expansion.
            await Context.Channel.SendMessageAsync("`No expansion candidate found for " + msg.ToUpper() + ".`");

        }

        public class SystemData
        {
            [JsonProperty("id")]
            public int id { get; set; }

            [JsonProperty("id64")]
            public long id64 { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("url")]
            public string url { get; set; }

            [JsonProperty("controllingFaction")]
            public ControllingFactionData controllingFaction { get; set; }

            [JsonProperty("factions")]
            public List<FactionData> factions { get; set; }

            [JsonProperty("traffic")]
            public TrafficData traffic { get; set; }
        }

        public class ControllingFactionData
        {
            [JsonProperty("id")]
            public int id { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("allegiance")]
            public string allegiance { get; set; }

            [JsonProperty("government")]
            public string government { get; set; }
        }

        public class PredictiveState
        {
            [JsonProperty("state")]
            public string state { get; set; }

            [JsonProperty("trend")]
            public double trend { get; set; }
        }

        public class FactionData
        {
            [JsonProperty("id")]
            public int id { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("allegiance")]
            public string allegiance { get; set; }

            [JsonProperty("government")]
            public string government { get; set; }

            [JsonProperty("influence")]
            public double influence { get; set; }

            [JsonProperty("state")]
            public string state { get; set; }

            [JsonProperty("states")]
            public List<PredictiveState> states { get; set; }

            [JsonProperty("recoveringStates")]
            public List<PredictiveState> recoveringStates { get; set; }

            [JsonProperty("pendingStates")]
            public List<PredictiveState> pendingStates { get; set; }

            [JsonProperty("happiness")]
            public string happiness { get; set; }

            [JsonProperty("isplayer")]
            public bool isplayer { get; set; }

            [JsonProperty("lastupdate")]
            public int lastupdate { get; set; }
        }

        public class SystemTraffic
        {
            [JsonProperty("id")]
            public int id { get; set; }

            [JsonProperty("id64")]
            public long id64 { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("url")]
            public string url { get; set; }

            [JsonProperty("discovery")]
            public DiscoveryData discovery { get; set; }

            [JsonProperty("traffic")]
            public TrafficData traffic { get; set; }
        }

        public class DiscoveryData
        {
            [JsonProperty("commander")]
            public string commander { get; set; }

            [JsonProperty("date")]
            public DateTime date { get; set; }
        }

        public class TrafficData
        {
            [JsonProperty("total")]
            public int total { get; set; }

            [JsonProperty("week")]
            public int week { get; set; }

            [JsonProperty("day")]
            public int day { get; set; }
        }

//        public class SphereResults
//        {
//            public List<SphereData> sphereDataList { set; get; }
//        }

        public class SphereData
        {
            [JsonProperty("distance")]
            public double distance { get; set; }

            [JsonProperty("bodyCount")]
            public int bodyCount { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }
    }
}

