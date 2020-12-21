using Kaede.Lib;
using MapleLib.WzLib;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaede.Web.Pages {
    public partial class AnimationMaker : ComponentBase {
        private string id = "";
        private WzImage wzImage;

        private void GetWzImage() {
            if(id != "") {
                wzImage = KaedeProcess.GetWzImageFromId(id) ?? null;
            }
        }
    }
}
