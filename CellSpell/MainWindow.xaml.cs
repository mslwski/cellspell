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
            String txt = new String();
            foreach(var node in pnodes)
            {
                txt += node.InnerHtml;
            }

            RawOutputBox.Items.Add(txt);
        }
    }
}
