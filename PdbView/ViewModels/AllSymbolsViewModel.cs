using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugHelp;

namespace PdbView.ViewModels {
    sealed class AllSymbolsViewModel : TabItemViewModelBase {
        IEnumerable<SymbolViewModel> _symbols;

        public IEnumerable<SymbolViewModel> Symbols => _symbols;

        public AllSymbolsViewModel(MainViewModel mainViewModel, IEnumerable<SymbolViewModel> symbols) : base(mainViewModel) {
            _symbols = symbols;

            Text = "All Symbols";
            Icon = "/icons/symbols.ico";
        }

    }
}
