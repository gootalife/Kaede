using Kaede.Lib.Extensions;
using Kaede.Lib.Models;
using Newtonsoft.Json;
using SharpApng;
using System;
using System.Collections.Generic;
using System.IO;

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
            int delayBase = 1000;
            try {
                // アニメーション情報を出力する
                var json = JsonConvert.SerializeObject(animationInfo, Formatting.Indented);
                File.WriteAllText($@"{savePath}\{animationInfo.animationName}.json", json);
                // APNGを生成
                Apng apng = new Apng();
                foreach(var frame in animation) {
                    apng.AddFrame(new Frame(frame.Bitmap, frame.Delay, delayBase));
                }
                apng.WriteApng($@"{savePath}\{animationInfo.animationName}.png", false, true);
            } catch(Exception e) {
                throw e;
            }
        }
    }
}
