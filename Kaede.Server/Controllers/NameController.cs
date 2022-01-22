using Kaede.Lib;
using Kaede.Server.Models;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kaede.Server.Controllers {
    public class NameController : ApiController {
        public NameController() { }

        public HttpResponseMessage Get(string id) {
            var mapleDir = ConfigurationManager.AppSettings["mapleDir"];
            var target = ConfigurationManager.AppSettings["target"];
            var kaedeProcess = new KaedeProcess(mapleDir, target);
            var name = kaedeProcess.SearchNameById(id);
            if (name is null) {
                return Request.CreateResponse(HttpStatusCode.NotFound, new NameResponse(null));
            } else {
                var result = new NameItem(id, name);
                var jsonObj = new NameResponse(result);
                return Request.CreateResponse(HttpStatusCode.OK, jsonObj);
            }
        }
    }
}
