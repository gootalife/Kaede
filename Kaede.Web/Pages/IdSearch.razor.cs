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

namespace Kaede.Web.Pages {
    public partial class IdSearch : ComponentBase {
        [Inject]
        private KaedeProcess KaedeProcess { get; set; }
        private string name = "";
        private IEnumerable<string> result;

        private void Search() {
            if(name != "") {
                result = KaedeProcess.GetNamesFromVagueName(name);
            }
        }
    }
}
