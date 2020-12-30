using Kaede.Lib;
using Kaede.Web.Shared;
using MapleLib.WzLib;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kaede.Web.Pages {
    public partial class AnimationExporter : ComponentBase {
        private string id = "";
        private WzImage wzImage;
        private ElementReference idInput;

        protected override async Task OnAfterRenderAsync(bool isFirstRender) {
            if(isFirstRender) {
                await idInput.FocusAsync();
            }
            await IJSRuntime.ConsoleLog($@"{Directory.GetCurrentDirectory()}\AnimatedPNGs");
        }

        private void GetWzImage() {
            if(id != "") {
                wzImage = KaedeProcess.GetWzImageFromId(id) ?? null;
            }
        }

        private async Task Export() {
            try {
                if(await IJSRuntime.Confirm("APNGをエクスポートしますか?")) {
                    await Task.Run(() => {
                        var elements = KaedeProcess.GetAnimationFrames(wzImage);
                        KaedeProcess.BuildAPNGs(elements, $@"{Directory.GetCurrentDirectory()}\AnimatedPNGs", wzImage.Name);
                    });
                    await IJSRuntime.Alert($@"出力しました。\n{Directory.GetCurrentDirectory()}\AnimatedPNGs\{wzImage.Name}");
                }
            } catch(Exception e) {
                await IJSRuntime.ConsoleLog(e.Message);
            }
        }
    }
}
