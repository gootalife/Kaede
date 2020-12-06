using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaedeConsole {
    public class MonsterBook {
        private List<List<string>> table;
        Dictionary<string, string> idBook;
        Dictionary<string, List<string>> nameBook;
        public MonsterBook(List<List<string>> table) {
            this.table = table;
            idBook = new Dictionary<string, string>();
            nameBook = new Dictionary<string, List<string>>();
            RegisterIdAndName();
        }

        private void RegisterIdAndName() {
            table.ForEach(row => {
                if(!idBook.ContainsKey(row.ElementAt(0))) {
                    idBook.Add(row.ElementAt(0), row.ElementAt(1));
                }
            });
            table.ForEach(row => {
                if(nameBook.ContainsKey(row.ElementAt(1))) {
                    nameBook[row.ElementAt(1)].Add(row.ElementAt(0));
                } else {
                    nameBook.Add(row.ElementAt(1), new List<string> { row.ElementAt(0) });
                }
            });
        }

        public string GetNameFromId(string id) {
            return idBook.ContainsKey(id) ? idBook[id] : null;
        }

        public List<string> GetIdsFromName(string name) {
            return nameBook.ContainsKey(name) ? nameBook[name] : null;
        }
    }
}
