using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryApp.Data.Library;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using LibraryApp.Data;

using LibraryApp.Data.Library;

namespace LibraryApp.Data
{
    public class LiraryService
    {
        private readonly LibraryContext ctx;

        public LiraryService(LibraryContext context)
        {
            ctx = context;
        }

        #region Read

        public async Task<List<Admin>> GetAdminsAsync()
        {
            return await ctx.Admins.ToListAsync();
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await ctx.Users.ToListAsync();
        }

        public List<Book> GetBooks()
        {
            return ctx.Books.ToList();
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            return await ctx.Books.ToListAsync();
        }

        public async Task<dynamic> GetUsersBookAsync()
        {
            return await (from ubj in (from u in ctx.Users join ub in ctx.UserBooks on u.UserId equals ub.UserId select new { User = u, UserBook = ub })
                          join b in ctx.Books on ubj.UserBook.BookId equals b.BookId
                          select new
                          {
                              ubj.User.UserId,
                              ubj.User.UserName,
                              ubj.User.Name,
                              ubj.User.Surname,
                              ubj.User.Email,
                              ubj.User.DateOfBirth,
                              ubj.User.Phone,
                              b.BookId,
                              b.BookName,
                              b.Isbn,
                              b.Author,
                              ubj.UserBook.DateLent,
                              ubj.UserBook.DateReturn,
                              ubj.UserBook.UserBookId
                          }).ToListAsync();
        }

        public dynamic GetUserBook(int userId)
        {
            return (from all in (from userUserBooks in (from users in ctx.Users join userBooks in ctx.UserBooks on users.UserId equals userBooks.UserId select new { User = users, UserBook = userBooks })
                                 join books in ctx.Books on userUserBooks.UserBook.BookId equals books.BookId
                                 select new { UserUserBooks = userUserBooks, Books = books })
                    where all.UserUserBooks.User.UserId == userId
                    select new { all.Books.BookId, all.Books.BookName, all.Books.Isbn, all.Books.Author, all.UserUserBooks.UserBook.DateLent, all.UserUserBooks.UserBook.DateReturn }).ToList();
        }

        public async Task<dynamic> GetUserBookAsync(int userId)
        {
            return await (from all in (from userUserBooks in (from users in ctx.Users join userBooks in ctx.UserBooks on users.UserId equals userBooks.UserId select new { User = users, UserBook = userBooks })
                                       join books in ctx.Books on userUserBooks.UserBook.BookId equals books.BookId
                                       select new { UserUserBooks = userUserBooks, Books = books })
                          where all.UserUserBooks.User.UserId == userId
                          select new { all.Books.BookId, all.Books.BookName, all.Books.Isbn, all.Books.Author, all.UserUserBooks.UserBook.DateLent, all.UserUserBooks.UserBook.DateReturn }).ToListAsync();
        }

        #endregion Read

        #region Create

        public Task<Admin> CreateAdminAsync(Admin obj)
        {
            ctx.Admins.Add(obj);
            ctx.SaveChanges();

            return Task.FromResult(obj);
        }

        public Task<User> CreateUserAsync(User obj)
        {
            ctx.Users.Add(obj);
            ctx.SaveChanges();

            return Task.FromResult(obj);
        }

        public Task<Book> CreateBookAsync(Book obj)
        {
            ctx.Books.Add(obj);
            ctx.SaveChanges();

            return Task.FromResult(obj);
        }

        public void CreateUserBook(UserBook obj)
        {
            ctx.UserBooks.Add(obj);
            ctx.SaveChanges();
        }

        public Task<UserBook> CreateUserBookAsync(UserBook obj)
        {
            ctx.UserBooks.Add(obj);
            ctx.SaveChanges();

            return Task.FromResult(obj);
        }

        #endregion Create

        #region Update

