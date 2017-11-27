using DebugHelp;
using Prism.Commands;
using Prism.Mvvm;
using Syncfusion.UI.Xaml.TreeGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace PdbView.ViewModels {
    sealed class SymbolViewModel : BindableBase {
        public readonly SymbolInfo Symbol;

        public SymbolViewModel(in SymbolInfo symbol, string typename, int indent = 0) {
            Indent = indent;
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
        public int Indent { get; }

        IList<MemberViewModel> _members;
        public IList<MemberViewModel> Members {
            get {
                if (_members != null)
                    return _members;

                _members = new List<MemberViewModel>(8);
                var vm = MainViewModel.Instance;
                var sd = vm.SymbolHandler.BuildStructDescriptor(vm.BaseAddress, TypeIndex);
                if (IsEnum) {   // enum
                    foreach (var item in sd) {
                        _members.Add( new MemberViewModel(Type, Indent + 1) { Name = item.Name, Value = item.Value });
                    }
                }
                else {
                    // struct / class / union
                    foreach (var item in sd) {
                        _members.Add(new MemberViewModel(vm.SymbolHandler.GetSymbolUdtKind(vm.BaseAddress, vm.SymbolHandler.GetSymbolType(vm.BaseAddress, item.TypeId)).ToString(), Indent + 1) {
                            Name = item.Name,
                            Offset = item.Offset,
                            Type = vm.GetSymbolTypeName(item.TypeId),
                        });
                    }
                }
                return _members;
            }
        }

        public event Action<Predicate<object>> FilterChanged;

        string _filterText;
        public string FilterText {
            get => _filterText;
            set {
                if (SetProperty(ref _filterText, value)) {
                    if (string.IsNullOrWhiteSpace(value))
                        FilterChanged?.Invoke(null);
                    else {
                        value = value.ToLower();
                        FilterChanged?.Invoke(obj => {
                            var item = (MemberViewModel)obj;
                            return item.Name.ToLower().Contains(value) || item.Type.ToLower().Contains(value);
                        });
                    }
                }
            }
        }

    }
}
