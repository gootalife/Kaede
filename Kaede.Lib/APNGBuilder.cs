using Kaede.Lib.Extensions;
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
        private Dictionary<string, IEnumerable<AnimationFrame>> animations;
        private List<AnimationInfo> animInfo;

        public APNGBuilder((Dictionary<string, IEnumerable<AnimationFrame>> animations, List<AnimationInfo> animInfo) materials) {
            this.animations = materials.animations;
            this.animInfo = materials.animInfo;
        }

        /// <summary>
        /// APNG生成
        /// </summary>
        /// <param name="saveRoot">保存先ディレクトリ(英数字のみ)</param>
        /// <param name="dirName"></param>
        public void BuildAnimations(string saveRoot, string dirName) {
            string savePath = $@"{saveRoot}\{dirName}";
            string tempPath = $@"{saveRoot}\temp";
            int delayBase = 1000;
            try {
                // ディレクトリ作成
                if(!Directory.Exists(savePath)) {
                    Directory.CreateDirectory(savePath);
                }
                if(!Directory.Exists(tempPath)) {
                    Directory.CreateDirectory(tempPath);
                }
                // アニメーション情報を出力する
                var json2 = JsonConvert.SerializeObject(animInfo, Formatting.Indented);
                File.WriteAllText($@"{savePath}\animationInfo.json", json2);
                // すべてのアニメーションのAPNGを生成
                animations.Keys.ForEach(animationName => {
                    Apng apng = new Apng();
                    animations[animationName].ForEach(frame => {
                        apng.AddFrame(new Frame(frame.Bitmap, frame.Delay, delayBase));
                    });
                    apng.WriteApng($@"{tempPath}\{animationName}.png", false, true);
                    File.Copy($@"{tempPath}\{animationName}.png", $@"{savePath}\{animationName}.png", true);
                });
                //var options = new JsonSerializerOptions {
                //    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                //    WriteIndented = true
                //};
                //var json1 = System.Text.Json.JsonSerializer.Serialize(animInfo, options);
                //File.WriteAllText($@"{tempPath}\animationInfo.json", json2);
                //File.Copy($@"{tempPath}\animationInfo.json", $@"{savePath}\animationInfo.json", true);
            } catch(Exception e) {
                throw e;
            } finally {
                Directory.Delete($@"{tempPath}", true);
            }
        }
    }
}
