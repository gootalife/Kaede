using System.Collections.Generic;

namespace Kaede.Server.Models {
    public class NameResponse {
        public NameItem Result { get; }
        public NameResponse(NameItem Result) {
            this.Result = Result;
        }
    }
    public class NameItem {
        public string Id { get; }
        public string Name { get; }
        public NameItem(string Id, string Name) {
            this.Id = Id;
            this.Name = Name;
        }
    }
    public class IdsResponse {
        public List<IdsItem> Result { get; }
        public IdsResponse(List<IdsItem> Result) {
            this.Result = Result;
        }
    }

    public class IdsItem {
        public string Name { get; }
        public IEnumerable<string> Ids { get; }
        public IdsItem(string Name, IEnumerable<string> Ids) {
            this.Name = Name;
            this.Ids = Ids;
        }
    }
    public class AnimationsResponse {
        public List<AnimationsItem> Result { get; }
        public AnimationsResponse(List<AnimationsItem> Result) {
            this.Result = Result;
        }
    }
    public class AnimationsItem {
        public string AnimationName { get; }
        public string ImageStr { get; }
        public AnimationsItem(string AnimationName, string ImageStr) {
            this.AnimationName = AnimationName;
            this.ImageStr = ImageStr;
        }
    }
}
