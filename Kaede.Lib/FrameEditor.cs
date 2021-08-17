using Kaede.Lib.Extensions;
using Kaede.Lib.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
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
        private AnimationInfo CalcImageSize() {
            var origin = new Point();
            int leftX = 0, rightX = 0, topY = 0, bottomY = 0;
            // 画像サイズ(余白有)の計算
            foreach(var frame in frames) {
                // leftX
                if(leftX < frame.Origin.x) {
                    leftX = frame.Origin.x;
                    origin.x = frame.Origin.x;
                }
                // rightX
                if(rightX < frame.Bitmap.Width - frame.Origin.x) {
                    rightX = frame.Bitmap.Width - frame.Origin.x;
                }
                // topY
                if(topY < frame.Origin.y) {
                    topY = frame.Origin.y;
                    origin.y = frame.Origin.y;
                }
                // bottomY
                if(bottomY < frame.Bitmap.Height - frame.Origin.y) {
                    bottomY = frame.Bitmap.Height - frame.Origin.y;
                }
            }
            return new AnimationInfo(animationName, new Point(leftX + rightX, topY + bottomY), origin);
        }

        /// <summary>
        /// サイズが揃ったPNG画像リストの生成
        /// </summary>
        /// <param name="magnification">画像サイズの倍率</param>
        /// <returns>アニメーションのリストとアニメーション情報</returns>
        public (IEnumerable<AnimationFrame> frames, AnimationInfo animInfo) EditPNGImages(byte magnification) {
            var result = new List<AnimationFrame>();
            var info = CalcImageSize();
            var newSize = new Point(info.imageSize.x * magnification, info.imageSize.y * magnification);
            var newOrigin = new Point(info.origin.x * magnification, info.origin.y * magnification);
            using var baseImage = new Bitmap(info.imageSize.x * magnification, info.imageSize.y * magnification, PixelFormat.Format32bppArgb);
            foreach(var frame in frames) {
                var newImage = new Bitmap(baseImage);
                var graphics = Graphics.FromImage(newImage);
                var enlargedSize = new Point(frame.Bitmap.Width * magnification, frame.Bitmap.Height * magnification);
                var enlarged = new Bitmap(enlargedSize.x, enlargedSize.y);
                var gResize = Graphics.FromImage(enlarged);
                gResize.InterpolationMode = InterpolationMode.NearestNeighbor;
                gResize.DrawImage(frame.Bitmap, 0, 0, enlarged.Width, enlarged.Height);
                graphics.DrawImage(enlarged, newOrigin.x - frame.Origin.x * magnification, newOrigin.y - frame.Origin.y * magnification);
                var animationFrame = new AnimationFrame(newImage, frame.AnimationName, frame.Name, new Point(enlarged.Width, enlarged.Height), frame.Delay);
                result.Add(animationFrame);
            }
            return (result, new AnimationInfo(info.animationName, newSize, newOrigin));
        }
    }
}
