using HaRepacker;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KaedeConsole {
    public class Program {
        public static async Task Main(string[] args) {
            string wzFilePath = $@"{Directory.GetCurrentDirectory()}\wz\Mob.wz";
            string csvPath = $@"{Directory.GetCurrentDirectory()}\Resources\MonsterIdList.csv";
            string target = "ロムバード";
            string extension = ".img";
            if(!File.Exists(wzFilePath)) {
                Console.WriteLine($"{wzFilePath} is not exists.");
                return;
            }
            if(!File.Exists(csvPath)) {
                Console.WriteLine($"{csvPath} is not exists.");
                return;
            }

            //ID対応表を取得s
            Console.WriteLine($@"load MonsterIDList.csv from: {csvPath}");
            MonsterBook monsterBook = new MonsterBook(CSVReader.ReadCSV(csvPath, true));
            Console.WriteLine($@"MonsterIDList.csv was loaded.");
            List<string> idList = monsterBook.GetIdsFromName(target);
            WzImage wzImage = await Task.Run(() => {
                // wzのロード
                WzFileManager wzFileManager = new WzFileManager();
                Console.WriteLine($@"load wzfile from: {wzFilePath}");
                WzFile wz = wzFileManager.LoadWzFile(wzFilePath, WzMapleVersion.BMS);
                WzNode node = new WzNode(wz);
                Console.WriteLine($@"extracting items.");
                //IEnumerable<WzImage> wzImageNodes = node.Nodes.Cast<WzNode>().Where(nd => nd.Tag is WzImage).Select(nd => (WzImage)nd.Tag).Where(nd => nd.WzProperties.Count > 0);
                IEnumerable<WzImage> wzImageNodes = node.Nodes.Cast<WzNode>().Where(nd => nd.Tag is WzImage).Select(nd => (WzImage)nd.Tag);
                Console.WriteLine($@"{wzImageNodes.Count()} items were loaded.");

                //画像の抽出
                try {
                    wzImage = null;
                    List<string> targetIds = monsterBook.GetIdsFromName(target);
                    WzImage img = null;
                    targetIds.ForEach(id => {
                        if(wzImageNodes.Select(nd => nd.Name).Contains(id + extension) && img == null) {
                            IEnumerable<WzImage> imgs = wzImageNodes.Where(nd => nd.Name == id + extension).Where(nd => nd.WzProperties.Count > 0);
                            if(imgs.Count() > 0) {
                                img = wzImageNodes.Where(nd => nd.Name == id + extension).ElementAt(0);
                            }
                        }
                    });
                    wzImage = img;
                    //IDと名前の置き換え
                    wzImage.Name = monsterBook.GetNameFromId(wzImage.Name.Replace(extension, ""));
                } catch(Exception e) {
                    Console.WriteLine(e.Message);
                    wzImage = null;
                }
                return wzImage;
            });
            if(wzImage == null) {
                Console.WriteLine($"{target} is not exists or elements are nothing");
                return;
            }

            // アニメーション名と各フレームを取得
            Dictionary<string, List<AnimationFrame>> materials = new Dictionary<string, List<AnimationFrame>>();
            wzImage.WzProperties.Where(animation => animation is WzSubProperty && animation.Name != "info").ForEach(animation => {
                List<AnimationFrame> animationFrames = new List<AnimationFrame>();
                animation.WzProperties.Where(frame => frame is WzCanvasProperty).ForEach(frame => {
                    WzCanvasProperty canvasProperty = (WzCanvasProperty)frame;
                    Bitmap image = canvasProperty.GetLinkedWzCanvasBitmap();
                    int? delay = canvasProperty[WzCanvasProperty.AnimationDelayPropertyName]?.GetInt();
                    if(delay == null) {
                        delay = 0;
                    }
                    PointF origin = canvasProperty.GetCanvasOriginPosition();
                    AnimationFrame animationFrame = new AnimationFrame(image, animation.Name, frame.Name, new Point((int)origin.X, (int)origin.Y), (int)delay);
                    animationFrames.Add(animationFrame);
                });
                materials.Add(animation.Name, animationFrames);
            });

            // APNGの出力
            Console.WriteLine("start APNG build.");
            FrameEditor frameEditor = new FrameEditor(materials);
            Dictionary<string, List<AnimationFrame>> newMaterials = frameEditor.EditPNGImages();
            APNGBuilder aPNGBuilder = new APNGBuilder(newMaterials);
            aPNGBuilder.BuildAnimations($@"{Directory.GetCurrentDirectory()}\AnimatedPNGs", wzImage.Name);
            Console.WriteLine("APNG build was done.");
        }
    }
}
