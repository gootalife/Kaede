using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Lib.Extensions {
    public static class WzImagePropertyExtension {
        public static bool HasSubElements(this WzImageProperty wzImageProperty) {
            return wzImageProperty.GetFromPath($@"{wzImageProperty.Name}/info") != null;
        }
    }
}
