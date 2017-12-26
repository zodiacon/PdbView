using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Data;

namespace PdbView.ViewModels {
    sealed class AllSymbolsViewModel : TabItemViewModelBase {
        IEnumerable<SymbolViewModel> _symbols;

        public IEnumerable<SymbolViewModel> Symbols => _symbols;

        public AllSymbolsViewModel(MainViewModel mainViewModel, IEnumerable<SymbolViewModel> symbols) : base(mainViewModel) {
            _symbols = symbols;

            Text = "All Symbols";
            Icon = "/icons/symbols.ico";
        }

        public ICollectionViewAdv View { get; set; }

        string _filterText;
        public string FilterText {
            get => _filterText;
            set {
                if (SetProperty(ref _filterText, value)) {
                    if (string.IsNullOrWhiteSpace(value))
                        View.Filter = null;
                    else {
                        value = value.ToLower();
                        View.Filter = obj => {
                            var item = (SymbolViewModel)obj;
                            return item.Name.ToLower().Contains(value) || item.Type.ToLower().Contains(value);
                        };
                    }
                    View.RefreshFilter(true);
                }
            }
        }
    }
}
