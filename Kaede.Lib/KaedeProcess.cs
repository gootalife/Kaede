﻿using HaRepacker;
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
        private readonly string imgExtension;

        public KaedeProcess(string resourcesPath) {
            imgExtension = ".img";
            string wzName = "Mob.wz";
            string csvName = "MonsterIDList.csv";
            if(!File.Exists($@"{resourcesPath}/{wzName}")) {
                throw new Exception($@"{resourcesPath}/{wzName} is not exists.");
            }
            if(!File.Exists($@"{resourcesPath}/{csvName}")) {
                throw new Exception($@"{resourcesPath}/{csvName} is not exists.");
            }
            try {
                monsterBook = new MonsterBook(CSVReader.ReadCSV($@"{resourcesPath}/{csvName}", true));
                WzFileManager wzFileManager = new WzFileManager();
                wzFile = wzFileManager.LoadWzFile($@"{resourcesPath}/{wzName}", WzMapleVersion.BMS);
                wzNode = new WzNode(wzFile);
            } catch(Exception e) {
                throw e;
            }
        }

        /// <summary>
        /// 名前からWzImageを取得
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public WzImage GetWzImageFromName(string name) {
            var wzImageNodes = wzNode.Nodes.Where(nd => nd.Tag is WzImage).Select(nd => (WzImage)nd.Tag);
            WzImage wzImage;
            wzImage = null;
            try {
                var idList = monsterBook.GetIdsFromName(name);
                foreach(var id in idList) {
                    if(wzImageNodes.Select(nd => nd.Name).Contains(id + imgExtension) && wzImage == null) {
                        var imgs = wzImageNodes.Where(nd => nd.Name == id + imgExtension).Where(nd => nd.WzProperties.Count > 0);
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
        /// <param name="id"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public WzImage GetWzImageFromId(string id) {
            var wzImageNodes = wzNode.Nodes.Where(nd => nd.Tag is WzImage).Select(nd => (WzImage)nd.Tag);
            WzImage wzImage = null;
            try {
                var imgs = wzImageNodes.Where(nd => nd.Name == id + imgExtension).Where(nd => nd.WzProperties.Count > 0);
                if(imgs.Count() > 0) {
                    wzImage = imgs.First();
                }
            } catch(Exception e) {
                throw e;
            }
            return wzImage;
        }

        public IEnumerable<string> GetMainAnimationPaths(WzImage wzImage) {
            return wzImage.WzProperties
                .Where(x => x is WzSubProperty)
                .Where(elem => elem.WzProperties?.First() is WzCanvasProperty || elem.WzProperties?.First() is WzUOLProperty)
                .Select(elem => elem.Name);
        }

        public IEnumerable<string> GetSubAnimationPaths(WzImage wzImage, string animationName) {
            return wzImage.GetFromPath($@"{animationName}/info")?.WzProperties?
                    .Where(x => x is WzSubProperty)
                    .Where(x => x.WzProperties?.First() is WzCanvasProperty || x.WzProperties?.First() is WzUOLProperty)
                    .Select(y => $@"{y.Parent?.Parent?.Name}/{y.Parent?.Name}/{y.Name}");
        }

        /// <summary>
        /// 指定パス直下のフレームを取得<br/>
        /// exp. 9300708.img/attack1/info/hit → GetAnimationFromPath("9300708",attack1/info/hit");
        /// </summary>
        /// <param name="wzImage"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
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
            foreach(var elem in imgProp.WzProperties?.Where(elem => elem is WzCanvasProperty || elem is WzUOLProperty)) {
                WzCanvasProperty canvasProperty;
                Bitmap image;
                if(elem is WzCanvasProperty || elem is WzUOLProperty) {
                    if(elem is WzCanvasProperty property1) {
                        canvasProperty = property1;
                        image = canvasProperty.GetLinkedWzCanvasBitmap();
                    } else {
                        var linkVal = ((WzUOLProperty)elem).LinkValue;
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
                    var animationFrame = new AnimationFrame(image, animationName, elem.Name, new Point((int)origin.X, (int)origin.Y), (int)delay);
                    animation.Add(animationFrame);
                }

            }
            return (animationName, animation);
        }

        /// <summary>
        /// idから名前を取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetNameFromId(string id) {
            return monsterBook.GetNameFromId(id);
        }

        /// <summary>
        /// 名前からidを取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<string> GetIdsFromName(string name) {
            return monsterBook.GetIdsFromName(name);
        }

        /// <summary>
        /// 名前の一部からそれを含む名前を取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<string> GetNamesFromVagueName(string name) {
            return monsterBook.GetNamesFromVagueName(name);
        }

        /// <summary>
        /// APNGを生成
        /// </summary>
        /// <param name="elemensts"></param>
        /// <param name="savePath"></param>
        /// <param name="dirName"></param>
        /// <exception cref="Exception"></exception>
        public void BuildAPNG(string animationName, IEnumerable<AnimationFrame> elemensts, string savePath) {
            // APNGの出力
            try {
                FrameEditor frameEditor = new FrameEditor(animationName.Split('/')?.Last(), elemensts);
                var (frames, animInfo) = frameEditor.EditPNGImages();
                APNGBuilder aPNGBuilder = new APNGBuilder(frames, animInfo);
                aPNGBuilder.BuildAnimation(savePath);
            } catch(Exception e) {
                throw e;
            }
        }
    }
}
