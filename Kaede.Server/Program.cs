using Kaede.Lib;
using Kaede.Server.Models;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var port = Environment.GetEnvironmentVariable("PORT") ?? "8000";
var mapleDir = Environment.GetEnvironmentVariable("MAPLE_DIR");
var target = Environment.GetEnvironmentVariable("TARGET");
var resourcesDir = Environment.GetEnvironmentVariable("RESOURCES_DIR")?.Replace(@"\", "/");
const string policyName = "allowAny";
var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

if (mapleDir is null || target is null || resourcesDir is null) {
    throw new Exception("Can't read required env values.");
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => options.AddPolicy(policyName, option => option
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins(new string[] { "http://localhost:3000" })));
var app = builder.Build();
app.UseCors();

var kaedeProcess = new KaedeProcess(mapleDir, target);

var getAnimations =[EnableCors(policyName)] (string id, byte? ratio) => {
    if (ratio is null) {
        ratio = 1;
    }
    var dir = ratio == 1 ? id : @$"{id}_x{ratio}";
    var imgPaths = Directory.GetFiles($@"{resourcesDir}/{dir}", "*.png", SearchOption.AllDirectories).ToList();
    var result = new List<AnimationsItem>();
    imgPaths.ForEach(imgPath => {
        using var fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        var bs = new byte[fs.Length];
        fs.Read(bs, 0, bs.Length);
        fs.Close();
        var animationName = Path.GetDirectoryName(imgPath)?.Replace(@"\", "/").Replace($@"{resourcesDir}/{id}/", "");
        var item = new AnimationsItem(animationName ?? "undefined", Convert.ToBase64String(bs));
        result.Add(item);
    });
    var jsonObj = new AnimationsResponse(result);
    return JsonConvert.SerializeObject(jsonObj, Formatting.Indented, jsonSerializerSettings);
};

var searchNameById =[EnableCors(policyName)] (string id) => {
    var name = kaedeProcess.SearchNameById(id);
    if (name is null) {
        return JsonConvert.SerializeObject(new NameResponse(null), Formatting.Indented, jsonSerializerSettings);
    }
    var result = new NameItem(id, name);
    var jsonObj = new NameResponse(result);
    return JsonConvert.SerializeObject(jsonObj, Formatting.Indented, jsonSerializerSettings);
};

var searchIdsByPartialName =[EnableCors(policyName)] (string name) => {
    var names = kaedeProcess.SearchNamesByPartialName(name);
    var result = new List<IdsItem>();
    foreach (var n in names) {
        var ids = kaedeProcess.SearchIdsByName(n);
        var item = new IdsItem(n, ids);
        result.Add(item);
    }
    var jsonObj = new IdsResponse(result);
    return JsonConvert.SerializeObject(jsonObj, Formatting.Indented, jsonSerializerSettings);
};

app.MapGet("/animations/{id}", getAnimations);
app.MapGet("/name/{id}", searchNameById);
app.MapGet("/ids/{name}", searchIdsByPartialName);

app.Run($"http://localhost:{port}");
