using Kaede.Lib.Extensions;
using Kaede.Lib.Models;
using System.Collections.Generic;
using System.Drawing;
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
        /// <returns>アニメーションのリストとアニメーション情報</returns>
        public (IEnumerable<AnimationFrame> frames, AnimationInfo animInfo) EditPNGImages() {
            var result = new List<AnimationFrame>();
            var info = CalcImageSize();
            using var baseImage = new Bitmap(info.imageSize.x, info.imageSize.y, PixelFormat.Format32bppArgb);
            foreach(var frame in frames) {
                var newImage = new Bitmap(baseImage);
                var graphics = Graphics.FromImage(newImage);
                graphics.DrawImage(frame.Bitmap, info.origin.x - frame.Origin.x, info.origin.y - frame.Origin.y);
                var animationFrame = new AnimationFrame(newImage, frame.AnimationName, frame.Name, frame.Origin, frame.Delay);
                result.Add(animationFrame);
            }
            return (result, info);
        }
    }
}
