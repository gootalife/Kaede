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

namespace Kaede.Lib {
    public class FrameEditor {
        private readonly Dictionary<string, IEnumerable<AnimationFrame>> materials;
        public FrameEditor(Dictionary<string, IEnumerable<AnimationFrame>> materials) {
            this.materials = materials;
        }

        /// <summary>
        /// 画像サイズの計算
        /// </summary>
        //private void CalcImageSize() {
        //    int leftX = 0, rightX = 0, topY = 0, bottomY = 0;
        //    // 画像サイズ(余白有)の計算
        //    foreach(var key in materials.Keys) {
        //        foreach(var frame in materials[key]) {
        //            // leftX
        //            if(leftX < frame.Origin.X) {
        //                leftX = frame.Origin.X;
        //                origin.X = frame.Origin.X;
        //            }
        //            // rightX
        //            if(rightX < frame.Bitmap.Width - frame.Origin.X) {
        //                rightX = frame.Bitmap.Width - frame.Origin.X;
        //            }
        //            // topY
        //            if(topY < frame.Origin.Y) {
        //                topY = frame.Origin.Y;
        //                origin.Y = frame.Origin.Y;
        //            }
        //            // bottomY
        //            if(bottomY < frame.Bitmap.Height - frame.Origin.Y) {
        //                bottomY = frame.Bitmap.Height - frame.Origin.Y;
        //            }
        //        }
        //    }
        //    imageSize = new Point(leftX + rightX, topY + bottomY);
        //}

        /// <summary>
        /// アニメーション毎の画像サイズを計算する
        /// </summary>
        /// <param name="animationName"></param>
        /// <returns></returns>
        private AnimationInfo CalcImageSize(string animationName) {
            Point origin = new Point();
            int leftX = 0, rightX = 0, topY = 0, bottomY = 0;
            // 画像サイズ(余白有)の計算
            foreach(var frame in materials[animationName]) {
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
        /// サイズが揃ったPNG画像の生成
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="dirName"></param>
        public (Dictionary<string, IEnumerable<AnimationFrame>> animations, List<AnimationInfo> animInfo) EditPNGImages() {
            //CalcImageSize();
            var animInfo = new List<AnimationInfo>();
            string currentDir = Directory.GetCurrentDirectory();
            Dictionary<string, IEnumerable<AnimationFrame>> result = new Dictionary<string, IEnumerable<AnimationFrame>>();
            materials.Keys.ForEach(animationName => {
                var info = CalcImageSize(animationName);
                animInfo.Add(info);
                using(var baseImage = new Bitmap(info.imageSize.x, info.imageSize.y, PixelFormat.Format32bppArgb)) {
                    List<AnimationFrame> animationFrames = new List<AnimationFrame>();
                    materials[animationName].ForEach(frame => {
                        var newImage = new Bitmap(baseImage);
                        var graphics = Graphics.FromImage(newImage);
                        graphics.DrawImage(frame.Bitmap, info.origin.x - frame.Origin.x, info.origin.y - frame.Origin.y);
                        AnimationFrame animationFrame = new AnimationFrame(newImage, frame.AnimationName, frame.Name, frame.Origin, frame.Delay);
                        animationFrames.Add(animationFrame);
                    });
                    result.Add(animationName, animationFrames);
                }
            });
            return (result, animInfo);
        }
    }
}
