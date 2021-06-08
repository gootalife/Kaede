using HaRepacker;
using Kaede.Lib.Extensions;
using Kaede.Lib.Models;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Point = Kaede.Lib.Models.Point;

namespace Kaede.Lib {
    public class KaedeProcess {
        private readonly WzFile wzFile;
        private readonly WzNode wzNode;
        private readonly MonsterBook monsterBook;
        private const string imgExtension = ".img";

        public KaedeProcess(string resourcesPath, string wzName, string csvName) {
            if(!File.Exists($@"{resourcesPath}\{wzName}")) {
                throw new Exception($@"{resourcesPath}{wzName} is not exists.");
            }
            if(!File.Exists($@"{resourcesPath}\{csvName}")) {
                throw new Exception($@"{resourcesPath}\{csvName} is not exists.");
            }
            try {
                monsterBook = new MonsterBook(CSVReader.ReadCSV($@"{resourcesPath}\{csvName}", true));
                WzFileManager wzFileManager = new WzFileManager();
                wzFile = wzFileManager.LoadWzFile($@"{resourcesPath}\{wzName}", WzMapleVersion.BMS);
                wzNode = new WzNode(wzFile);
            } catch(Exception e) {
                throw e;
            }
        }

        /// <summary>
        /// 名前からWzImageを取得
        /// </summary>
        /// <param name="name">モンスター名</param>
        /// <exception cref="Exception"></exception>
        /// <returns>WzImage</returns>
        public WzImage GetWzImageFromName(string name) {
            var wzImageNodes = wzNode.Nodes.Where(node => node.Tag is WzImage).Select(node => (WzImage)node.Tag);
            WzImage wzImage;
            wzImage = null;
            try {
                var idList = monsterBook.GetIdsFromName(name);
                foreach(var id in idList) {
                    if(wzImageNodes.Select(node => node.Name).Contains(id + imgExtension) && wzImage == null) {
                        var imgs = wzImageNodes.Where(node => node.Name == id + imgExtension).Where(node => node.WzProperties.Count > 0);
                        if(imgs.Count() > 0) {
                            wzImage = imgs.First();
                        }
                    }
                }
            } catch(Exception e) {
                throw e;
            }
            return wzImage;
        }

        /// <summary>
        /// idからWzImageを取得
        /// </summary>
        /// <param name="id">モンスターID</param>
        /// <exception cref="Exception"></exception>
        /// <returns>WzImage</returns>
        public WzImage GetWzImageFromId(string id) {
            var wzImageNodes = wzNode.Nodes.Where(node => node.Tag is WzImage).Select(node => (WzImage)node.Tag);
            WzImage wzImage = null;
            try {
                var imgs = wzImageNodes.Where(node => node.Name == id + imgExtension).Where(node => node.WzProperties.Count > 0);
                if(imgs.Count() > 0) {
                    wzImage = imgs.First();
                }
            } catch(Exception e) {
                throw e;
            }
            return wzImage;
        }

