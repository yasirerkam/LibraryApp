using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using LibraryApp.Data;
using LibraryApp.Data.Library;
using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using LibraryApp.Services;

namespace LibraryApp.Pages.AdminPages
{
    public partial class PageBook
    {
        protected override async Task OnInitializedAsync()
        {
            books = await @Service.GetBooksAsync();
        }

        private string bookNameAdding = "Book Name", bookAuthorAdding = "Book Author";
        private List<string> resultISBN = new List<string>();

        #region file uploadn and ocr

        private List<string> imageName = new List<string>();
        private List<string> ocrResult = new List<string>();

        private ElementReference dropTargetElement;
        private ElementReference dropTargetInput;
        private IFileReaderRef dropReference;
        private IFileReaderRef ipReference;
        private bool Additive { get; set; }

        private const string dropTargetDragClass = "droptarget-drag";
        private const string dropTargetClass = "droptarget";

        private List<string> _dropClasses = new List<string>() { dropTargetClass };
        private List<string> _ipdropClasses = new List<string>() { dropTargetClass };

        private string DropClass => string.Join(" ", _dropClasses);
        private string IpDropClass => string.Join(" ", _ipdropClasses);

        private string Output { get; set; }
        private List<IFileInfo> DropFileList
        { get; } = new List<IFileInfo>
                ();
        private List<IFileInfo> IpFileList
        { get; } = new List<IFileInfo>
                ();

        protected override async Task OnAfterRenderAsync(bool isFirstRender)
        {
            if (isFirstRender)
            {
                dropReference = fileReaderService.CreateReference(dropTargetElement);
                ipReference = fileReaderService.CreateReference(dropTargetInput);
                await dropReference.RegisterDropEventsAsync();
            }
        }

        public async Task OnAdditiveChange(ChangeEventArgs e)
        {
            Additive = (bool)e.Value;
            StateHasChanged();
            await dropReference.UnregisterDropEventsAsync();
            await dropReference.RegisterDropEventsAsync(Additive);
        }

        public async Task ClearDrop()
        {
            await dropReference.ClearValue();
            await this.RefreshDropFileList();
        }

        public async Task ClearClick()
        {
            await ipReference.ClearValue();
            await this.RefreshIpFileList();

            imageName = new List<string>
                ();
            ocrResult = new List<string>
                ();
            Output = "";
        }

        public void OnDragEnter(EventArgs e)
        {
            _dropClasses.Add(dropTargetDragClass);
        }

        public void OnDragLeave(EventArgs e)
        {
            _dropClasses.Remove(dropTargetDragClass);
        }

        public void OnIpDragEnter(EventArgs e)
        {
            _ipdropClasses.Add(dropTargetDragClass);
        }

        public void OnIpDragLeave(EventArgs e)
        {
            _ipdropClasses.Remove(dropTargetDragClass);
        }

        public async Task OnDrop(EventArgs e)
        {
            Output += "Dropped a file.";
            _dropClasses.Remove(dropTargetDragClass);
            this.StateHasChanged();
            await this.RefreshDropFileList();
        }

        public async Task OnInputChange(EventArgs e)
        {
            Output += "Dropped a file on the Clickable.";
            _ipdropClasses.Remove(dropTargetDragClass);
            this.StateHasChanged();
            await this.RefreshIpFileList();
        }

        private Task RefreshDropFileList()
        {
            return RefreshFileList(dropReference, DropFileList);
        }

        private Task RefreshIpFileList()
        {
            return RefreshFileList(ipReference, IpFileList);
        }

        private async Task RefreshFileList(IFileReaderRef reader, List<IFileInfo> list)
        {
            list.Clear();
            foreach (var file in await reader.EnumerateFilesAsync())
            {
                var fileInfo = await file.ReadFileInfoAsync();
                list.Add(fileInfo);
            }
            this.StateHasChanged();
        }

        public async Task ReadDrop()
        {
            await ReadFile(dropReference);
        }

        public async Task ReadClick()
        {
            await ReadFile(ipReference);
        }

