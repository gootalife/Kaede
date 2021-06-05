using Kaede.Lib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Lib.Models {
    public class MonsterBook {
        private readonly IEnumerable<IEnumerable<string>> table;
        private readonly Dictionary<string, string> idBook;
        private readonly Dictionary<string, List<string>> nameBook;
        public MonsterBook(IEnumerable<IEnumerable<string>> table) {
            this.table = table;
            idBook = new Dictionary<string, string>();
            nameBook = new Dictionary<string, List<string>>();
            Register();
        }

        private void Register() {
            foreach(var row in table) {
                if(!idBook.ContainsKey(row.ElementAt(0))) {
                    idBook.Add(row.ElementAt(0), row.ElementAt(1));
                }
            }
            foreach(var row in table) {
                if(nameBook.ContainsKey(row.ElementAt(1))) {
                    nameBook[row.ElementAt(1)].Add(row.ElementAt(0));
                } else {
                    nameBook.Add(row.ElementAt(1), new List<string> { row.ElementAt(0) });
                }
            }
        }

        public string GetNameFromId(string id) {
            return idBook.ContainsKey(id) ? idBook[id] : null;
        }

        public IEnumerable<string> GetIdsFromName(string name) {
            return nameBook.ContainsKey(name) ? nameBook[name] : new List<string>();
        }

        public IEnumerable<string> GetNamesFromVagueName(string name) {
            return nameBook.Keys.Where(key => key.Contains(name));
        }
    }
}
