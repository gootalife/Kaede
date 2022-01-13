using Kaede.Lib;
using Kaede.Lib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var app = ConsoleApp.Create(args);
app.AddCommand("extract", Extract);
app.AddCommand("name", SearchNameFromId);
app.AddCommand("id", SearchIdsFromName);
app.Run();

static void Extract([Option("i", "Id of target")] string id,
            [Option("p", "Path of MapleStory's directory")] string mapleDir,
            [Option("t", "Name of {TARGET.wz}")] string target,
            [Option("r", "Ratio of output images size")] byte ratio = 1) {
    try {
        Console.WriteLine($"--- Kaede process start. ---");
        Console.Write("Init: ");
        var kaedeProcess = new KaedeProcess(mapleDir, target);
        Console.WriteLine("Done.");
        Console.Write("Extracting WzImage: ");
        var wzImage = kaedeProcess.GetWzImageById(id);
        if (wzImage is null) {
            Console.WriteLine($"{id} is not exists or elements are nothing");
            return;
        }
        Console.WriteLine("Done.");

        // APNGの出力
        var animationPaths = kaedeProcess.GetAnimationPaths(wzImage.WzProperties.OrEmptyIfNull());
        var targetName = kaedeProcess.SearchNameById(id);
        var savePath = $@"{Directory.GetCurrentDirectory()}/AnimatedPNGs/{id}";
        Console.WriteLine($"Target: {wzImage.Name} {targetName}");
        Console.WriteLine("Building APNG: start.");
        foreach (var (animationName, index) in animationPaths.OrEmptyIfNull().Select((path, index) => (path, index))) {
            Console.Write($@"({index + 1,2}/{animationPaths.Count(),2}) {animationName}: ");
            var dir = ratio == 1 ? $@"{savePath}/{animationName}" : $@"{savePath}_x{ratio}/{animationName}";
            Directory.CreateDirectory(dir);
            var animatoion = kaedeProcess.GetAnimationByPath(id, animationName);
            KaedeProcess.BuildAPNG(animationName, animatoion, ratio, dir);
            Console.WriteLine("Done.");
        }
        Console.WriteLine("Building APNG: Done.");
        Console.WriteLine("--- Kaede process ended. ---");
    } catch (Exception e) {
        Console.WriteLine(e.Message);
        Console.WriteLine("*** Kaede process abended. ***");
    }
}

static void SearchNameFromId([Option("i", "Id of target")] string id,
            [Option("p", "Path of MapleStory's directory")] string mapleDir) {
    var kaedeProcess = new KaedeProcess(mapleDir, "Mob.wz");
    var name = kaedeProcess.SearchNameById(id);
    var jsonObj = new Dictionary<string, string> {
                { id, name }
            };
    var json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
    Console.WriteLine(json);
}

static void SearchIdsFromName([Option("n", "Part of target name")] string name,
            [Option("p", "Path of MapleStory's directory")] string mapleDir) {
    var kaedeProcess = new KaedeProcess(mapleDir, "Mob.wz");
    var names = kaedeProcess.SearchNamesByPartialName(name);
    var jsonObj = new Dictionary<string, IEnumerable<string>>();
    foreach (var n in names) {
        var ids = kaedeProcess.SearchIdsByName(n);
        jsonObj.Add(n, ids);
    }
    var json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
    Console.WriteLine(json);
}