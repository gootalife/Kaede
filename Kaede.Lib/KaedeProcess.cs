﻿using HaRepacker;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Lib {
    public class KaedeProcess {
        public WzFile WzFile { get; private set; }
        private WzNode wzNode;
        private MonsterBook monsterBook;
        private string imgExtension;
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
                WzFile = wzFileManager.LoadWzFile($@"{resourcesPath}/{wzName}", WzMapleVersion.BMS);
                wzNode = new WzNode(WzFile);
            } catch(Exception e) {
                throw e;
            }
        }
        public KaedeProcess() {
        }

        public void Init(WzFile wzFile, MonsterBook monsterBook) {
            imgExtension = ".img";
            this.WzFile = wzFile;
            this.monsterBook = monsterBook;
            wzNode = new WzNode(wzFile);
        }

        public KaedeProcess(WzFile wzFile, MonsterBook monsterBook) {
            imgExtension = ".img";
            this.WzFile = wzFile;
            this.monsterBook = monsterBook;
            wzNode = new WzNode(wzFile);
        }

        public WzImage GetWzImageFromName(string name) {
            IEnumerable<WzImage> wzImageNodes = wzNode.Nodes.Where(nd => nd.Tag is WzImage).Select(nd => (WzImage)nd.Tag);
            //画像の抽出
            WzImage wzImage;
            try {
                IEnumerable<string> idList = GetIdsFromName(name);
                wzImage = null;
                idList.ForEach(id => {
                    if(wzImageNodes.Select(nd => nd.Name).Contains(id + imgExtension) && wzImage == null) {
                        IEnumerable<WzImage> imgs = wzImageNodes.Where(nd => nd.Name == id + imgExtension).Where(nd => nd.WzProperties.Count > 0);
                        if(imgs.Count() > 0) {
                            wzImage = imgs.ElementAt(0);
                        }
                    }
                });
            } catch {
                wzImage = null;
            }
            return wzImage;
        }

        public WzImage GetWzImageFromId(string id) {
            IEnumerable<WzImage> wzImageNodes = wzNode.Nodes.Where(nd => nd.Tag is WzImage).Select(nd => (WzImage)nd.Tag);
            //画像の抽出
            WzImage wzImage;
            try {
                wzImage = null;
                IEnumerable<WzImage> imgs = wzImageNodes.Where(nd => nd.Name == id + imgExtension).Where(nd => nd.WzProperties.Count > 0);
                if(imgs.Count() > 0) {
                    wzImage = imgs.ElementAt(0);
                }
            } catch {
                wzImage = null;
            }
            return wzImage;
        }

        public Dictionary<string, List<AnimationFrame>> GetAnimationElements(WzImage wzImage) {
            // アニメーション名と各フレームを取得
            Dictionary<string, List<AnimationFrame>> elements = new Dictionary<string, List<AnimationFrame>>();
            wzImage.WzProperties.Where(animation => animation is WzSubProperty && animation.Name != "info").ForEach(animation => {
                List<AnimationFrame> animationFrames = new List<AnimationFrame>();
                animation.WzProperties.ForEach(frame => {
                    WzCanvasProperty canvasProperty;
                    Bitmap image;
                    if(frame is WzCanvasProperty || frame is WzUOLProperty) {
                        if(frame is WzCanvasProperty property1) {
                            canvasProperty = property1;
                            image = canvasProperty.GetLinkedWzCanvasBitmap();
                        } else {
                            WzObject linkVal = ((WzUOLProperty)frame).LinkValue;
                            if(linkVal is WzCanvasProperty property2) {
                                canvasProperty = property2;
                                image = canvasProperty.GetLinkedWzCanvasBitmap();
                            } else {
                                return;
                            }
                        }
                        int? delay = canvasProperty[WzCanvasProperty.AnimationDelayPropertyName]?.GetInt();
                        if(delay == null) {
                            delay = 0;
                        }
                        PointF origin = canvasProperty.GetCanvasOriginPosition();
                        AnimationFrame animationFrame = new AnimationFrame(image, animation.Name, frame.Name, new Point((int)origin.X, (int)origin.Y), (int)delay);
                        animationFrames.Add(animationFrame);
                    }
                });
                elements.Add(animation.Name, animationFrames);
            });
            return elements;
        }

        public string GetNameFromId(string id) {
            return monsterBook.GetNameFromId(id);
        }

        public IEnumerable<string> GetIdsFromName(string name) {
            return monsterBook.GetIdsFromName(name);
        }

        public void BuildAPNGs(Dictionary<string, List<AnimationFrame>> elemensts, string saveRoot, string dirName) {
            // APNGの出力
            FrameEditor frameEditor = new FrameEditor(elemensts);
            Dictionary<string, List<AnimationFrame>> newMaterials = frameEditor.EditPNGImages();
            APNGBuilder aPNGBuilder = new APNGBuilder(newMaterials);
            aPNGBuilder.BuildAnimations(saveRoot, dirName);
        }
    }
}