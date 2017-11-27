using Syncfusion.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PdbView.ViewModels {
    sealed class AllTypesViewModel : TabItemViewModelBase {
        public IEnumerable<SymbolViewModel> Types { get; }

        public AllTypesViewModel(MainViewModel mainViewModel, IEnumerable<SymbolViewModel> types) : base(mainViewModel) {
            Types = types;

            Text = "Types";
            Icon = "/icons/types.ico";
        }

        SymbolViewModel _selectedType;
        public SymbolViewModel SelectedType {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
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
                            return item.Name.ToLower().Contains(value);
                        };
                    }
                    View.RefreshFilter(true);
                }
            }
        }


    }
}
