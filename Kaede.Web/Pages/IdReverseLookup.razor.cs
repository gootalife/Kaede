using Microsoft.AspNetCore.Components;
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
                name = monsterBook.GetNameFromId(id) ?? null;
            }
        }
    }
}
