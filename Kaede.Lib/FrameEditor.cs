using Kaede.Lib.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Point = Kaede.Lib.Models.Point;

namespace Kaede.Lib {
    public class FrameEditor {
        private readonly string animationName;
        private readonly IEnumerable<AnimationFrame> frames;

        public FrameEditor(string animationName, IEnumerable<AnimationFrame> frames) {
            this.animationName = animationName;
            this.frames = frames;
        }

        /// <summary>
        /// アニメーション毎の画像サイズを計算する
        /// </summary>
        /// <returns></returns>
        private (Point size, Point origin) CalcImageSize() {
            var origin = new Point();
            int leftX = 0, rightX = 0, topY = 0, bottomY = 0;
            // 画像サイズ(余白有)の計算
            foreach (var frame in frames) {
                // leftX
                if (leftX < frame.Origin.x) {
                    leftX = frame.Origin.x;
                    origin.x = frame.Origin.x;
                }
                // rightX
                if (rightX < frame.Bitmap.Width - frame.Origin.x) {
                    rightX = frame.Bitmap.Width - frame.Origin.x;
                }
                // topY
                if (topY < frame.Origin.y) {
                    topY = frame.Origin.y;
                    origin.y = frame.Origin.y;
                }
                // bottomY
                if (bottomY < frame.Bitmap.Height - frame.Origin.y) {
                    bottomY = frame.Bitmap.Height - frame.Origin.y;
                }
            }
            return (new Point(leftX + rightX, topY + bottomY), origin);
        }

        /// <summary>
        /// サイズが揃ったPNG画像リストの生成
        /// </summary>
        /// <param name="rate">画像サイズの倍率</param>
        /// <returns>アニメーションのリストとアニメーション情報</returns>
        public (IEnumerable<AnimationFrame> frames, AnimationInfo animInfo) EditPNGImages(byte rate) {
            var result = new List<AnimationFrame>();
            var (size, origin) = CalcImageSize();
            var newSize = new Point(size.x * rate, size.y * rate);
            var newOrigin = new Point(origin.x * rate, origin.y * rate);
            using var baseImage = new Bitmap(size.x * rate, size.y * rate, PixelFormat.Format32bppArgb);
            foreach (var frame in frames) {
                var newImage = new Bitmap(baseImage);
                var graphics = Graphics.FromImage(newImage);
                var enlargedSize = new Point(frame.Bitmap.Width * rate, frame.Bitmap.Height * rate);
                var enlarged = new Bitmap(enlargedSize.x, enlargedSize.y);
                var gResize = Graphics.FromImage(enlarged);
                gResize.InterpolationMode = InterpolationMode.NearestNeighbor;
                gResize.DrawImage(frame.Bitmap, 0, 0, enlarged.Width, enlarged.Height);
                graphics.DrawImage(enlarged, newOrigin.x - frame.Origin.x * rate, newOrigin.y - frame.Origin.y * rate);
                var animationFrame = new AnimationFrame(newImage, frame.AnimationName, frame.Name, new Point(enlarged.Width, enlarged.Height), frame.Delay);
                result.Add(animationFrame);
            }
            return (result, new AnimationInfo(animationName, newSize, newOrigin));
        }
    }
}
