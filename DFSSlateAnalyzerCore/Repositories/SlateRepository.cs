//using AutoMapper;

using CsvHelper;
using CsvHelper.Configuration;
using DFSSlateAnalyzerCore.Models;
using DFSSlateAnalyzerCore.Repositories.Interfaces;
using System.Globalization;

namespace DFSSlateAnalyzerCore.Repositories
{
    public class SlateRepository : ISlateRepository
    {
        public SlateRepository()
        {

        }

        public async Task<Contest> LoadContest(int ID)
        {
            var contest = new Contest();

            List<Entry> entryList = new List<Entry>();
            List<Player> contestPlayerList = new List<Player>();

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {

            };

            using (var reader = new StreamReader("contest-standings-121707355.csv"))

            using (var csv = new CsvReader(reader, csvConfig))
            {



                await csv.ReadAsync();
                csv.ReadHeader();

                while (csv.Read())
                {

                    var entry = new Entry();
                    var player = new Player();


                    // Get first entry from csv

                    entry.Rank = int.Parse(csv.GetField<string>("Rank") ?? "0");
                    entry.Id = csv.GetField<string>("EntryId");
                    entry.Name = csv.GetField<string>("EntryName");
                    entry.TimeRemaining = csv.GetField<string>("TimeRemaining");
                    entry.Points = decimal.Parse(csv.GetField<string>("Points") ?? "0");
                    entry.Lineup = csv.GetField<string>("Lineup");



                    if (entry.Lineup != null && entry.Lineup != "")
                    { entry.EntryMembers = GetEntryMembers(entry.Lineup, entry.Id); }




                    entryList.Add(entry);



                    if (csv.GetField<string>("Player") != "")
                    {
                        player.Name = csv.GetField<string>("Player");
                        player.RosterPosition = csv.GetField<string>("Roster Position");
                        player.Drafted = csv.GetField<string>("%Drafted");
                        player.FPTS = csv.GetField<string>("FPTS");


                        contestPlayerList.Add(player);
                    }

                }

            }

            contest.Entries = entryList;
            contest.ContestPlayers = contestPlayerList;

            return contest;



            List<EntryMember> GetEntryMembers(string? lineup, string? entryId)
            {
                List<EntryMember> entryMembers = new List<EntryMember>();

                string[] words = (lineup ?? "").Split(' ');

                entryMembers.Add(new EntryMember { EntryId = entryId, LineupSlot = GetLineupSlot(words[0]), Player = words[1] + " " + words[2], Position = words[0] });
                entryMembers.Add(new EntryMember { EntryId = entryId, LineupSlot = GetLineupSlot(words[3]), Player = words[4] + " " + words[5], Position = words[3] });
                entryMembers.Add(new EntryMember { EntryId = entryId, LineupSlot = GetLineupSlot(words[6]), Player = words[7] + " " + words[8], Position = words[6] });
                entryMembers.Add(new EntryMember { EntryId = entryId, LineupSlot = GetLineupSlot(words[9]), Player = words[10] + " " + words[11], Position = words[9] });
                entryMembers.Add(new EntryMember { EntryId = entryId, LineupSlot = GetLineupSlot(words[12]), Player = words[13] + " " + words[14], Position = words[12] });
                entryMembers.Add(new EntryMember { EntryId = entryId, LineupSlot = GetLineupSlot(words[15]), Player = words[16] + " " + words[17], Position = words[15] });
                entryMembers.Add(new EntryMember { EntryId = entryId, LineupSlot = GetLineupSlot(words[18]), Player = words[19] + " " + words[20], Position = words[18] });
                entryMembers.Add(new EntryMember { EntryId = entryId, LineupSlot = GetLineupSlot(words[21]), Player = words[22] + " " + words[23], Position = words[21] });


                return entryMembers;

            }

            int GetLineupSlot(string position)
            {
                int slot = 0;

                switch (position)
                {
                    case "PG":
                        slot = 1;
                        break;
                    case "SG":
                        slot = 2;
                        break;
                    case "SF":
                        slot = 3;
                        break;
                    case "PF":
                        slot = 4;
                        break;
                    case "C":
                        slot = 5;
                        break;
                    case "G":
                        slot = 6;
                        break;
                    case "F":
                        slot = 7;
                        break;
                    case "UTIL":
                        slot = 8;
                        break;


                }


                return slot;
            }


        }
    }
}