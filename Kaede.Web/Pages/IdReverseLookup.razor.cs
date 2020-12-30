using Kaede.Lib;
using Kaede.Web.Shared;
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
        private ElementReference idInput;

        protected override async Task OnAfterRenderAsync(bool isFirstRender) {
            if(isFirstRender) {
                await idInput.FocusAsync();
            }
        }

        private void Search() {
            if(id != "") {
                name = KaedeProcess.GetNameFromId(id) ?? null;
            }
        }
    }
}
