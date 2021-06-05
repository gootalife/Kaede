using Kaede.Lib;
using Kaede.Lib.Extensions;
using MapleLib.WzLib;
using System;
using System.IO;
using System.Linq;

namespace Kaede.Console {
    public class Program {
        public static void Main(string[] args) {
            if(args.Length < 1) {
                System.Console.WriteLine("引数1を指定してください。");
                return;
            }
            try {
                string id = args[0];
                string resourcesPath = $@"{Directory.GetCurrentDirectory()}\Resources";

                System.Console.WriteLine($"-- Kaede process start. --");
                System.Console.Write("Init: ");
                KaedeProcess kaedeProcess = new KaedeProcess(resourcesPath);
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
                var saveRoot = $@"{Directory.GetCurrentDirectory()}\AnimatedPNGs";
                var monsterName = kaedeProcess.GetNameFromId(id);
                var dirName = $@"{wzImage.Name}_{monsterName}";
                string savePath = $@"{saveRoot}\{dirName}";
                string tempPath = $@"{saveRoot}\temp";
                try {
                    if(Directory.Exists(tempPath)) {
                        Directory.Delete(tempPath, true);
                    }
                    if(Directory.Exists(savePath)) {
                        Directory.Delete(savePath, true);
                    }
                    // メインアニメーション出力
                    var mainAnimationPaths = kaedeProcess.GetMainAnimationPaths(wzImage);
                    System.Console.WriteLine($"Target: {dirName}");
                    foreach(var path in mainAnimationPaths.OrEmptyIfNull()) {
                        Directory.CreateDirectory($@"{tempPath}\{path}");
                        var (animationPath, animatoion) = kaedeProcess.GetAnimationFromPath(id, path);
                        kaedeProcess.BuildAPNG(animationPath, animatoion, $@"{tempPath}\{path}");
                        System.Console.WriteLine($@"{path}: Done.");
                        // サブアニメーション出力
                        var subAnimationPaths = kaedeProcess.GetSubAnimationPaths(wzImage, animationPath);
                        foreach(var subPath in subAnimationPaths.OrEmptyIfNull()) {
                            Directory.CreateDirectory($@"{tempPath}\{subPath}");
                            var (subAnimationPath, subAnimatoion) = kaedeProcess.GetAnimationFromPath(id, subPath);
                            kaedeProcess.BuildAPNG(subAnimationPath, subAnimatoion, $@"{tempPath}\{subPath}");
                            System.Console.WriteLine($@"{subPath}: Done.");
                        }
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
