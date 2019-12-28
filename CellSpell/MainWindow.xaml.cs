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
            string code = @"
                using System;
                using System.Collections.Generic;
                using CellSpell.Models;

                namespace UserFunctions
                {                
                   
                    public class BinaryFunction
                    {                
                        public static double Function(List<String> input)
                        {
                            Result r = new Result();
                            function_body
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
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }

                throw new InvalidOperationException(sb.ToString());
            }

            Assembly assembly = results.CompiledAssembly;
            Type program = assembly.GetType("UserFunctions.BinaryFunction");
            MethodInfo function = program.GetMethod("Function");

            var betterFunction = (Func< List < String >, double>)Delegate.CreateDelegate(typeof(Func<List<String>, double>), function);

            List<String> input = new List<string>();
            foreach(var item in RawOutputBox.Items)
            {
                input.Add(item.ToString());
            }
            

            double result = betterFunction(input);
            Console.WriteLine(result);

            Result r = new Result();
        }
    }
}
