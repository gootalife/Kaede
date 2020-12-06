using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace KaedeConsole {
    public class FrameEditor {
        private Dictionary<string, List<AnimationFrame>> materials;
        private Point imageSize;
        private Point origin;
        public FrameEditor(Dictionary<string, List<AnimationFrame>> materials) {
            imageSize = new Point();
            origin = new Point();
            this.materials = materials;
            CalcImageSize();
        }

        /// <summary>
        /// 画像サイズの計算
        /// </summary>
        private void CalcImageSize() {
            int minX = 0, maxX = 0, minY = 0, maxY = 0;
            // 画像サイズ(余白有)の計算
            foreach(var key in materials.Keys) {
                foreach(var frame in materials[key]) {
                    // minX
                    if(minX < frame.Origin.X) {
                        minX = frame.Origin.X;
                        origin.X = frame.Origin.X;
                    }
                    // maxX
                    if(maxX < frame.Bitmap.Width - frame.Origin.X) {
                        maxX = frame.Bitmap.Width - frame.Origin.X;
                        origin.X = frame.Origin.X;
                    }
                    // minY
                    if(minY < frame.Origin.Y) {
                        minY = frame.Origin.Y;
                        origin.Y = frame.Origin.Y;
                    }
                    // maxY
                    if(maxY < frame.Bitmap.Height - frame.Origin.Y) {
                        maxY = frame.Bitmap.Height - frame.Origin.Y;
                        origin.Y = frame.Origin.Y;
                    }
                }
            }
            imageSize = new Point(minX + maxX, minY + maxY);
        }

        /// <summary>
        /// サイズが揃ったPNG画像の生成
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="dirName"></param>
        public Dictionary<string, List<AnimationFrame>> EditPNGImages() {
            string currentDir = Directory.GetCurrentDirectory();
            Dictionary<string, List<AnimationFrame>> results = new Dictionary<string, List<AnimationFrame>>();

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
                    results.Add(animationName, animationFrames);
                });
            }
            return results;
        }
    }
}
