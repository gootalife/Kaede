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
        /// アニメーション全体の画像サイズを計算する
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
        /// <param name="ratio">画像サイズの倍率</param>
        /// <returns>アニメーションのリストとアニメーション情報</returns>
        public (IEnumerable<AnimationFrame> frames, AnimationInfo animInfo) EditPNGImages(byte ratio) {
            var result = new List<AnimationFrame>();
            var (size, origin) = CalcImageSize();
            var overallSize = new Point(size.x * ratio, size.y * ratio);
            var overallOrigin = new Point(origin.x * ratio, origin.y * ratio);
            // 背景(透明)
            using var baseImage = new Bitmap(size.x * ratio, size.y * ratio, PixelFormat.Format32bppArgb);
            foreach (var frame in frames) {
                // newImageにリサイズ後の画像を書き込む
                var newImage = new Bitmap(baseImage);
                var graphics = Graphics.FromImage(newImage);

                var resizedSize = new Point(frame.Bitmap.Width * ratio, frame.Bitmap.Height * ratio);
                var resizedOrigin = new Point(frame.Origin.x * ratio, frame.Origin.y * ratio);
                var resizedImage = new Bitmap(resizedSize.x, resizedSize.y);
                var gResize = Graphics.FromImage(resizedImage);
                gResize.InterpolationMode = InterpolationMode.NearestNeighbor;
                // ベース画像をリサイズ後のサイズで描画する
                gResize.DrawImage(frame.Bitmap, 0, 0, resizedImage.Width, resizedImage.Height);
                // newImageに上書きする
                graphics.DrawImage(resizedImage, overallOrigin.x - resizedOrigin.x, overallOrigin.y - resizedOrigin.y);
                var animationFrame = new AnimationFrame(newImage, frame.AnimationName, frame.Name, new Point(resizedImage.Width, resizedImage.Height), frame.Delay);
                result.Add(animationFrame);
            }
            return (result, new AnimationInfo(animationName, overallSize));
        }
    }
}
