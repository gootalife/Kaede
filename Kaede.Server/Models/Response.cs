namespace Kaede.Server.Models {
    public record NameResponse(NameItem? Result);
    public record NameItem(string Id, string Name);
    public record IdsResponse(List<IdsItem> Result);
    public record IdsItem(string Name, IEnumerable<string> Ids);
    public record AnimationsResponse(List<AnimationsItem> Result);
    public record AnimationsItem(string AnimationName, string ImageStr);
}
