using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibraryApp.Pages;

namespace LibraryApp.Services
{
    public class Tesseract
    {
        public string Text { get; set; }
        public Action DoAfterRecognise;

        public void Recognise()
        {
            AppDataService.Instance.TessTextList.Clear();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var solutionDirectory = TryGetSolutionDirectoryInfo()?.FullName;
            if (solutionDirectory == null)
            {
                Console.WriteLine("Could not find the solution folder.");
                return;
            }

            var tesseractPath = solutionDirectory + @"\tes_294687";
            //var testFiles = Directory.EnumerateFiles(solutionDirectory + @"\LibraryApp\wwwroot\Images\");
            var testFiles = Directory.EnumerateFiles(AppDataService.Instance.pathImageDirectory);

            Task.Factory.StartNew(() =>
            {
                var maxDegreeOfParallelism = Environment.ProcessorCount;
                Parallel.ForEach(testFiles, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (fileName) =>
                {
                    var imageFile = File.ReadAllBytes(fileName);
                    Text = ParseText(tesseractPath, imageFile, "eng", "tur");
                    Console.WriteLine("File:" + fileName + "\n" + Text + "\n");

                    AppDataService.Instance.TessTextList.Add(Text);
                });
            }).ContinueWith(_ => Task.Delay(1000))
               .Unwrap()
            .ContinueWith((antecedent) =>
            {
                stopwatch.Stop();
                Console.WriteLine("Duration: " + stopwatch.Elapsed);

                DoAfterRecognise();
            }).
            Wait();
        }

        private string ParseText(string tesseractPath, byte[] imageFile, params string[] lang)
        {
            string output = string.Empty;
            var tempOutputFile = Path.GetTempPath() + Guid.NewGuid();
            var tempImageFile = Path.GetTempFileName();

            try
            {
                File.WriteAllBytes(tempImageFile, imageFile);

                ProcessStartInfo info = new ProcessStartInfo();
                info.WorkingDirectory = tesseractPath;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.UseShellExecute = false;
                info.FileName = "cmd.exe";
                info.Arguments =
                    "/c tesseract.exe " +
                    // Image file.
                    tempImageFile + " " +
                    // Output file (tesseract add '.txt' at the end)
                    tempOutputFile +
                    // Languages.
                    " -l " + string.Join("+", lang);

                // Start tesseract.
                Process process = Process.Start(info);
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    // Exit code: success.
                    output = File.ReadAllText(tempOutputFile + ".txt");
                }
                else
                {
                    throw new Exception("Error. Tesseract stopped with an error code = " + process.ExitCode);
                }
            }
            finally
            {
                File.Delete(tempImageFile);
                File.Delete(tempOutputFile + ".txt");
            }

            return output;
        }

        private DirectoryInfo TryGetSolutionDirectoryInfo()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
                directory = directory.Parent;
            return directory;
        }
    }
}