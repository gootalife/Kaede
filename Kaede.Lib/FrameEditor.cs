using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System;
using Kaede.Lib.Models;
using Kaede.Lib.Extensions;
using Point = Kaede.Lib.Models.Point;
using System.Linq;

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
            Point origin = new Point();
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

        ///// <summary>
        ///// サイズが揃ったPNG画像の生成
        ///// </summary>
        ///// <param name="animations"></param>
        ///// <param name="animInfo"></param>
        //public (Dictionary<string, IEnumerable<AnimationFrame>> animations, List<AnimationInfo> animInfo) EditPNGImages() {
        //    var animInfo = new List<AnimationInfo>();
        //    var result = new Dictionary<string, IEnumerable<AnimationFrame>>();
        //    materials.Keys.ForEach(animationName => {
        //        var info = CalcImageSize(animationName);
        //        animInfo.Add(info);
        //        using(var baseImage = new Bitmap(info.imageSize.x, info.imageSize.y, PixelFormat.Format32bppArgb)) {
        //            List<AnimationFrame> animationFrames = new List<AnimationFrame>();
        //            materials[animationName].ForEach(frame => {
        //                var newImage = new Bitmap(baseImage);
        //                var graphics = Graphics.FromImage(newImage);
        //                graphics.DrawImage(frame.Bitmap, info.origin.x - frame.Origin.x, info.origin.y - frame.Origin.y);
        //                AnimationFrame animationFrame = new AnimationFrame(newImage, frame.AnimationName, frame.Name, frame.Origin, frame.Delay);
        //                animationFrames.Add(animationFrame);
        //            });
        //            result.Add(animationName, animationFrames);
        //        }
        //    });
        //    return (result, animInfo);
        //}

        /// <summary>
        /// サイズが揃ったPNG画像リストの生成
        /// </summary>
        /// <param name="frames"></param>
        /// <param name="animInfo"></param>
        public (IEnumerable<AnimationFrame> frames, AnimationInfo animInfo) EditPNGImages() {
            List<AnimationFrame> result = new List<AnimationFrame>();
            var info = CalcImageSize();
            using(var baseImage = new Bitmap(info.imageSize.x, info.imageSize.y, PixelFormat.Format32bppArgb)) {
                frames.ForEach(frame => {
                    var newImage = new Bitmap(baseImage);
                    var graphics = Graphics.FromImage(newImage);
                    graphics.DrawImage(frame.Bitmap, info.origin.x - frame.Origin.x, info.origin.y - frame.Origin.y);
                    AnimationFrame animationFrame = new AnimationFrame(newImage, frame.AnimationName, frame.Name, frame.Origin, frame.Delay);
                    result.Add(animationFrame);
                });
            }
            return (result, info);
        }
    }
}
