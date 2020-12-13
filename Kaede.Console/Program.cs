using Kaede.Lib;
using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kaede.Console {
    public class Program {
        public static void Main(string[] args) {
            string resourcesPath = $@"{Directory.GetCurrentDirectory()}\Resources";
            string targetName = "メイプルキノコ";

            try {
                System.Console.WriteLine($"-- Kaede process start --");

                System.Console.WriteLine("Start init.");
                KaedeProcess kaedeProcess = new KaedeProcess(resourcesPath);
                System.Console.WriteLine("Done.");

                System.Console.WriteLine("Start extracting WzImage");
                WzImage wzImage = kaedeProcess.GetWzImageFromName(targetName);
                if(wzImage == null) {
                    System.Console.WriteLine($"{targetName} is not exists or elements are nothing");
                    return;
                }
                System.Console.WriteLine("Done.");

                // アニメーション名と各フレームを取得
                Dictionary<string, List<AnimationFrame>> elements = kaedeProcess.GetAnimationElements(wzImage);

                // APNGの出力
                System.Console.WriteLine("Start APNG build.");
                kaedeProcess.BuildAPNGs(elements, $@"{Directory.GetCurrentDirectory()}\AnimatedPNGs", wzImage.Name);
                System.Console.WriteLine("Done.");
                System.Console.WriteLine("-- Kaede process end --");
            } catch(Exception e) {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