        /// <summary>
        /// 指定パス直下のフレームを取得<br/>
        /// exp. 9300708.img/attack1/info/hit → GetAnimationFromPath("9300708", "attack1/info/hit");
        /// </summary>
        /// <param name="id">モンスターID</param>
        /// <param name="path">検索パス</param>
        /// <exception cref="Exception"></exception>
        /// <returns>指定したパスのアニメーション</returns>
        public (string animationPath, IEnumerable<AnimationFrame> animatoion) GetAnimationFromPath(string id, string path) {
            var animation = new List<AnimationFrame>();
            var wzImage = wzNode.Nodes
                .Where(node => node.Tag is WzImage)
                .Select(node => (WzImage)node.Tag)
                .Where(img => img.Name == id + imgExtension)?.First();
            var imgProp = wzImage?.GetFromPath(path);
            if(imgProp?.WzProperties?.Count() <= 0) {
                throw new Exception("空のノードか無効なノードです。");
            }
            var animationName = imgProp.Name;
            if(imgProp.Parent?.Parent?.Name != imgProp.WzFileParent.Name) {
                animationName = $@"{imgProp.Parent?.Parent?.Name}/{imgProp.Parent?.Name}/{animationName}";
            }
            foreach(var child in imgProp.WzProperties?.Where(child => child is WzCanvasProperty || child is WzUOLProperty)) {
                WzCanvasProperty canvasProperty;
                Bitmap image;
                if(child is WzCanvasProperty || child is WzUOLProperty) {
                    if(child is WzCanvasProperty property1) {
                        canvasProperty = property1;
                        image = canvasProperty.GetLinkedWzCanvasBitmap();
                    } else {
                        var linkVal = ((WzUOLProperty)child).LinkValue;
                        if(linkVal is WzCanvasProperty property2) {
                            canvasProperty = property2;
                            image = canvasProperty.GetLinkedWzCanvasBitmap();
                        } else {
                            break;
                        }
                    }
                    var delay = canvasProperty[WzCanvasProperty.AnimationDelayPropertyName]?.GetInt();
                    if(delay == null) {
                        delay = 0;
                    }
                    var origin = canvasProperty.GetCanvasOriginPosition();
                    var animationFrame = new AnimationFrame(image, animationName, child.Name, new Point((int)origin.X, (int)origin.Y), (int)delay);
                    animation.Add(animationFrame);
                }

            }
            return (animationName, animation);
        }

        /// <summary>
        /// 指定プロパティ以下のアニメーションのパスをすべて取得
        /// </summary>
        /// <param name="wzImageProperties">開始プロパティ</param>
        /// <returns>アニメーションのパス</returns>
        public IEnumerable<string> GetAnimationPaths(IEnumerable<WzImageProperty> wzImageProperties) {
            var list = new List<string>();
            foreach(var wzImageProperty in wzImageProperties) {
                // 子がWzCanvasPropertyかWzUOLPropertyを持つなら抽出
                if(wzImageProperty.WzProperties?.Where(child => child is WzCanvasProperty || child is WzUOLProperty).Count() > 0) {
                    var path = wzImageProperty.FullPath.Replace($"{wzImageProperty.ParentImage.FullPath}\\", "").Replace('\\', '/');
                    list.Add(path);
                }
                // 子がWzSubPropertyを持つなら下階層の探索を再帰的に継続
                if(wzImageProperty.WzProperties?.Where(child => child is WzSubProperty).Count() > 0) {
                    var paths = GetAnimationPaths(wzImageProperty.WzProperties);
                    list.AddRange(paths);
                }
            }
            return list;
        }

        /// <summary>
        /// idから名前を取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns>モンスター名</returns>
        public string GetNameFromId(string id) {
            return monsterBook.GetNameFromId(id);
        }

        /// <summary>
        /// 名前からidを取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns>モンスターID</returns>
        public IEnumerable<string> GetIdsFromName(string name) {
            return monsterBook.GetIdsFromName(name);
        }

        /// <summary>
        /// 名前の一部からそれを含む名前を取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns>引数の文字列を含むモンスター名のコレクション</returns>
        public IEnumerable<string> GetNamesFromVagueName(string name) {
            return monsterBook.GetNamesFromVagueName(name);
        }

        /// <summary>
        /// APNGを生成
        /// </summary>
        /// <param name="animationName">アニメーション名</param>
        /// <param name="animation">画像のコレクション</param>
        /// <param name="savePath">保存先パス</param>
        /// <exception cref="Exception"></exception>
        public void BuildAPNG(string animationName, IEnumerable<AnimationFrame> animation, string savePath) {
            try {
                FrameEditor frameEditor = new FrameEditor(animationName.Split('/')?.Last(), animation);
                var (frames, animInfo) = frameEditor.EditPNGImages();
                APNGBuilder aPNGBuilder = new APNGBuilder(frames, animInfo);
                aPNGBuilder.BuildAnimation(savePath);
            } catch(Exception e) {
                throw e;
            }
        }
    }
}
