using Kaede.Lib.Extensions;
using Kaede.Lib.Models;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Point = Kaede.Lib.Models.Point;

namespace Kaede.Lib {
    public class KaedeProcess {
        private readonly WzFile wzFile;
        private readonly WzFile stringWz;
        private readonly WzNode wzNode;
        private readonly WzImage stringImage;
        private const string imgExtension = ".img";
        private const string stringWzName = "String.wz";

        /// <summary>
        /// Kaedeメイン機能呼び出し用クラス
        /// </summary>
        /// <param name="mapleDir">MapleStoryのパス</param>
        /// <param name="target">{TARGET}.wzのパス</param>
        /// <exception cref="Exception"></exception>
        public KaedeProcess(string mapleDir, string target) {
            if (!File.Exists($@"{mapleDir}/{target}")) {
                throw new Exception($@"{mapleDir}/{target} doesn't exists.");
            }
            if (!File.Exists($@"{mapleDir}/{stringWzName}")) {
                throw new Exception($@"{mapleDir}/{stringWzName} doesn't exists.");
            }
            var wzFileManager = new WzFileManager();
            wzFile = wzFileManager.LoadWzFile($@"{mapleDir}/{target}", WzMapleVersion.BMS);
            stringWz = wzFileManager.LoadWzFile($@"{mapleDir}/{stringWzName}", WzMapleVersion.BMS);
            wzNode = new WzNode(wzFile);
            var stringNodeName = GetStringNodeName(target);
            stringImage = new WzNode(stringWz).Nodes
                .Where(node => node.Tag is WzImage)
                .Select(node => (WzImage)node.Tag)
                .FirstOrDefault(node => node.Name == stringNodeName + imgExtension);
        }

        /// <summary>
        /// ターゲット名によるStrinNode名の取得
        /// </summary>
        /// <param name="target">{TARGET}.wz</param>
        /// <returns></returns>
        private static string GetStringNodeName(string target) {
            // パスと数値を除去
            target = Regex.Replace(target, @"\d", "").Replace(".wz", "");
            string nodeName = target switch {
                "Mob" => target,
                _ => null,
            };
            return nodeName;
        }

        /// <summary>
        /// idからWzImageを取得
        /// </summary>
        /// <param name="id">モンスターID</param>
        /// <exception cref="Exception"></exception>
        /// <returns>WzImage</returns>
        public WzImage GetWzImageFromId(string id) {
            var wzImage = wzNode.Nodes
                .Where(node => node.Tag is WzImage)?
                .Select(node => (WzImage)node.Tag)?
                .FirstOrDefault(node => node.Name == id + imgExtension);
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
        public IEnumerable<AnimationFrame> GetAnimationFromPath(string id, string path) {
            var animation = new List<AnimationFrame>();
            var wzImage = wzNode.Nodes
                .Where(node => node.Tag is WzImage)?
                .Select(node => (WzImage)node.Tag)?
                .FirstOrDefault(img => img.Name == id + imgExtension);
            var imgProp = wzImage?.GetFromPath(path);
            if (imgProp.WzProperties is null) {
                throw new Exception("空のノードか無効なノードです。");
            }
            var animationName = imgProp.Name;
            foreach (var child in imgProp.WzProperties.Where(child => child is WzCanvasProperty || child is WzUOLProperty)) {
                WzCanvasProperty canvasProperty;
                Bitmap image;
                if (child is WzCanvasProperty || child is WzUOLProperty) {
                    if (child is WzCanvasProperty property1) {
                        canvasProperty = property1;
                        image = canvasProperty.GetLinkedWzCanvasBitmap();
                    } else {
                        var linkVal = ((WzUOLProperty)child).LinkValue;
                        if (linkVal is WzCanvasProperty property2) {
                            canvasProperty = property2;
                            image = canvasProperty.GetLinkedWzCanvasBitmap();
                        } else {
                            break;
                        }
                    }
                    var delay = canvasProperty[WzCanvasProperty.AnimationDelayPropertyName]?.GetInt();
                    if (delay is null) {
                        delay = 0;
                    }
                    var origin = canvasProperty.GetCanvasOriginPosition();
                    var animationFrame = new AnimationFrame(image, animationName, child.Name, new Point((int)origin.X, (int)origin.Y), (int)delay);
                    animation.Add(animationFrame);
                }
            }
            return animation;
        }

        /// <summary>
        /// 指定プロパティ以下のアニメーションのパスをすべて取得
        /// </summary>
        /// <param name="wzImageProperties">開始プロパティ</param>
        /// <exception cref="Exception"></exception>
        /// <returns>アニメーションのパス</returns>
        public IEnumerable<string> GetAnimationPaths(IEnumerable<WzImageProperty> wzImageProperties) {
            var list = new List<string>();
            foreach (var wzImageProperty in wzImageProperties) {
                // 子がWzCanvasPropertyかWzUOLPropertyを持つなら抽出
                if (wzImageProperty.WzProperties.OrEmptyIfNull().Where(child => child is WzCanvasProperty || child is WzUOLProperty).OrEmptyIfNull().Any()) {
                    var path = wzImageProperty.FullPath.Replace($"{wzImageProperty.ParentImage.FullPath}\\", "").Replace('\\', '/');
                    list.Add(path);
                }
                // 子がWzSubPropertyを持つなら下階層の探索を再帰的に継続
                if (wzImageProperty.WzProperties.OrEmptyIfNull().Where(child => child is WzSubProperty).OrEmptyIfNull().Any()) {
                    var paths = GetAnimationPaths(wzImageProperty.WzProperties.OrEmptyIfNull());
                    list.AddRange(paths);
                }
            }
            return list;
        }

        /// <summary>
        /// idから名前を取得
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="Exception"></exception>
        /// <returns>モンスター名</returns>
        public string GetNameFromId(string id) {
            var name = stringImage?.WzProperties?
                .FirstOrDefault(prop => prop.Name == id)?.WzProperties?
                .FirstOrDefault(prop => prop.Name == "name")?.WzValue as string;
            return name;
        }

        /// <summary>
        /// 名前からidを取得
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        /// <returns>モンスターID</returns>
        public IEnumerable<string> GetIdsFromName(string name) {
            var ids = stringImage?.WzProperties?
                .Where(prop => prop.WzProperties?.FirstOrDefault(p => p.Name == "name")?.WzValue as string == name)?
                .Select(prop => prop.Name);
            return ids;
        }

        /// <summary>
        /// 名前の一部からそれを含む名前を取得
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        /// <returns>引数の文字列を含むモンスター名のコレクション</returns>
        public IEnumerable<string> GetNamesFromPartialName(string name) {
            var names = stringImage?.WzProperties?
                .Select(prop => (prop.WzProperties?.FirstOrDefault(p => p.Name == "name")?.WzValue as string))?
                .Distinct()?
                .Where(prop => prop.Contains(name));
            return names;
        }

        /// <summary>
        /// APNGを生成
        /// </summary>
        /// <param name="animationName">アニメーション名</param>
        /// <param name="animation">画像のコレクション</param>
        /// <param name="rate">画像サイズの倍率</param>
        /// <param name="savePath">保存先パス</param>
        /// <exception cref="Exception"></exception>
        public static void BuildAPNG(string animationName, IEnumerable<AnimationFrame> animation, byte rate, string savePath) {
            var frameEditor = new FrameEditor(animationName, animation);
            var (frames, animInfo) = frameEditor.EditPNGImages(rate);
            var aPNGBuilder = new APNGBuilder(frames, animInfo);
            aPNGBuilder.BuildAnimation(savePath);
        }
    }
}
