using ConsoleAppFramework;
using Kaede.Lib;
using Kaede.Lib.Extensions;
using Kaede.Lib.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CS = System.Console;

namespace Kaede.Console {
    public class Program : ConsoleAppBase {
        public static async Task Main(string[] args) {
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args);
        }

        [Command("extract")]
        public void Extract([Option("i", "Id of target.")] string id,
            [Option("w", "Name of wz file.")] string wzPath,
            [Option("b", "Name of monster book csv file.")] string bookPath,
            [Option("m", "Magnification of output images size.")] byte magnification = 1) {
            try {
                CS.WriteLine($"--- Kaede process start. ---");
                CS.Write("Init: ");
                var kaedeProcess = new KaedeProcess(wzPath, bookPath);
                CS.WriteLine("Done.");
                CS.Write("Extracting WzImage: ");
                var wzImage = kaedeProcess.GetWzImageFromId(id);
                if(wzImage is null) {
                    CS.WriteLine($"{id} is not exists or elements are nothing");
                    return;
                }
                CS.WriteLine("Done.");

                // APNGの出力
                var animationPaths = kaedeProcess.GetAnimationPaths(wzImage.WzProperties.OrEmptyIfNull());
                var saveRoot = $@"{Directory.GetCurrentDirectory()}\AnimatedPNGs";
                var targetName = kaedeProcess.GetNameFromId(id);
                var dirName = $@"{wzImage.Name}_{targetName}";
                var savePath = $@"{saveRoot}\{dirName}";
                CS.WriteLine($"Target: {wzImage.Name} {targetName}");
                CS.WriteLine("APNG build start.");
                foreach(var (path, index) in animationPaths.OrEmptyIfNull().Select((path, index) => (path, index))) {
                    CS.Write($@"({index + 1, 2}/{animationPaths.Count(), 2}) {path}: ");
                    var dir = magnification == 1 ? $@"{savePath}\{path}" : $@"{savePath}_x{magnification}\{path}";
                    Directory.CreateDirectory(dir);
                    var (animationPath, animatoion) = kaedeProcess.GetAnimationFromPath(id, path);
                    kaedeProcess.BuildAPNG(animationPath, animatoion, magnification, dir);
                    CS.WriteLine("Done.");
                }
                CS.WriteLine("APNG build: Done.");
                CS.WriteLine("--- Kaede process ended. ---");
            } catch(Exception e) {
                CS.WriteLine(e.Message);
                CS.WriteLine("--- Kaede process abended. ---");
            }
        }

        [Command("search_name")]
        public void SearchNameFromId([Option("i", "Id of target.")] string id,
            [Option("b", "Name of monster book csv file.")] string bookPath) {
            if(!File.Exists($@"{bookPath}")) {
                throw new Exception($@"{bookPath} is not exists.");
            }
            var monsterBook = new MonsterBook(CSVReader.ReadCSV($@"{bookPath}", true));
            var name = monsterBook.GetNameFromId(id);
            var jsonObj = new Dictionary<string, string> {
                { id, name }
            };
            var json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            CS.WriteLine(json);
        }

        [Command("search_ids")]
        public void SearchIdsFromName([Option("n", "A part of target name.")] string name,
            [Option("b", "Name of monster book csv file.")] string bookPath) {
            if(!File.Exists($@"{bookPath}")) {
                throw new Exception($@"{bookPath} is not exists.");
            }
            var monsterBook = new MonsterBook(CSVReader.ReadCSV($@"{bookPath}", true));
            var names = monsterBook.GetNamesFromVagueName(name);
            var jsonObj = new Dictionary<string, IEnumerable<string>>();
            foreach(var n in names) {
                var ids = monsterBook.GetIdsFromName(n);
                jsonObj.Add(n, ids);
            }
            var json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            CS.WriteLine(json);
        }
    }
}
