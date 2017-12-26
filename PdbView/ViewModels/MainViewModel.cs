using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Zodiacon.DebugHelp;
using Zodiacon.WPF;

namespace PdbView.ViewModels {
    class MainViewModel : BindableBase {
        Dictionary<int, string> _cache = new Dictionary<int, string>(256);

        public readonly IUIServices UI;
        public SymbolHandler SymbolHandler { get; private set; }
        public ulong BaseAddress { get; private set; }
        ObservableCollection<TabItemViewModelBase> _tabItems = new ObservableCollection<TabItemViewModelBase>();
        public IReadOnlyList<SymbolViewModel> Symbols { get; private set; }
        public IReadOnlyList<SymbolViewModel> Types { get; private set; }
        public AllTypesViewModel AllTypes { get; private set; }

        string _filename;
        public string FileName {
            get => _filename;
            set => SetProperty(ref _filename, value);
        }

        public IList<TabItemViewModelBase> TabItems => _tabItems;

        public static MainViewModel Instance { get; private set; }

        public MainViewModel(IUIServices ui) {
            UI = ui;
            Instance = Instance == null ? this : throw new InvalidOperationException();
        }

        public ICommand OpenCommand => new DelegateCommand(async() => await OpenFileInternal());
        public ICommand ExitCommand => new DelegateCommand(() => Application.Current.Shutdown());

        TabItemViewModelBase _selectedItem;
        public TabItemViewModelBase SelectedItem {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private async Task OpenFileInternal() {
            var filename = UI.FileDialogService.GetFileForOpen(
                "All supported files|*.pdb;*.exe;*.dll;*.sys;*.ocx|Pdb Files|*.pdb|Image files|*.exe;*.dll;*.sys;*.ocx|All Files|*.*", "Select File to Load");
            if (filename == null)
                return;

            try {
                var handler = SymbolHandler.Create();
                var isPdb = Path.GetExtension(filename).Equals(".pdb", StringComparison.InvariantCultureIgnoreCase);
                BaseAddress = await handler.TryLoadSymbolsForModuleAsync(filename, isPdb ? 0x1000000UL : 0);
                SymbolHandler?.Dispose();
                SymbolHandler = handler;
                TabItems.Clear();

                _cache.Clear();

                Symbols = SymbolHandler.EnumSymbols(BaseAddress).Select(sym => new SymbolViewModel(sym, GetTypeName(sym))).ToList();
                Types = SymbolHandler.EnumTypes(BaseAddress).Select(sym => new SymbolViewModel(sym, GetTypeName(sym))).ToList();

                var allSymbols = new AllSymbolsViewModel(this, Symbols.Concat(Types).OrderBy(sym => sym.Name));
                TabItems.Add(allSymbols);

                var allTypes = new AllTypesViewModel(this, Types);
                AllTypes = allTypes;
                TabItems.Add(allTypes);

                SelectedItem = TabItems[0];

                RaisePropertyChanged(nameof(Symbols));
                RaisePropertyChanged(nameof(Types));

                FileName = filename;
            }
            catch (Exception ex) {
                UI.MessageBoxService.ShowMessage($"Error: {ex.Message}", App.Title);
            }
        }

        public string GetTypeName(SymbolInfo sym) {
            var tag = sym.Tag;
            if (tag != SymbolTag.UDT)
                return tag.ToString();

            var udt = SymbolHandler.GetSymbolUdtKind(BaseAddress, sym.TypeIndex);
            return udt.ToString();
        }

        static int PointerSize;

        public string GetSymbolTypeName(int index) {
            var handler = SymbolHandler;
            var address = BaseAddress;

            var symbol = SymbolInfo.Create();
            int typeIndex = handler.GetSymbolType(address, index);
            if (_cache.TryGetValue(typeIndex, out var typename))
                return typename;

            var tag = handler.GetSymbolTag(address, typeIndex);

            if (handler.GetSymbolFromIndex(address, typeIndex, ref symbol)) {
                if ((typename = handler.GetTypeInfoName(address, typeIndex)) != null) {
                    _cache.Add(typeIndex, typename);
                    return typename;
                }
            }

            string name = null;
            switch (tag) {
                case SymbolTag.PointerType:
                    var len = handler.GetSymbolLength(address, typeIndex);
                    if(PointerSize == 0)
                        PointerSize = (int)len;
                    name = GetSymbolTypeName(typeIndex);
                    if (name == null) {
                        var baseType = handler.GetSymbolBaseType(address, typeIndex);
                        baseType = baseType == BasicType.NoType ? BasicType.Void : baseType;
                        name = $"Ptr{len * 8} {GetBasicTypeNameFromBasicType(baseType)}";
                    }
                    else {
                        name = $"Ptr{len * 8} {name}";
                    }
                    break;

                case SymbolTag.BaseType:
                    name = GetBasicTypeNameFromBasicType(handler.GetSymbolBaseType(address, typeIndex));
                    var bit = handler.GetSymbolBitPosition(address, index);
                    if (bit >= 0) {
                        name += $" Bit: {bit} Len: {handler.GetSymbolLength(address, index)}";
                        break;
                    }
                    break;

                case SymbolTag.Data:
                    break;

                case SymbolTag.ArrayType:
                    var len2 = handler.GetSymbolLength(address, typeIndex);
                    var arrayType = handler.GetSymbolType(address, typeIndex);
                    
                    var sizeType = handler.GetSymbolBaseType(address, arrayType);
                    var size = GetBasicDataTypeSize(sizeType);
                    name = $"[{len2 / (uint)size}] {GetSymbolTypeName(typeIndex)}";
                    break;
            }

            return name;
        }

        public static readonly ICommand JumpToTypeCommand = new DelegateCommand<MemberViewModel>(member => {
            Instance.AllTypes.GotoType(member.Type);
        });

        private static int GetBasicDataTypeSize(BasicType type) {
            switch (type) {
                case BasicType.NoType:
                case BasicType.Void:
                    return PointerSize;

                case BasicType.Char:
                    return 1;

                case BasicType.WChar:
                    return 2;

                case BasicType.Variant:
                    return 16;

                case BasicType.Bool:
                    return 1;

                default:
                    return 4;
            }
        }

        static string GetBasicTypeNameFromBasicType(BasicType type) {
            switch (type) {
                case BasicType.UInt:
                case BasicType.ULong:
                    return "Uint4B";

                case BasicType.Int:
                case BasicType.Long:
                    return "Int4B";

                default:
                    return type.ToString();
            }
        }
    }
}
