using PdbView.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PdbView.Views {
    /// <summary>
    /// Interaction logic for TypeDetailsView.xaml
    /// </summary>
    public partial class TypeDetailsView : UserControl {
        public TypeDetailsView() {
            InitializeComponent();

            DataContextChanged += (s, e) => {
                var oldvm = (SymbolViewModel)e.OldValue;
                if (oldvm != null) {
                    oldvm.FilterChanged -= OnFilterChanged;
                }
                var vm = (SymbolViewModel)e.NewValue;
                if (vm != null) {
                    vm.FilterChanged += OnFilterChanged;
                }
            };
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
        }

        private void OnFilterChanged(Predicate<object> filter) {
            var view = _treegrid.View;
            if (view != null) {
                view.Filter = filter;
                view.RefreshFilter();
            }
        }
    }
}
