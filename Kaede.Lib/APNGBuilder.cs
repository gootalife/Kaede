﻿using Kaede.Lib.Extensions;
using Kaede.Lib.Models;
using Newtonsoft.Json;
using SharpApng;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
//using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Kaede.Lib {
    public class APNGBuilder {
        private readonly IEnumerable<AnimationFrame> animation;
        private readonly AnimationInfo animationInfo;

        public APNGBuilder(IEnumerable<AnimationFrame> animation, AnimationInfo animationInfo) {
            this.animation = animation;
            this.animationInfo = animationInfo;
        }

        /// <summary>
        /// temp以下にAPNGを生成
        /// </summary>
        /// <param name="saveRoot">保存先ディレクトリ(英数字のみ)</param>
        /// <param name="dirName"></param>
        public void BuildAnimation(string savePath) {
            int delayBase = 1000;
            try {
                // アニメーション情報を出力する
                var json = JsonConvert.SerializeObject(animationInfo, Formatting.Indented);
                File.WriteAllText($@"{savePath}\{animationInfo.animationName}.json", json);
                // APNGを生成
                Apng apng = new Apng();
                animation.ForEach(frame => {
                    apng.AddFrame(new Frame(frame.Bitmap, frame.Delay, delayBase));
                });
                apng.WriteApng($@"{savePath}\{animationInfo.animationName}.png", false, true);
            } catch(Exception e) {
                throw e;
            }
        }
    }
}
