using ConsoleAppFramework;
using Kaede.Lib;
using Kaede.Lib.Extensions;
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
        public void Extract([Option("i", "Id of target")] string id,
            [Option("p", "Path of MapleStory's directory")] string mapleDir,
            [Option("t", "Name of {TARGET}.wz")] string target,
            [Option("r", "Rate of output images size")] byte rate = 1) {
            try {
                CS.WriteLine($"--- Kaede process start. ---");
                CS.Write("Init: ");
                var kaedeProcess = new KaedeProcess(mapleDir, target);
                CS.WriteLine("Done.");
                CS.Write("Extracting WzImage: ");
                var wzImage = kaedeProcess.GetWzImageFromId(id);
                var a = kaedeProcess.GetNamesFromPartialName("ピンクビーン");
                if (wzImage is null) {
                    CS.WriteLine($"{id} is not exists or elements are nothing");
                    return;
                }
                CS.WriteLine("Done.");

                // APNGの出力
                var animationPaths = kaedeProcess.GetAnimationPaths(wzImage.WzProperties.OrEmptyIfNull());
                var targetName = kaedeProcess.GetNameFromId(id);
                var savePath = $@"{Directory.GetCurrentDirectory()}/AnimatedPNGs/{wzImage.Name}_{targetName}";
                CS.WriteLine($"Target: {wzImage.Name} {targetName}");
                CS.WriteLine("Building APNG: start.");
                foreach (var (animationName, index) in animationPaths.OrEmptyIfNull().Select((path, index) => (path, index))) {
                    CS.Write($@"({index + 1,2}/{animationPaths.Count(),2}) {animationName}: ");
                    var dir = rate == 1 ? $@"{savePath}/{animationName}" : $@"{savePath}_x{rate}/{animationName}";
                    Directory.CreateDirectory(dir);
                    var animatoion = kaedeProcess.GetAnimationFromPath(id, animationName);
                    KaedeProcess.BuildAPNG(animationName, animatoion, rate, dir);
                    CS.WriteLine("Done.");
                }
                CS.WriteLine("Building APNG: Done.");
                CS.WriteLine("--- Kaede process ended. ---");
            } catch (Exception e) {
                CS.WriteLine(e.Message);
                CS.WriteLine("*** Kaede process abended. ***");
            }
        }

        [Command("name")]
        public void SearchNameFromId([Option("i", "Id of target")] string id,
            [Option("p", "Path of MapleStory's directory")] string mapleDir) {
            var kaedeProcess = new KaedeProcess(mapleDir, "Mob.wz");
            var name = kaedeProcess.GetNameFromId(id);
            var jsonObj = new Dictionary<string, string> {
                { id, name }
            };
            var json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            CS.WriteLine(json);
        }

        [Command("id")]
        public void SearchIdsFromName([Option("n", "Part of target name")] string name,
            [Option("p", "Path of MapleStory's directory")] string mapleDir) {
            var kaedeProcess = new KaedeProcess(mapleDir, "Mob.wz");
            var names = kaedeProcess.GetNamesFromPartialName(name);
            var jsonObj = new Dictionary<string, IEnumerable<string>>();
            foreach (var n in names) {
                var ids = kaedeProcess.GetIdsFromName(n);
                jsonObj.Add(n, ids);
            }
            var json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            CS.WriteLine(json);
        }
    }
}
