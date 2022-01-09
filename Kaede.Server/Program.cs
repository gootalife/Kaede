using Kaede.Lib;
using Newtonsoft.Json;

var app = WebApplication.Create(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
var mapleDir = Environment.GetEnvironmentVariable("MAPLE_DIR");
var target = Environment.GetEnvironmentVariable("TARGET");
var resourcesDir = Environment.GetEnvironmentVariable("RESOURCES_DIR")?.Replace(@"\", "/");

if (mapleDir is null || target is null) {
    throw new Exception("Can't read required env values.");
}

var kaedeProcess = new KaedeProcess(mapleDir, target);

var getAnimations = (string id, byte? ratio) => {
    if (ratio is null) {
        ratio = 1;
    }
    var dir = ratio == 1 ? id : @$"{id}_x{ratio}";
    var imgPaths = Directory.GetFiles($@"{resourcesDir}/{dir}", "*.png", SearchOption.AllDirectories).ToList();
    var jsonObj = new Dictionary<string, string>();
    imgPaths.ForEach(imgPath => {
        using var fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        var bs = new byte[fs.Length];
        fs.Read(bs, 0, bs.Length);
        fs.Close();
        jsonObj.Add(imgPath.Replace(@"\", "/").Replace($@"{resourcesDir}/{id}/", ""), Convert.ToBase64String(bs));
    });
    return JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
};

var getName = (string id) => {
    var name = kaedeProcess.GetNameFromID(id);
    var jsonObj = new Dictionary<string, string> {
                { id, name }
            };
    return JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
};

var getIDs = (string name) => {
    var names = kaedeProcess.GetNamesFromPartialName(name);
    var jsonObj = new Dictionary<string, IEnumerable<string>>();
    foreach (var n in names) {
        var ids = kaedeProcess.GetIdsFromName(n);
        jsonObj.Add(n, ids);
    }
    return JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
};

app.MapGet("/animations/{id}", getAnimations);
app.MapGet("/name/{id}", getName);
app.MapGet("/ids/{name}", getIDs);

app.Run($"http://localhost:{port}");
