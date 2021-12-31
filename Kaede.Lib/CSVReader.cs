using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Lib {
    public static class CSVReader {
        /// <summary>
        /// CSVを同期で読み込む
        /// </summary>
        /// <param name="path">csvファイルのパス</param>
        /// <param name="removeHeader">最初の1行を除去するかどうか</param>
        /// <returns>表データ</returns>
        public static IEnumerable<IEnumerable<string>> ReadCSV(string path, bool removeHeader = false) {
            var csv = new List<IEnumerable<string>>();
            using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
            if (removeHeader) {
                sr.ReadLine();
            }
            while (!sr.EndOfStream) {
                string line = sr.ReadLine();
                IEnumerable<string> row = line.Split(',').Cast<string>();
                csv.Add(row);
            }
            return csv;
        }

        /// <summary>
        /// CSVを非同期で読み込む
        /// </summary>
        /// <param name="path">csvファイルのパス</param>
        /// <param name="removeHeader">最初の1行を除去するかどうか</param>
        /// <returns>表データ</returns>
        public static async Task<IEnumerable<IEnumerable<string>>> ReadCSVAsync(string path, bool removeHeader = false) {
            var csv = new List<IEnumerable<string>>();
            using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
            if (removeHeader) {
                await sr.ReadLineAsync();
            }
            while (!sr.EndOfStream) {
                string line = await sr.ReadLineAsync();
                IEnumerable<string> row = line.Split(',').Cast<string>();
                csv.Add(row);
            }
            return csv;
        }
    }
}
