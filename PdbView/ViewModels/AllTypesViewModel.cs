using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
