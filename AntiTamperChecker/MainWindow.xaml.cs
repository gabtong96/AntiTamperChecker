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
using Ookii.Dialogs.Wpf;
using Ookii.Dialogs;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.Windows.Forms;

namespace AntiTamperChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        string pdfname;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Browse_Files(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaOpenFileDialog filedlg = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();

            if (filedlg.ShowDialog(this).GetValueOrDefault())
            {
                pdfname = filedlg.FileName;
            }

            if (!string.IsNullOrEmpty(pdfname))
            {
                Debug.AppendText("Selected file is " + Get_File_Name(pdfname) + "\n");
            }
        }

        private string Get_File_Name (string fullname)
        {
            string[] filename = fullname.Split('\\');
            return filename[filename.Length-1];
        }

        private void Validate(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pdfname))
            {
                Debug.AppendText("No PDF file is selected\n");
                return;
            }

            MD5HashGenerator mD5Hash = new MD5HashGenerator();
            string generatedhash = mD5Hash.GenerateHash(pdfname);
            
            string coverpage = pdfname.Substring(0, pdfname.Length-4) + "_CP.pdf";
            string keyword;

            try
            {
                PdfDocument pdfDocument = PdfReader.Open(coverpage);
                keyword = pdfDocument.Info.Keywords;
                if (keyword == generatedhash)
                {
                    Debug.AppendText("File " + Get_File_Name(pdfname) + " has been validated.\n");
                }
                else
                {
                    TextRange tr = new TextRange(Debug.Document.ContentEnd, Debug.Document.ContentEnd);
                    tr.Text = "File " + Get_File_Name(pdfname) + " has been tampered with.\n";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                    tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);      
                }
            }
            catch
            {
                Debug.AppendText(Get_File_Name(pdfname).Substring(0, Get_File_Name(pdfname).Length-4) + "_CP.pdf can't be found.\n");
            }
            
        }

        private void About_Page(object sender, RoutedEventArgs e)
        {
            InfoPage info = new InfoPage();
            info.Owner = this;
            info.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            info.ShowDialog();
        }
        private void Clear_Box(object sender, RoutedEventArgs e)
        {
            Debug.Document.Blocks.Clear();
            Debug.AppendText("\n");
        }
        private void Restore_Defaults(object sender, RoutedEventArgs e)
        {
            pdfname = "";
            Debug.AppendText("Values restored to default.");
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Scroll_To_End(object sender, TextChangedEventArgs e)
        {
            Debug.ScrollToEnd();
        }
    }
}
