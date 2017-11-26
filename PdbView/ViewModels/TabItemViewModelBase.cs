using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdbView.ViewModels {
    abstract class TabItemViewModelBase : BindableBase {
        private string _text, _icon;
        protected readonly MainViewModel MainViewModel;

        public string Text {
            get => _text;
            set => SetProperty(ref _text, value);       
        }

        public string Icon {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        protected TabItemViewModelBase(MainViewModel mainViewModel) {
            MainViewModel = mainViewModel;
        }
    }
}
