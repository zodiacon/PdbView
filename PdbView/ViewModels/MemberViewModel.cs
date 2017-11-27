using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PdbView.ViewModels {
    sealed class MemberViewModel {
        public string UdtType { get; }
        public MemberViewModel(string udttype, int indent) {
            UdtType = udttype;
            Indent = new Thickness(indent * 4, 0, 0, 0);
        }

        public string Name { get; set; }
        public long Value { get; set; }
        public string Type { get; set; }
        public int Offset { get; set; }
        public bool IsNotBitField => IsEnum || !Type.Contains(" Bit: ");
        public bool IsNotPointer => IsEnum || !Type.StartsWith("Ptr");
        public bool IsEnum => UdtType == "Enum";
        public bool IsStruct => UdtType == "Struct";
        public bool IsUnion => UdtType == "Union";
        public Thickness Indent { get; set; }

        public IEnumerable<MemberViewModel> SubItems {
            get {
                if (IsEnum || !IsNotBitField || !IsNotPointer)
                    yield break;

                var vm = MainViewModel.Instance;
                var symbol = vm.Types.FirstOrDefault(sym => sym.Name == Type);
                if (symbol == null)
                    yield break;

                foreach (var member in symbol.Members) {
                    member.Indent = new Thickness(Indent.Left + 12, 0, 0, 0);
                    yield return member;
                }

            }
        }
    }
}
