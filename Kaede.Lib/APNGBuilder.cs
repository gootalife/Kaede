using CMK;
using Kaede.Lib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using static CMK.AnimatedPngCreator;

namespace Kaede.Lib {
    public class APNGBuilder {
        private readonly IEnumerable<AnimationFrame> animation;
        private readonly AnimationInfo animationInfo;
        private readonly Config config = new() {
                FilterUnchangedPixels = false
            };

        public APNGBuilder(IEnumerable<AnimationFrame> animation, AnimationInfo animationInfo) {
            this.animation = animation;
            this.animationInfo = animationInfo;
        }

        /// <summary>
        /// 指定パス以下にAPNGを生成
        /// </summary>
        /// <param name="savePath">保存先パス(英数字のみ)</param>
        /// <exception cref="Exception"></exception>
        public void BuildAnimationToFile(string savePath) {
            // アニメーション情報を出力する
            var json = JsonConvert.SerializeObject(animationInfo, Formatting.Indented);
            File.WriteAllText($@"{savePath}\{animationInfo.animationName}.json", json);
            // APNGを生成
            using var stream = File.Create($@"{savePath}\{animationInfo.animationName}.png");
            using var apngCreator = new AnimatedPngCreator(stream, animationInfo.imageSize.x, animationInfo.imageSize.y, config);
            foreach (var frame in animation) {
                apngCreator.WriteFrame(frame.Bitmap, (short)frame.Delay);
            }
        }

        /// <summary>
        /// ストリームにアニメーションを書き込む
        /// </summary>
        /// <returns></returns>
        public MemoryStream BuildAnimationToStream() {
            // APNGを生成
            using var memoryStream = new MemoryStream();
            using var apngCreator = new AnimatedPngCreator(memoryStream, animationInfo.imageSize.x, animationInfo.imageSize.y, config);
            foreach (var frame in animation) {
                apngCreator.WriteFrame(frame.Bitmap, (short)frame.Delay);
            }
            var bs = new MemoryStream(memoryStream.GetBuffer());
            return bs;
        }
    }
}
