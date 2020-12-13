using HaRepacker;
using Kaede.Lib;
using MapleLib.WzLib;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kaede.Blazor.Pages {
    public partial class IdSearch : ComponentBase {
        [Inject]
        private HttpClient HttpClient { get; set; }
        private MonsterBook monsterBook;
        private string searchWord;
        private IEnumerable<string> result;

        protected override async Task OnInitializedAsync() {
            using Stream csvStream = await HttpClient.GetStreamAsync("/Resources/MonsterIDList.csv");
            var table = CSVReader.ReadCSV(csvStream);
            monsterBook = new MonsterBook(table);
        }

        private void Search() {
            if(searchWord != "") {
                result = monsterBook.GetNamesFromVagueName(searchWord);
            }
        }
    }
}
