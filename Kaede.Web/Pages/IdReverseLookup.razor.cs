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
        private KaedeProcess KaedeProcess { get; set; }
        private string id = "";
        private string result;

        private void Search() {
            if(id != "") {
                result = KaedeProcess.GetNameFromId(id) ?? "";
            }
        }
    }
}