        public async Task ReadFile(IFileReaderRef list)
        {
            imageName.Clear();
            DeleteFiles(AppDataService.Instance.pathImageDirectory);

            Output = string.Empty;
            this.StateHasChanged();
            var nl = Environment.NewLine;

            foreach (var file in await list.EnumerateFilesAsync())
            {
                var fileInfo = await file.ReadFileInfoAsync();
                Output += $"{nameof(IFileInfo)}.{nameof(fileInfo.Name)}: {fileInfo.Name}{nl}";
                Output += $"{nameof(IFileInfo)}.{nameof(fileInfo.Size)}: {fileInfo.Size}{nl}";
                Output += $"{nameof(IFileInfo)}.{nameof(fileInfo.Type)}: {fileInfo.Type}{nl}";
                Output += $"{nameof(IFileInfo)}.{nameof(fileInfo.LastModifiedDate)}: {fileInfo.LastModifiedDate?.ToString() ?? "(N/A)"}{nl}";
                Output += $"Reading file...";
                this.StateHasChanged();

                using (var fs = await file.OpenReadAsync())
                {
                    var bufferSize = 20480;
                    var buffer = new byte[bufferSize];
                    int count;
                    while ((count = await fs.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        Output += $"Read {count} bytes. {fs.Position} / {fs.Length}{nl}";
                        this.StateHasChanged();
                    }
                    Output += $"Done reading file {fileInfo.Name}{nl}.";
                }

                using (var ms = await file.CreateMemoryStreamAsync(2048))
                {
                    imageName.Add("file_" + DateTime.Now.ToFileTime().ToString() + "." + fileInfo.Type.Split('/')[1]);
                    string path = Path.Combine(AppDataService.Instance.pathImageDirectory, imageName.Last());
                    FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 2048, true);

                    ms.WriteTo(fileStream);

                    fileStream.Close();
                    ms.Close();
                }
            }

            this.StateHasChanged();

            Tesseract.DoAfterRecognise += ShowResults;
            Tesseract.DoAfterRecognise += AddBookAfterResult;

            Tesseract.Recognise();
        }

        public void ShowResults()
        {
            AppDataService.Instance.TessTextList.ForEach(text => ocrResult.Add(text));
            Console.WriteLine(AppDataService.Instance.TessTextList.Count);
            Console.WriteLine(ocrResult.Count);

            Tesseract.DoAfterRecognise -= ShowResults;
            InvokeAsync(StateHasChanged);
        }

        private void AddBook(string bookName, string iSBN, string author)
        {
            Book bookNew = new Book();
            bookNew.BookName = bookName;
            bookNew.Isbn = iSBN;
            bookNew.Author = author;

            Service.CreateBookAsync(bookNew);
        }

        public void AddBookAfterResult()
        {
            foreach (string tessText in AppDataService.Instance.TessTextList)
            {
                string isbn = AppDataService.Instance.ExtractISBN(tessText) ?? "";
                resultISBN.Add(isbn);

                Console.WriteLine("AddBookAfterResult: ", tessText);
                Console.WriteLine("AddBookAfterResult: ", isbn);
                AddBook(bookNameAdding, isbn, bookAuthorAdding);
            }
            Tesseract.DoAfterRecognise -= AddBookAfterResult;
        }

        public void DeleteFiles(string path)
        {
            foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
            {
                file.Delete();
            }
        }

        #endregion file uploadn and ocr

        #region Database

        public List<Book> books;

        private bool addingNew = false;
        private Book objBook = new Book();
        private bool ShowPopup = false;

        private void ClosePopup()
        {
            addingNew = false;
            // Close the Popup
            ShowPopup = false;
            this.StateHasChanged();
        }

        private void AddNew()
        {
            // Make new book
            objBook = new Book();

            addingNew = true;

            // Open the Popup
            ShowPopup = true;

            this.StateHasChanged();
        }

        private async Task Save()
        {
            // Close the Popup
            ShowPopup = false;

            // A new book will have the Id set to 0
            if (addingNew)
            {
                AddBook(objBook.BookName, objBook.Isbn, objBook.Author);
            }
            else
            {
                // This is an update
                var result = @Service.UpdateBookAsync(objBook);
            }
            // Get the books for the current book
            books = await @Service.GetBooksAsync();

            this.StateHasChanged();
        }

        private void Edit(Book book)
        {
            // Set the selected book as the current book
            objBook = book;

            // Open the Popup
            ShowPopup = true;

            this.StateHasChanged();
        }

        private async Task Delete(Book book)
        {
            // Close the Popup
            ShowPopup = false;

            // Delete the book
            var result = @Service.DeleteBookAsync(book);

            // Get the books for the current book
            books = await @Service.GetBooksAsync();

            this.StateHasChanged();
        }

        #endregion Database
    }
}