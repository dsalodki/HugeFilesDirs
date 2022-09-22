using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HugeFilesDirs
{
    internal class Item
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public List<Item> Items { get; set; }
    }
}
