using Kaede.Lib;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kaede.Console {
    public class Program {
        public static async Task Main(string[] args) {
            if(args.Length < 1) {
                System.Console.WriteLine("引数1を指定してください。");
                return;
            }
            string id = args[0];
            string resourcesPath = $@"{Directory.GetCurrentDirectory()}\Resources";

            try {
                System.Console.WriteLine($"-- Kaede process start --");
                System.Console.WriteLine("Start init.");
                KaedeProcess kaedeProcess = new KaedeProcess(resourcesPath);
                System.Console.WriteLine("Done.");

                System.Console.WriteLine("Start extracting WzImage");
                WzImage wzImage = kaedeProcess.GetWzImageFromId(id);
                if(wzImage == null) {
                    System.Console.WriteLine($"{id} is not exists or elements are nothing");
                    return;
                }
                System.Console.WriteLine("Done.");

                System.Console.WriteLine("Get from path.");
                System.Console.WriteLine("Done.");

                // アニメーション名と各フレームを取得
                Dictionary<string, IEnumerable<AnimationFrame>> elements = kaedeProcess.GetAnimationFrames(wzImage);

                // APNGの出力
                System.Console.WriteLine("Start APNG build.");
                await Task.Run(() => kaedeProcess.BuildAPNGs(elements, $@"{Directory.GetCurrentDirectory()}\AnimatedPNGs", wzImage.Name));
                System.Console.WriteLine("Done.");
                System.Console.WriteLine("-- Kaede process end --");
            } catch(Exception e) {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
