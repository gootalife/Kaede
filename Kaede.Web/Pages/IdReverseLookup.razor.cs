using Kaede.Lib;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kaede.Web.Pages {
    public partial class IdReverseLookup : ComponentBase {
        private string id = "";
        private string name;

        private void Search() {
            if(id != "") {
                name = KaedeProcess.GetNameFromId(id) ?? "";
            }
        }

        private ValueTask CopyToClipBoard(string text) {
            return IJSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}
