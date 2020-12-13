using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Lib {
    public static class CSVReader {
        /// <summary>
        /// CSVの読み込み
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<string>> ReadCSV(string path, bool removeHeader = false) {
            List<List<string>> csv = new List<List<string>>();
            try {
                using(var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"))) {
                    if(removeHeader) {
                        sr.ReadLine();
                    }
                    while(!sr.EndOfStream) {
                        List<string> row = sr.ReadLine().Split(',').ToList();
                        csv.Add(row);
                    }
                }
            } catch(Exception e) {
                throw e;
            }
            return csv;
        }

        public static IEnumerable<IEnumerable<string>> ReadCSV(Stream stream, bool removeHeader = false) {
            List<List<string>> csv = new List<List<string>>();
            try {
                using(var sr = new StreamReader(stream, Encoding.GetEncoding("UTF-8"))) {
                    if(removeHeader) {
                        sr.ReadLine();
                    }
                    while(!sr.EndOfStream) {
                        List<string> row = sr.ReadLine().Split(',').ToList();
                        csv.Add(row);
                    }
                }
            } catch(Exception e) {
                throw e;
            }
            return csv;
        }
    }
}
