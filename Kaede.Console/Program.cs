using ConsoleAppFramework;
using Kaede.Lib;
using Kaede.Lib.Extensions;
using Kaede.Lib.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kaede.Console {
    public class Program : ConsoleAppBase {
        private readonly string resourcesPath = $@"{Directory.GetCurrentDirectory()}\Resources";

        public static async Task Main(string[] args) {
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args);
        }

        [Command("extract")]
        public void Extract([Option("i", "id of target.")] string id, [Option("w", "name of wz file.")] string wzName = "Mob.wz", [Option("b", "name of monster book csv file.")] string bookName = "MonsterIdList.csv") {
            try {
                System.Console.WriteLine($"--- Kaede process start. ---");
                System.Console.Write("Init: ");
                var kaedeProcess = new KaedeProcess(resourcesPath, wzName, bookName);
                System.Console.WriteLine("Done.");
                System.Console.Write("Extracting WzImage: ");
                var wzImage = kaedeProcess.GetWzImageFromId(id);
                if(wzImage is null) {
                    System.Console.WriteLine($"{id} is not exists or elements are nothing");
                    return;
                }
                System.Console.WriteLine("Done.");

                // APNGの出力
                var animationPaths = kaedeProcess.GetAnimationPaths(wzImage.WzProperties.OrEmptyIfNull());
                var saveRoot = $@"{Directory.GetCurrentDirectory()}\AnimatedPNGs";
                var targetName = kaedeProcess.GetNameFromId(id);
                var dirName = $@"{wzImage.Name}_{targetName}";
                var savePath = $@"{saveRoot}\{dirName}";
                // アニメーション出力
                System.Console.WriteLine($"Target: {wzImage.Name} {targetName}");
                System.Console.WriteLine("APNG build start.");
                foreach(var (path, index) in animationPaths.OrEmptyIfNull().Select((path, index) => (path, index))) {
                    System.Console.Write($@"({index + 1}/{animationPaths.Count()}) {path}: ");
                    var dir = $@"{savePath}\{path}";
                    var dirx2 = $@"{savePath}_x2\{path}";
                    Directory.CreateDirectory(dir);
                    Directory.CreateDirectory(dirx2);
                    var (animationPath, animatoion) = kaedeProcess.GetAnimationFromPath(id, path);
                    kaedeProcess.BuildAPNG(animationPath, animatoion, dir);
                    kaedeProcess.BuildAPNGx2(animationPath, animatoion, dirx2);
                    System.Console.WriteLine("Done.");
                }
                System.Console.WriteLine("APNG build: Done.");
                System.Console.WriteLine("--- Kaede process ended. ---");
            } catch(Exception e) {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine("--- Kaede process abended. ---");
            } finally {
                System.Console.WriteLine("Press any key.");
                System.Console.ReadKey();
            }
        }

        [Command("search_name")]
        public void SearchNameFromId([Option("i", "id of target.")] string id, [Option("b", "name of monster book csv file.")] string bookName = "MonsterIdList.csv") {
            if(!File.Exists($@"{resourcesPath}\{bookName}")) {
                throw new Exception($@"{resourcesPath}\{bookName} is not exists.");
            }
            try {
                var monsterBook = new MonsterBook(CSVReader.ReadCSV($@"{resourcesPath}\{bookName}", true));
                var name = monsterBook.GetNameFromId(id);
                System.Console.WriteLine($"{id} : {name}");
            } catch {
                throw;
            } finally {
                System.Console.WriteLine();
            }
        }

        [Command("search_ids")]
        public void SearchIdsFromName([Option("n", "A part of target name.")] string name, [Option("b", "name of monster book csv file.")] string bookName = "MonsterIdList.csv") {
            if(!File.Exists($@"{resourcesPath}\{bookName}")) {
                throw new Exception($@"{resourcesPath}\{bookName} is not exists.");
            }
            try {
                var monsterBook = new MonsterBook(CSVReader.ReadCSV($@"{resourcesPath}\{bookName}", true));
                var names = monsterBook.GetNamesFromVagueName(name);
                foreach(var n in names) {
                    var ids = monsterBook.GetIdsFromName(n);
                    System.Console.WriteLine(n);
                    foreach(var item in ids.Select((id, index) => (id, index))) {
                        System.Console.WriteLine($" - {item.id}");
                    }
                    System.Console.WriteLine();
                }
            } catch {
                throw;
            } finally {
                System.Console.WriteLine();
            }
        }
    }
}
