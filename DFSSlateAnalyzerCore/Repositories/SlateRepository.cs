//using AutoMapper;

using CsvHelper;
using CsvHelper.Configuration;
using DFSSlateAnalyzerCore.Models;
using DFSSlateAnalyzerCore.Repositories.Interfaces;
using DFSSlateAnalyzerData;
using DFSSlateAnalyzerData.Data;
using System.Data.Entity;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace DFSSlateAnalyzerCore.Repositories
{
    public class SlateRepository : ISlateRepository
    {

        protected readonly DFSSlateAnalyzerContext _db ;

        public SlateRepository(DFSSlateAnalyzerContext db)
        {
            _db = db;

        }


        public async Task<ContestModel> GetContest(Int64 ID)
        {
            var contest = new Contest();
            ID = 121707355;

            contest = _db.Contests.SingleOrDefault(x => x.ContestID == ID);

            if (contest == null)
            {

                contest = await UploadContest(ID);
            }

            var contestModel =
                new ContestModel( contest.ContestID, contest.Name ?? "blah", contest.PercentComplete, contest.Size, contest.Fee);

           List<EntryModel> entryList = new List<EntryModel>(); 

            foreach (var entry in contestModel.Entries)
            {

                entryList.Add(new EntryModel());
            }

           List<PlayerModel> contestPlayerList = new List<PlayerModel>(); 

            foreach (var player in contestModel.ContestPlayers) { 
            
            
                contestPlayerList.Add(player);
            }



            return contestModel;

        }


            public async Task<Contest> UploadContest(Int64 ID)
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
                    entry.EntryID = csv.GetField<Int64>("EntryId");
                    entry.Name = csv.GetField<string>("EntryName");
                    entry.TimeRemaining = csv.GetField<string>("TimeRemaining");
                    entry.Points = decimal.Parse(csv.GetField<string>("Points") ?? "0");
                    entry.Lineup = csv.GetField<string>("Lineup");



                    if (entry.Lineup != null && entry.Lineup != "")
                    { entry.EntryMembers = GetEntryMembers(entry.Lineup, entry.EntryID); }




                    entryList.Add(entry);



                    if (csv.GetField<string>("Player") != "")
                    {
                        player.PlayerName = csv.GetField<string>("Player");
                        player.RosterPosition = csv.GetField<string>("Roster Position");
                        player.Drafted = csv.GetField<string>("%Drafted");
                        player.FPTS = csv.GetField<string>("FPTS");


                        contestPlayerList.Add(player);
                    }

                }

            }

            contest.ContestID = 121707355;
            contest.Entries = entryList;
            contest.ContestPlayers = contestPlayerList;

            SaveContestToDatabase(contest);

            return contest;



            List<EntryMember> GetEntryMembers(string? lineup, Int64 entryId)
            {
                List<EntryMember> entryMembers = new List<EntryMember>();

                string[] words = (lineup ?? "").Split(' ');

                entryMembers.Add(new EntryMember { EntryID = entryId, LineupSlot = GetLineupSlot(words[0]), EntryMemberPlayerName = words[1] + " " + words[2], Position = words[0] });
                entryMembers.Add(new EntryMember { EntryID = entryId, LineupSlot = GetLineupSlot(words[3]), EntryMemberPlayerName = words[4] + " " + words[5], Position = words[3] });
                entryMembers.Add(new EntryMember { EntryID = entryId, LineupSlot = GetLineupSlot(words[6]), EntryMemberPlayerName = words[7] + " " + words[8], Position = words[6] });
                entryMembers.Add(new EntryMember { EntryID = entryId, LineupSlot = GetLineupSlot(words[9]), EntryMemberPlayerName = words[10] + " " + words[11], Position = words[9] });
                entryMembers.Add(new EntryMember { EntryID = entryId, LineupSlot = GetLineupSlot(words[12]), EntryMemberPlayerName = words[13] + " " + words[14], Position = words[12] });
                entryMembers.Add(new EntryMember { EntryID = entryId, LineupSlot = GetLineupSlot(words[15]), EntryMemberPlayerName = words[16] + " " + words[17], Position = words[15] });
                entryMembers.Add(new EntryMember { EntryID = entryId, LineupSlot = GetLineupSlot(words[18]), EntryMemberPlayerName = words[19] + " " + words[20], Position = words[18] });
                entryMembers.Add(new EntryMember { EntryID = entryId, LineupSlot = GetLineupSlot(words[21]), EntryMemberPlayerName = words[22] + " " + words[23], Position = words[21] });


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

        public void SaveContestToDatabase(Contest contest)
        {
           
          



            _db?.Contests.Add(contest);
             _db?.SaveChanges();
        }
    }
}