using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Kaede.Lib {
    public class FrameEditor {
        private Dictionary<string, IEnumerable<AnimationFrame>> materials;
        private Point imageSize;
        private Point origin;
        public FrameEditor(Dictionary<string, IEnumerable<AnimationFrame>> materials) {
            imageSize = new Point();
            origin = new Point();
            this.materials = materials;
        }

        /// <summary>
        /// 画像サイズの計算
        /// </summary>
        private void CalcImageSize() {
            int leftX = 0, rightX = 0, topY = 0, bottomY = 0;
            // 画像サイズ(余白有)の計算
            foreach(var key in materials.Keys) {
                foreach(var frame in materials[key]) {
                    // leftX
                    if(leftX < frame.Origin.X) {
                        leftX = frame.Origin.X;
                        origin.X = frame.Origin.X;
                    }
                    // rightX
                    if(rightX < frame.Bitmap.Width - frame.Origin.X) {
                        rightX = frame.Bitmap.Width - frame.Origin.X;
                    }
                    // topY
                    if(topY < frame.Origin.Y) {
                        topY = frame.Origin.Y;
                        origin.Y = frame.Origin.Y;
                    }
                    // bottomY
                    if(bottomY < frame.Bitmap.Height - frame.Origin.Y) {
                        bottomY = frame.Bitmap.Height - frame.Origin.Y;
                    }
                }
            }
            imageSize = new Point(leftX + rightX, topY + bottomY);
        }

        /// <summary>
        /// サイズが揃ったPNG画像の生成
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="dirName"></param>
        public Dictionary<string, IEnumerable<AnimationFrame>> EditPNGImages() {
            CalcImageSize();
            string currentDir = Directory.GetCurrentDirectory();
            Dictionary<string, IEnumerable<AnimationFrame>> result = new Dictionary<string, IEnumerable<AnimationFrame>>();
            using(var baseImage = new Bitmap(imageSize.X, imageSize.Y, PixelFormat.Format32bppArgb)) {
                materials.Keys.ForEach(animationName => {
                    List<AnimationFrame> animationFrames = new List<AnimationFrame>();
                    materials[animationName].ForEach(frame => {
                        var newImage = new Bitmap(baseImage);
                        var graphics = Graphics.FromImage(newImage);
                        graphics.DrawImage(frame.Bitmap, origin.X - frame.Origin.X, origin.Y - frame.Origin.Y);
                        AnimationFrame animationFrame = new AnimationFrame(newImage, frame.AnimationName, frame.Name, frame.Origin, frame.Delay);
                        animationFrames.Add(animationFrame);
                    });
                    result.Add(animationName, animationFrames);
                });
            }
            return result;
        }
    }
}
