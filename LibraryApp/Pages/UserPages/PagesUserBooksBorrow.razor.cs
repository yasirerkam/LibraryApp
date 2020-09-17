using Blazor.FileReader;
using LibraryApp.Data.Library;
using LibraryApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApp.Pages.UserPages
{
    public partial class PagesUserBooksBorrow
    {
        public async Task Borrow(Book book)
        {
            if (!Service.IsContainsUserBook(book.BookId) && Service.GetUserBook(AppDataService.Instance.userSigned.UserId).Count < 3 && !Service.HaveUserExpiredBook(AppDataService.Instance.userSigned.UserId))
            {
                UserBook userBook = new UserBook();
                userBook.UserId = AppDataService.Instance.userSigned.UserId;
                userBook.BookId = book.BookId;
                userBook.DateLent = DateTime.Now;
                userBook.DateReturn = DateTime.Now.AddDays(7);

                await Service.CreateUserBookAsync(userBook);

                myBooks = await Service.GetUserBookAsync(AppDataService.Instance.userSigned.UserId);

                this.StateHasChanged();
            }
        }

        public async Task Return(int BookId)
        {
            await Service.DeleteUserBookAsync(BookId);

            myBooks = await Service.GetUserBookAsync(AppDataService.Instance.userSigned.UserId);

            this.StateHasChanged();
        }

        #region Database

        public List<Book> books;
        public dynamic myBooks;
        private string searchBookName, searchIsbn;

        public string SearchBookName
        {
            get { return searchBookName; }
            set
            {
                searchBookName = value;
            }
        }
        public string SearchIsbn
        {
            get { return searchIsbn; }
            set
            {
                searchIsbn = value;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            books = Service.GetBooks();
        }

        protected override async Task OnInitializedAsync()
        {
            myBooks = await Service.GetUserBookAsync(AppDataService.Instance.userSigned.UserId);
        }

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
            // Make new forecast
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

            // A new forecast will have the Id set to 0
            if (addingNew)
            {
                // Create new forecast
                Book bookNew = new Book();
                bookNew.BookName = objBook.BookName;
                bookNew.Isbn = objBook.Isbn;
                bookNew.Author = objBook.Author;

                // Save the result
                var result = Service.CreateBookAsync(bookNew);
            }
            else
            {
                // This is an update
                var result = Service.UpdateBookAsync(objBook);
            }
            // Get the forecasts for the current book
            books = await Service.GetBooksAsync();

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

            // Delete the forecast
            var result = Service.DeleteBookAsync(book);

            // Get the forecasts for the current book
            books = await Service.GetBooksAsync();

            this.StateHasChanged();
        }

        public async Task Search()
        {
            if (!String.IsNullOrEmpty(searchBookName) && String.IsNullOrEmpty(searchIsbn))
            {
                books = await Service.FindBooksWithNameAsync(searchBookName);
            }
            else if (String.IsNullOrEmpty(searchBookName) && !String.IsNullOrEmpty(searchIsbn))

            {
                books = await Service.FindBooksWithIsbnAsync(searchIsbn);
            }
            else if (!String.IsNullOrEmpty(searchBookName) && !String.IsNullOrEmpty(searchIsbn))
            {
                books = await Service.FindBooksAsync(searchBookName, searchIsbn);
            }
            else
            {
                books = await Service.GetBooksAsync();
            }

            this.StateHasChanged();
        }

        public async Task OnSearchBookNameInputChange(ChangeEventArgs args)
        {
            searchBookName = args.Value.ToString();
            await Search();
        }

        public async Task OnSearchIsbnInputChange(ChangeEventArgs args)
        {
            searchIsbn = args.Value.ToString();
            await Search();
        }

        #endregion Database
    }
}