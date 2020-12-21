using HaRepacker;
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
        private readonly WzFile wzFile;
        private readonly WzNode wzNode;
        private readonly MonsterBook monsterBook;
        private readonly string imgExtension;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourcesPath"></param>
        /// <exception cref="Exception"></exception>
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
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public WzImage GetWzImageFromName(string name) {
            IEnumerable<WzImage> wzImageNodes = wzNode.Nodes.Where(nd => nd.Tag is WzImage).Select(nd => (WzImage)nd.Tag);
            WzImage wzImage;
            try {
                IEnumerable<string> idList = monsterBook.GetIdsFromName(name);
                wzImage = null;
                idList.ForEach(id => {
                    if(wzImageNodes.Select(nd => nd.Name).Contains(id + imgExtension) && wzImage == null) {
                        IEnumerable<WzImage> imgs = wzImageNodes.Where(nd => nd.Name == id + imgExtension).Where(nd => nd.WzProperties.Count > 0);
                        if(imgs.Count() > 0) {
                            wzImage = imgs.ElementAt(0);
                        }
                    }
                });
            } catch(Exception e) {
                throw e;
            }
            return wzImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public WzImage GetWzImageFromId(string id) {
            IEnumerable<WzImage> wzImageNodes = wzNode.Nodes.Where(nd => nd.Tag is WzImage).Select(nd => (WzImage)nd.Tag);
            WzImage wzImage;
            try {
                wzImage = null;
                IEnumerable<WzImage> imgs = wzImageNodes.Where(nd => nd.Name == id + imgExtension).Where(nd => nd.WzProperties.Count > 0);
                if(imgs.Count() > 0) {
                    wzImage = imgs.ElementAt(0);
                }
            } catch(Exception e) {
                throw e;
            }
            return wzImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wzImage"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetNameFromId(string id) {
            return monsterBook.GetNameFromId(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<string> GetIdsFromName(string name) {
            return monsterBook.GetIdsFromName(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<string> GetNamesFromVagueName(string name) {
            return monsterBook.GetNamesFromVagueName(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elemensts"></param>
        /// <param name="saveRoot"></param>
        /// <param name="dirName"></param>
        /// <exception cref="Exception"></exception>
        public void BuildAPNGs(Dictionary<string, List<AnimationFrame>> elemensts, string saveRoot, string dirName) {
            // APNGの出力
            try {
                FrameEditor frameEditor = new FrameEditor(elemensts);
                Dictionary<string, List<AnimationFrame>> newMaterials = frameEditor.EditPNGImages();
                APNGBuilder aPNGBuilder = new APNGBuilder(newMaterials);
                aPNGBuilder.BuildAnimations(saveRoot, dirName);
            } catch(Exception e) {
                throw e;
            }
        }
    }
}
