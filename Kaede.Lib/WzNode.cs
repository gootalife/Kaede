using MapleLib.WzLib;
using System;
using System.Collections.Generic;

namespace Kaede.Lib {
    public class WzNode {
        public List<WzNode> Nodes { get; set; }
        public object Tag { get; set; }
        public string Text { get; set; }

        public WzNode(WzObject SourceObject) {
            Text = SourceObject.Name;
            Tag = SourceObject;
            Nodes = new List<WzNode>();
            // Childs
            ParseChilds(SourceObject);
        }

        private void ParseChilds(WzObject SourceObject) {
            Tag = SourceObject ?? throw new NullReferenceException("Cannot create a null WzNode");
            SourceObject.HRTag = this;

            if(SourceObject is WzFile)
                SourceObject = ((WzFile)SourceObject).WzDirectory;
            if(SourceObject is WzDirectory) {
                foreach(WzDirectory dir in ((WzDirectory)SourceObject).WzDirectories)
                    Nodes.Add(new WzNode(dir));
                foreach(WzImage img in ((WzDirectory)SourceObject).WzImages)
                    Nodes.Add(new WzNode(img));
            } else if(SourceObject is WzImage image) {
                if(image.Parsed)
                    foreach(WzImageProperty prop in image.WzProperties)
                        Nodes.Add(new WzNode(prop));
            } else if(SourceObject is IPropertyContainer container) {
                foreach(WzImageProperty prop in container.WzProperties)
                    Nodes.Add(new WzNode(prop));
            }
        }
    }
}
