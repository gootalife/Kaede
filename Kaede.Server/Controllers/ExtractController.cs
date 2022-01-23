using Kaede.Lib;
using Kaede.Server.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;

namespace Kaede.Server.Controllers {
    public class ExtractController : ApiController {
        public ExtractController() { }

        public HttpResponseMessage Get(string id, byte? ratio = 1) {
            var mapleDir = ConfigurationManager.AppSettings["mapleDir"];
            var target = ConfigurationManager.AppSettings["target"];
            var kaedeProcess = new KaedeProcess(mapleDir, target);
            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var result = new List<AnimationsItem>();
            var wzImage = kaedeProcess.GetWzImageById(id);
            if (wzImage is null) {
                return Request.CreateResponse(HttpStatusCode.NotFound, new AnimationsResponse(result));
            }
            var animationPaths = kaedeProcess.GetAnimationPaths(wzImage.WzProperties);
            var targetName = kaedeProcess.SearchNameById(id);
            foreach (var (animationName, index) in animationPaths.Select((path, index) => (path, index))) {
                var animatoion = kaedeProcess.GetAnimationByPath(id, animationName);
                using (var stream = KaedeProcess.BuildAPNGToStream(animationName, animatoion, (byte)ratio)) {
                    var bs = new byte[stream.Length];
                    stream.Read(bs, 0, bs.Length);
                    var item = new AnimationsItem(animationName, Convert.ToBase64String(bs));
                    result.Add(item);
                }
            }
            var jsonObj = new AnimationsResponse(result);
            return Request.CreateResponse(HttpStatusCode.OK, new AnimationsResponse(result));
        }
    }
}
