using Kaede.Lib;
using SharpApng;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Lib {
    public class APNGBuilder {
        private Dictionary<string, IEnumerable<AnimationFrame>> materials;

        public APNGBuilder(Dictionary<string, IEnumerable<AnimationFrame>> materials) {
            this.materials = materials;
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
            // ディレクトリ作成
            try {
                if(!Directory.Exists(savePath)) {
                    Directory.CreateDirectory(savePath);
                }
                if(!Directory.Exists(tempPath)) {
                    Directory.CreateDirectory(tempPath);
                }
                // すべてのアニメーションのAPNGを生成
                materials.Keys.ForEach(animationName => {
                    Apng apng = new Apng();
                    materials[animationName].ForEach(frame => {
                        apng.AddFrame(new Frame(frame.Bitmap, frame.Delay, delayBase));
                    });
                    apng.WriteApng($@"{tempPath}\{animationName}.png", false, true);
                    File.Copy($@"{tempPath}\{animationName}.png", $@"{savePath}\{animationName}.png", true);
                });
                Directory.Delete($@"{tempPath}", true);
            } catch(Exception e) {
                throw e;
            }
        }
    }
}
