using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Microsoft.CSharp;
using CellSpell.Models;
using System.Diagnostics;

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
            //var pnodes = doc.DocumentNode.SelectNodes("//p/text()");
            //foreach (var node in pnodes)
            //{
            //    RawOutputBox.Items.Add(node.InnerHtml);
            //}

            var anodes = doc.DocumentNode.SelectNodes("//a");
            foreach (var anode in anodes)
            {
                if (anode.Attributes["href"] != null)
                {
                    var baseUrl = new Uri(url);
                    var newUrl = new Uri(baseUrl, anode.Attributes["href"].Value);
                    if (baseUrl.Host == newUrl.Host && anode.Attributes["href"].Value.ToCharArray()[0] != '#' && anode.Attributes["href"].Value.ToCharArray()[0] != '&')
                    {
                        RawOutputBox.Items.Add(newUrl.AbsoluteUri);
                        //Debug.WriteLine(newUrl.AbsoluteUri);
                    }
                    else
                    {
                        //Debug.WriteLine("Wrong Host");
                    }


                }
                else
                {
                    //Debug.WriteLine("no href");
                }
                //Debug.WriteLine("**************************");

            }
        }

        private void Wykonaj(object sender, RoutedEventArgs e)
        {
            string code = @"
                using System;
                using System.Collections.Generic;
                using CellSpell.Models;
                using System.Linq;

                namespace UserFunctions
                {                
                   
                    public class BinaryFunction
                    {                
                        public static List<String> Function(List<String> input)
                        {
                            Result r = new Result();
                            function_body
                            return input;
                        }
                    }
                }
            ";
            code = code.Replace("function_body", PythonScriptBox.Text);
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            var assemblies = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .Where(a => !a.IsDynamic)
                            .Select(a => a.Location);
            parameters.ReferencedAssemblies.AddRange(assemblies.ToArray());

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.HasErrors)
            {
                List<String> sb = new List<string>();

                foreach (CompilerError error in results.Errors)
                {
                    sb.Add(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }
                ModifiedOutputBox.ItemsSource = sb;
                //throw new InvalidOperationException(sb.ToString());
            }
            else
            {
                Assembly assembly = results.CompiledAssembly;
                Type program = assembly.GetType("UserFunctions.BinaryFunction");
                MethodInfo function = program.GetMethod("Function");

                var betterFunction = (Func<List<String>, List<String>>)Delegate.CreateDelegate(typeof(Func<List<String>, List<String>>), function);

                List<String> input = new List<string>();
                foreach (var item in RawOutputBox.Items)
                {
                    input.Add(item.ToString());
                }


                List<String> result = betterFunction(input);
                ModifiedOutputBox.ItemsSource = null;
                ModifiedOutputBox.ItemsSource = result;
                Clipboard.SetText(result.First());
                //dodaj treeview
                //pobierz linki i dodaj do treeview
                //regex

                //input = input.Where(x => x.Contains("Sejm_Czteroletni")).Distinct().OrderBy(q => q).ToList();
            }
        }
    }
}
