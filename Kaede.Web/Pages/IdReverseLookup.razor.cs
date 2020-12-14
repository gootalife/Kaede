using Kaede.Lib;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kaede.Web.Pages {
    public partial class IdReverseLookup : ComponentBase {
        [Inject]
        private HttpClient HttpClient { get; set; }
        private MonsterBook monsterBook;
        private string id;
        private string result;

        protected override async Task OnInitializedAsync() {
            using Stream csvStream = await HttpClient.GetStreamAsync("/Resources/MonsterIDList.csv");
            var table = CSVReader.ReadCSV(csvStream);
            monsterBook = new MonsterBook(table);
        }

        private void Search() {
            if(id != "") {
                result = monsterBook.GetNameFromId(id);
            }
        }
    }
}
