using FileCompare_Reforged.Annotations;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Prism.Mvvm;
using Prism.Commands;
using System.Windows;
using System.Windows.Threading;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using static FileCompare_Reforged.DownloadTimer;
using Settings = FileCompare_Reforged.Properties.Settings;

//using IWshRuntimeLibrary;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;

namespace FileCompare_Reforged.ViewModel
{

    class MainWindowVM : BindableBase
    {
        private readonly WebClient webclient = new WebClient();

        private static Properties.Settings settings = new Properties.Settings();

        public static string FilePath => settings.FilePath + @"\" + settings.FileName + settings.FileType;
        public static string AltFilePath => settings.FilePath + @"\" + settings.FileName + "Alt" + settings.FileType;

        private string _linkPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Filecompare.lnk";

        private FileInfo _finfo;

        public MainWindowVM()
        {
            webclient = new WebClient();

            _finfo = new FileInfo(string.IsNullOrWhiteSpace(FilePath) ? Path.GetDirectoryName(Path.GetDirectoryName(
                    System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase )))
                ?.Remove(0,6) : FilePath);

            
            Timer.Tick += (s, e) => { Main(); Timer.Start(); };
            //timer.Start();

            Main();

        }

        private void Main()
        {
            try
            {
                if (_finfo.Exists)
                {
                    _finfo = new FileInfo(AltFilePath);
                    if (_finfo.Exists)
                    {
                        FileReplaceAndClear(FilePath, AltFilePath);
                        webclient.DownloadFile(settings.Url, AltFilePath);
                        if (FileCompare(FilePath, AltFilePath) == false)
                        {
                            App.MainPage.Result = WordInFileSearch() ?
                                FileResult.FileHandlerResult.WithFindWord :
                                FileResult.FileHandlerResult.WithChanges;
                            //NewFilesNotification();
                        }
                        else
                        {
                            App.MainPage.Result = FileResult.FileHandlerResult.NoChanges;
                            //NoNewFilesNotification();
                        }
                    }
                    else
                    {
                        webclient.DownloadFile(settings.Url, AltFilePath);
                        if (FileCompare(FilePath, AltFilePath) == false)
                        {
                            App.MainPage.Result = WordInFileSearch() ?
                                FileResult.FileHandlerResult.WithFindWord :
                                FileResult.FileHandlerResult.WithChanges;
                        }
                        else
                        {
                            App.MainPage.Result = FileResult.FileHandlerResult.NoChanges;
                        }
                    }
                }
                else
                {
                    webclient.DownloadFile(settings.Url, FilePath);
                    //StateLabel.Text = "Файлы остутсвуют... скачивание файлов. Программа начнет сравнивать файлы при следующем запуске.";
                    App.MainPage.Result = FileResult.FileHandlerResult.NoFiles;
                }
            }
            catch
            {
                MiniDefault(); //Сброс некоторых настроек
                Main();
            }
        }

        private void MiniDefault()
        {
            var result =  Path.GetDirectoryName(Path.GetDirectoryName(
                System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase )))
                ?.Remove(0,6);
            settings = Settings.Default;
            settings.Url = "http://smolapo.ru/sites/default/files/Studentam/2020-2021/izm2020.docx";
            settings.FilePath = result;
            settings.Save();
        }
        private bool WordInFileSearch()
        {
            WordprocessingDocument wordDoc = WordprocessingDocument.Open(FilePath, true);
            Body body = wordDoc.MainDocumentPart.Document.Body;
            var qwe = body.ChildElements;
            string readText = null;
            foreach (var childElement in qwe)
            {
                var asdf = childElement.InnerText;
                System.IO.File.AppendAllText(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\text_of_Word_2.txt", '\n' + asdf);
                readText = System.IO.File.ReadAllText(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\text_of_Word_2.txt");
            }
            System.IO.File.Delete(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\text_of_Word_2.txt");
            wordDoc.Close();
            if (readText.IndexOf(settings.FindWord) != -1)
                return true;
            return false;

        }
        private void FileReplaceAndClear(string filename, string filenamealt)
        {
            try
            {
                System.IO.File.Delete(FilePath);
                System.IO.File.Move(AltFilePath, FilePath);
                System.IO.File.Delete(AltFilePath);
            }
            catch
            {
                // ignored
            }
        }

        private bool FileCompare(string file1, string file2)
        {
            int file1byte = 0;
            int file2byte = 0;
            FileStream fs1;
            FileStream fs2;
            try
            {
                // Determine if the same file was referenced two times.
                if (file1 == file2)
                {
                    // Return true to indicate that the files are the same.
                    return true;
                }

                // Open the two files.
                fs1 = new FileStream(file1, FileMode.Open);
                fs2 = new FileStream(file2, FileMode.Open);

                // Check the file sizes. If they are not the same, the files
                // are not the same.
                if (fs1.Length != fs2.Length)
                {
                    // Close the file
                    fs1.Close();
                    fs2.Close();

                    // Return false to indicate files are different
                    return false;
                }

                // Read and compare a byte from each file until either a
                // non-matching set of bytes is found or until the end of
                // file1 is reached.
                do
                {
                    // Read one byte from each file.
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                }
                while ((file1byte == file2byte) && (file1byte != -1));

                // Close the files.
                fs1.Close();
                fs2.Close();
            }
            catch
            {
                //MiniDefault(); //Сброс некоторых настроек
                //Application.Exit();
                FileCompare(file1, file2);
            }
            // Return the success of the comparison. "file1byte" is
            // equal to "file2byte" at this point only if the files are
            // the same.
            return ((file1byte - file2byte) == 0);
        }
    }
}
