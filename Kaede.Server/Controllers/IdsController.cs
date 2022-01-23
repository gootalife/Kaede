using Kaede.Lib;
using Kaede.Server.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kaede.Server.Controllers {
    public class IdsController : ApiController {
        public IdsController() { }

        public HttpResponseMessage Get(string name) {
            var mapleDir = ConfigurationManager.AppSettings["mapleDir"];
            var target = ConfigurationManager.AppSettings["target"];
            var kaedeProcess = new KaedeProcess(mapleDir, target);
            var names = kaedeProcess.SearchNamesByPartialName(name);
            var result = new List<IdsItem>();
            foreach (var n in names) {
                var ids = kaedeProcess.SearchIdsByName(n);
                var item = new IdsItem(n, ids);
                result.Add(item);
            }
            var jsonObj = new IdsResponse(result);
            if(!result.Any()) {
                Request.CreateResponse(HttpStatusCode.NotFound, jsonObj);
            }
            return Request.CreateResponse(HttpStatusCode.OK, jsonObj);
        }
    }
}
