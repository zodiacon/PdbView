using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdbView.ViewModels {
    sealed class MemberViewModel {
        public string UdtType { get; }
        public MemberViewModel(string udttype) {
            UdtType = udttype;
        }

        public string Name { get; set; }
        public long Value { get; set; }
        public string Type { get; set; }
        public int Offset { get; set; }
        public bool IsNotBitField => IsEnum || !Type.Contains(" Bit ");
        public bool IsNotPointer => IsEnum || !Type.StartsWith("Ptr");
        public bool IsEnum => UdtType == "Enum";
        public bool IsStruct => UdtType == "Struct";
        public bool IsUnion => UdtType == "Union";

        public IEnumerable<MemberViewModel> SubItems {
            get {
                yield return new MemberViewModel("Struct") {
                    Name = "Dummy",
                    Type = "Dummy Type",
                   
                };
            }
        }
    }
}
