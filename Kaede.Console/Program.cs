using Kaede.Lib;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Kaede.Lib.Models;
using Kaede.Lib.Extensions;

namespace Kaede.Console {
    public class Program {
        public static async Task Main(string[] args) {
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
                    mainAnimationPaths.ForEach(path => {
                        System.Console.Write($@"{dirName}/{path}: ");
                        Directory.CreateDirectory($@"{tempPath}\{path}");
                        var (animationPath, animatoion) = kaedeProcess.GetAnimationFromPath(id, path);
                        kaedeProcess.BuildAPNG(animationPath, animatoion, $@"{tempPath}\{path}");
                        System.Console.WriteLine("Done.");
                        // サブアニメーション出力
                        var sub = kaedeProcess.GetSubAnimationPaths(wzImage, animationPath);
                        sub?.ForEach(subPath => {
                            System.Console.Write($@"{dirName}/{subPath}: ");
                            Directory.CreateDirectory($@"{tempPath}\{subPath}");
                            var (subAnimationPath, subAnimatoion) = kaedeProcess.GetAnimationFromPath(id, subPath);
                            kaedeProcess.BuildAPNG(subAnimationPath, subAnimatoion, $@"{tempPath}\{subPath}");
                            System.Console.WriteLine("Done.");
                        });
                    });
                    // 保存先にリネーム
                    System.Console.WriteLine("APNG build: Done.");
                    Directory.Move(tempPath, savePath);
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
