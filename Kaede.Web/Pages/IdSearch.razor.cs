using HaRepacker;
using Kaede.Lib;
using Kaede.Web.Shared;
using MapleLib.WzLib;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kaede.Web.Pages {
    public partial class IdSearch : ComponentBase {
        private string name = "";
        private IEnumerable<string> id;

        private void Search() {
            if(name != "") {
                id = KaedeProcess.GetNamesFromVagueName(name);
            }
        }

        private ValueTask CopyToClipBoard(string text) {
            return IJSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}
