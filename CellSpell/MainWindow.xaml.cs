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
using HtmlAgilityPack;
using IronPython.Hosting;

namespace CellSpell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Pobierz(object sender, RoutedEventArgs e)
        {
            var url = "https://pl.wikipedia.org/wiki/Sejm_Czteroletni";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var pnodes = doc.DocumentNode.SelectNodes("//p/text()");
            foreach (var node in pnodes)
            {
                RawOutputBox.Items.Add(node.InnerHtml);
            }
        }

        private void Wykonaj(object sender, RoutedEventArgs e)
        {
            var script = PythonScriptBox.Text;
            var pyEngine = Python.CreateEngine();
            var scope = pyEngine.CreateScope();
            scope.SetVariable("input", RawOutputBox.Items);
            pyEngine.Execute(script, scope);
            ModifiedOutputBox.ItemsSource = scope.GetVariable("output");
            //ModifiedOutputBox.Items.Add(scope.GetVariable("output"));
        }
    }
}
