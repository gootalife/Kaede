using System.Linq;

namespace Kaede.Lib.Models {
    public class AnimationInfo {
        public readonly string animationPath;
        public readonly string animationName;
        public readonly Point imageSize;

        public AnimationInfo(string animationPath, Point imageSize) {
            this.animationPath = animationPath;
            animationName = animationPath.Split('/').Last();
            this.imageSize = imageSize;
        }
    }
}
