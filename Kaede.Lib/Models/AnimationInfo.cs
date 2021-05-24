using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Lib.Models {
    public class AnimationInfo {
        public readonly string animationName;
        public readonly Point imageSize;
        public readonly Point origin;

        public AnimationInfo(string animationName, Point imageSize, Point origin) {
            this.animationName = animationName;
            this.imageSize = imageSize;
            this.origin = origin;
        }
    }
}
