using CMK;
using Kaede.Lib.Extensions;
using Kaede.Lib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Point = Kaede.Lib.Models.Point;

namespace Kaede.Lib {
    public class APNGBuilder {
        private readonly IEnumerable<AnimationFrame> animation;
        private readonly AnimationInfo animationInfo;

        public APNGBuilder(IEnumerable<AnimationFrame> animation, AnimationInfo animationInfo) {
            this.animation = animation;
            this.animationInfo = animationInfo;
        }

        /// <summary>
        /// 指定パス以下にAPNGを生成
        /// </summary>
        /// <param name="savePath">保存先パス(英数字のみ)</param>
        public void BuildAnimation(string savePath) {
            try {
                // アニメーション情報を出力する
                var json = JsonConvert.SerializeObject(animationInfo, Formatting.Indented);
                File.WriteAllText($@"{savePath}\{animationInfo.animationName}.json", json);
                // APNGを生成
                var config = new AnimatedPngCreator.Config {
                    FilterUnchangedPixels = false
                };
                using var stream = File.Create($@"{savePath}\{animationInfo.animationName}.png");
                using var apngCreator = new AnimatedPngCreator(stream, animationInfo.imageSize.x, animationInfo.imageSize.y, config);
                foreach(var frame in animation) {
                    apngCreator.WriteFrame(frame.Bitmap, (short)frame.Delay);
                }
            } catch {
                throw;
            }
        }
    }
}
