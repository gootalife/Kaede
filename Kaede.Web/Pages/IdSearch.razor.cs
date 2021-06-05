using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kaede.Web.Pages {
    public partial class IdSearch : ComponentBase {
        private string name = "";
        private IEnumerable<string> names;
        private ElementReference nameInput;

        protected override async Task OnAfterRenderAsync(bool isFirstRender) {
            if(isFirstRender) {
                await nameInput.FocusAsync();
            }
        }

        private void Search() {
            if(name != "") {
                names = monsterBook.GetNamesFromVagueName(name) ?? null;
            }
        }
    }
}
