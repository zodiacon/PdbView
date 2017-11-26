using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdbView.ViewModels {
    sealed class MemberViewModel {
        public string Name { get; set; }
        public long Value { get; set; }
        public string Type { get; set; }
        public int Offset { get; set; }
    }
}