        public Task<bool> UpdateAdminAsync(Admin objAdmin)
        {
            Admin ExistingAdmin = ctx.Admins.Where(x => x.AdminId == objAdmin.AdminId).FirstOrDefault();

            if (ExistingAdmin != null)
            {
                ExistingAdmin.AdminName = objAdmin.AdminName;
                ExistingAdmin.Password = objAdmin.Password;
                ExistingAdmin.Name = objAdmin.Name;
                ExistingAdmin.Surname = objAdmin.Surname;
                ExistingAdmin.Email = objAdmin.Email;
                ExistingAdmin.DateOfBirth = objAdmin.DateOfBirth;
                ExistingAdmin.Phone = objAdmin.Phone;

                ctx.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> UpdateUserAsync(User objUser)
        {
            User ExistingUser = ctx.Users.Where(x => x.UserId == objUser.UserId).FirstOrDefault();

            if (ExistingUser != null)
            {
                ExistingUser.UserName = objUser.UserName;
                ExistingUser.Password = objUser.Password;
                ExistingUser.Name = objUser.Name;
                ExistingUser.Surname = objUser.Surname;
                ExistingUser.Email = objUser.Email;
                ExistingUser.DateOfBirth = objUser.DateOfBirth;
                ExistingUser.Phone = objUser.Phone;

                ctx.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> UpdateBookAsync(Book objBook)
        {
            Book ExistingBook = ctx.Books.Where(x => x.BookId == objBook.BookId).FirstOrDefault();

            if (ExistingBook != null)
            {
                ExistingBook.BookId = objBook.BookId;
                ExistingBook.BookName = objBook.BookName;
                ExistingBook.Isbn = objBook.Isbn;
                ExistingBook.Author = objBook.Author;

                ctx.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> UpdateUserBookAsync(UserBook objUserBook)
        {
            UserBook Existing = ctx.UserBooks.Where(x => x.UserBookId == objUserBook.UserBookId).FirstOrDefault();

            if (Existing != null)
            {
                Existing.UserId = objUserBook.UserId;
                Existing.BookId = objUserBook.BookId;
                Existing.DateLent = objUserBook.DateLent;
                Existing.DateReturn = objUserBook.DateReturn;

                ctx.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        #endregion Update

        #region Delete

        public Task<bool> DeleteAdminAsync(Admin obj)
        {
            var Existing = ctx.Admins.Where(x => x.AdminId == obj.AdminId).FirstOrDefault();

            if (Existing != null)
            {
                ctx.Admins.Remove(Existing);
                ctx.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> DeleteUserAsync(User obj)
        {
            var Existing = ctx.Users.Where(x => x.UserId == obj.UserId).FirstOrDefault();

            if (Existing != null)
            {
                ctx.Users.Remove(Existing);
                ctx.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> DeleteBookAsync(Book obj)
        {
            var Existing = ctx.Books.Where(x => x.BookId == obj.BookId).FirstOrDefault();

            if (Existing != null)
            {
                ctx.Books.Remove(Existing);
                ctx.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public bool DeleteUserBook(int bookId)
        {
            var Existing = ctx.UserBooks.Where(x => x.BookId == bookId).FirstOrDefault();

            if (Existing != null)
            {
                ctx.UserBooks.Remove(Existing);
                ctx.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task<bool> DeleteUserBookAsync(int bookId)
        {
            var Existing = ctx.UserBooks.Where(x => x.BookId == bookId).FirstOrDefault();

            if (Existing != null)
            {
                ctx.UserBooks.Remove(Existing);
                ctx.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> DeleteUserBookWithUserBookIdAsync(int userBookId)
        {
            var Existing = ctx.UserBooks.Where(x => x.UserBookId == userBookId).FirstOrDefault();

            if (Existing != null)
            {
                ctx.UserBooks.Remove(Existing);
                ctx.SaveChanges();
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        #endregion Delete

        #region Search

        public Admin FindAdmin(string userName)
        {
            return ctx.Admins.Where(x => x.AdminName == userName).FirstOrDefault();
        }

        public async Task<Admin> FindAdminAsync(int id)
        {
            return await ctx.Admins.Where(x => x.AdminId == id).FirstOrDefaultAsync();
        }

        public async Task<User> FindUserAsync(int id)
        {
            return await ctx.Users.Where(x => x.UserId == id).FirstOrDefaultAsync();
        }

        public User FindUser(string userName)
        {
            return ctx.Users.Where(x => x.UserName == userName).FirstOrDefault();
        }

        public async Task<User> FindUserAsync(string userName)
        {
            return await ctx.Users.Where(x => x.UserName == userName).FirstOrDefaultAsync();
        }

        public async Task<List<Book>> FindBooksWithNameAsync(string bookName)
        {
            return await ctx.Books.Where(x => x.BookName.Contains(bookName)).ToListAsync();
        }

        public Book FindBookWithIsbn(string isbn)
        {
            return ctx.Books.Where(x => x.Isbn == isbn).First();
        }

        public async Task<Book> FindBookWithIsbnAsync(string isbn)
        {
            return await ctx.Books.Where(x => x.Isbn == isbn).FirstAsync();
        }

        public async Task<List<Book>> FindBooksWithIsbnAsync(string isbn)
        {
            return await ctx.Books.Where(x => x.Isbn.Contains(isbn)).ToListAsync();
        }

        public async Task<List<Book>> FindBooksAsync(string bookName, string isbn)
        {
            return await ctx.Books.Where(x => x.BookName.Contains(bookName) && x.Isbn.Contains(isbn)).ToListAsync();
        }

        #endregion Search

        #region check

        public bool IsContainsUserBook(int bookId)
        {
            return ctx.UserBooks.AsQueryable().Any(x => x.BookId == bookId);
        }

        public bool HaveUserExpiredBook(int userId)
        {
            return ctx.UserBooks.AsQueryable().Any(x => x.UserId == userId && x.DateReturn < System.DateTime.Now);
        }

        #endregion check
    }
}