using DebugHelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdbView.ViewModels {
    class SymbolViewModel {
        public readonly SymbolInfo Symbol;

        public SymbolViewModel(in SymbolInfo symbol, string typename) {
            Symbol = symbol;
            Type = typename;

            switch (typename) {
                case "Struct":
                    Icon = "/icons/structure.ico";
                    break;

                case "Class":
                    Icon = "/icons/class.ico";
                    break;

                case "Enum":
                    Icon = "/icons/enum.ico";
                    break;

                case "Function":
                    Icon = "/icons/function.ico";
                    break;

                case "Union":
                    Icon = "/icons/union.ico";
                    break;

                default:
                    Icon = "/icons/generic.ico";
                    break;
            }
        }

        public string Name => Symbol.Name;
        public int Size => Symbol.Size;
        public int TypeIndex => Symbol.TypeIndex;
        public string Type { get; }
        public SymbolFlags Flags => Symbol.Flags;
        public ulong Address => Symbol.Address;
        public int Index => Symbol.Index;
        public string Icon { get; }
        public bool IsNotEnum => Type != "Enum";
        public bool IsEnum => !IsNotEnum;

        public IEnumerable<MemberViewModel> Members {
            get {
                var vm = MainViewModel.Instance;
                var sd = vm.SymbolHandler.BuildStructDescriptor(vm.BaseAddress, TypeIndex);
                if (IsEnum) {   // enum
                    foreach (var item in sd) {
                        yield return new MemberViewModel { Name = item.Name, Value = item.Value };
                    }
                }
                else {
                    // struct / class / union
                    foreach (var item in sd) {
                        yield return new MemberViewModel {
                            Name = item.Name,
                            Offset = item.Offset,
                            Type = vm.GetSymbolTypeName(item.TypeId)
                        };
                    }
                }
            }
        }
    }
}
