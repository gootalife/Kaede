using Kaede.Lib;
using Kaede.Lib.Extensions;
using MapleLib.WzLib;
using System;
using System.IO;
using System.Linq;

namespace Kaede.Console {
    public class Program {
        public static void Main(string[] args) {
            if(args.Length < 3) {
                System.Console.WriteLine("引数を3つ指定してください。(リソースwzファイル名, モンスターIDリストCSV名, モンスターID)");
                return;
            }
            try {
                var wzName = args[0];
                var csvName = args[1];
                var id = args[2];
                var resourcesPath = $@"{Directory.GetCurrentDirectory()}\Resources";
                System.Console.WriteLine($"--- Kaede process start. ---");
                System.Console.Write("Init: ");
                var kaedeProcess = new KaedeProcess(resourcesPath, wzName, csvName);
                System.Console.WriteLine("Done.");
                System.Console.Write("Extracting WzImage: ");
                var wzImage = kaedeProcess.GetWzImageFromId(id);
                if(wzImage is null) {
                    System.Console.WriteLine($"{id} is not exists or elements are nothing");
                    return;
                }
                System.Console.WriteLine("Done.");

                // APNGの出力
                System.Console.WriteLine("APNG build start.");
                var animationPaths = kaedeProcess.GetAnimationPaths(wzImage.WzProperties.OrEmptyIfNull());
                var saveRoot = $@"{Directory.GetCurrentDirectory()}\AnimatedPNGs";
                var monsterName = kaedeProcess.GetNameFromId(id);
                var dirName = $@"{wzImage.Name}_{monsterName}";
                var savePath = $@"{saveRoot}\{dirName}";
                try {
                    // アニメーション出力
                    System.Console.WriteLine($"Target: {wzImage.Name} {monsterName}");
                    foreach(var (path, index) in animationPaths.OrEmptyIfNull().Select((path, index) => (path, index))) {
                        System.Console.Write($@"({index + 1}/{animationPaths.Count()}) {path}: ");
                        Directory.CreateDirectory($@"{savePath}\{path}");
                        var (animationPath, animatoion) = kaedeProcess.GetAnimationFromPath(id, path);
                        kaedeProcess.BuildAPNG(animationPath, animatoion, $@"{savePath}\{path}");
                        System.Console.WriteLine("Done.");
                    }
                    System.Console.WriteLine("APNG build: Done.");
                } catch {
                    throw;
                }
                System.Console.WriteLine("--- Kaede process ended. ---");
            } catch(Exception e) {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine("--- Kaede process abended. ---");
            }
        }
    }
}
