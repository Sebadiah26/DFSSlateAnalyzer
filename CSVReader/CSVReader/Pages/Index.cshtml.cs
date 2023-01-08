using CsvHelper;
using CsvHelper.Configuration;
using CSVReader.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace CSVReader.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;



        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()

        {


            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {

            };

            using (var reader = new StreamReader("contest-standings-121707355.csv"))

            using (var csv = new CsvReader(reader, csvConfig))
            {

                List<Entry> entryList = new List<Entry>();
                List<ContestPlayer> contestPlayerList = new List<ContestPlayer>();

                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {

                    var entry = new Entry();
                    var contestPlayer = new ContestPlayer();




                    entry.Rank = int.Parse(csv.GetField<string>("Rank"));
                    entry.EntryId = csv.GetField<string>("EntryId");
                    entry.EntryName = csv.GetField<string>("EntryName");
                    entry.TimeRemaining = csv.GetField<string>("TimeRemaining");
                    entry.Points = csv.GetField<string>("Points");
                    entry.Lineup = csv.GetField<string>("Lineup");

                    if (entry.Lineup != null && entry.Lineup != "")
                    { entry.EntryMembers = GetEntryMembers(entry.Lineup, entry.EntryId); }




                    entryList.Add(entry);

                    if (csv.GetField<string>("Player") != "")
                    {
                        contestPlayer.Player = csv.GetField<string>("Player");
                        contestPlayer.RosterPosition = csv.GetField<string>("Roster Position");
                        contestPlayer.Drafted = csv.GetField<string>("%Drafted");
                        contestPlayer.FPTS = csv.GetField<string>("FPTS");


                        contestPlayerList.Add(contestPlayer);
                    }

                }

                //while (csv.Read())
                //     {

                //        var rank = csv.GetField(0);
                //        var entryId = csv.GetField(1);
                //        var entryName = csv.GetField(2);
                //        var timeRemaining = csv.GetField(3);
                //        var points = csv.GetField(4);
                //        var lineup = csv.GetField(5);


                //        var player = csv.GetField(7);
                //        var rosterPosition = csv.GetField(8);
                //        var percentDrafted = csv.GetField(9);
                //        var fantasyPoints = csv.GetField(10);


                //    }  






                //foreach (var entry in entries)
                //{

                //}

                Console.WriteLine(entryList.Count().ToString() + " entries");
                Console.WriteLine(contestPlayerList.Count().ToString() + " players");
            }

            List<EntryMember> GetEntryMembers(string lineup, string entryId)
            {
                List<EntryMember> entryMembers = new List<EntryMember>();

                string[] words = lineup.Split(' ');

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