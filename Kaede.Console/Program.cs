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
                string wzName = args[0];
                string csvName = args[1];
                string id = args[2];
                string resourcesPath = $@"{Directory.GetCurrentDirectory()}\Resources";
                System.Console.WriteLine($"-- Kaede process start. --");
                System.Console.Write("Init: ");
                KaedeProcess kaedeProcess = new KaedeProcess(resourcesPath, wzName, csvName);
                System.Console.WriteLine("Done.");
                System.Console.Write("Extracting WzImage: ");
                WzImage wzImage = kaedeProcess.GetWzImageFromId(id);
                if(wzImage == null) {
                    System.Console.WriteLine($"{id} is not exists or elements are nothing");
                    return;
                }
                System.Console.WriteLine("Done.");

                // APNGの出力
                System.Console.WriteLine("--- APNG build start ---");
                var animationPaths = kaedeProcess.GetAnimationPaths(wzImage.WzProperties.OrEmptyIfNull());
                var saveRoot = $@"{Directory.GetCurrentDirectory()}\AnimatedPNGs";
                var monsterName = kaedeProcess.GetNameFromId(id);
                var dirName = $@"{wzImage.Name}_{monsterName}";
                string savePath = $@"{saveRoot}\{dirName}";
                string tempPath = $@"{saveRoot}\temp";
                try {
                    // ディレクトリ削除
                    if(Directory.Exists(tempPath)) {
                        Directory.Delete(tempPath, true);
                    }
                    if(Directory.Exists(savePath)) {
                        Directory.Delete(savePath, true);
                    }
                    // アニメーション出力
                    System.Console.WriteLine($"Target: {wzImage.Name} {monsterName}");
                    foreach(var (path, index) in animationPaths.OrEmptyIfNull().Select((path, index) => (path, index))) {
                        System.Console.Write($@"({index + 1}/{animationPaths.Count()}) {path}: ");
                        Directory.CreateDirectory($@"{tempPath}\{path}");
                        var (animationPath, animatoion) = kaedeProcess.GetAnimationFromPath(id, path);
                        kaedeProcess.BuildAPNG(animationPath, animatoion, $@"{tempPath}\{path}");
                        System.Console.WriteLine("Done.");
                    }
                    // 保存先にリネーム
                    Directory.Move(tempPath, savePath);
                    System.Console.WriteLine("APNG build: Done.");
                } catch(Exception e) {
                    throw e;
                } finally {
                    // 一時ディレクトリ削除
                    if(Directory.Exists(tempPath)) {
                        Directory.Delete(tempPath, true);
                    }
                }
                System.Console.WriteLine("-- Kaede process ended. --");
            } catch(Exception e) {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine("-- Kaede process abended. --");
            }
        }
    }
}
