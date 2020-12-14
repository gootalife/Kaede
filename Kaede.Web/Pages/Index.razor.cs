using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kaede.Web.Pages {
    public partial class Index : ComponentBase {
        protected override void OnInitialized() {
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine(File.Exists($@"{Directory.GetCurrentDirectory()}\Resources\MonsterIdList.csv"));
        }
    }
}
