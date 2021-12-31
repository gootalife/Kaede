using System.Linq;

namespace Kaede.Lib.Models {
    public class AnimationInfo {
        public readonly string animationPath;
        public readonly string animationName;
        public readonly Point imageSize;
        public readonly Point origin;

        public AnimationInfo(string animationPath, Point imageSize, Point origin) {
            this.animationPath = animationPath;
            this.animationName = animationPath.Split("/").Last();
            this.imageSize = imageSize;
            this.origin = origin;
        }
    }
}
