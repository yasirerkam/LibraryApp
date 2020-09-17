using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LibraryApp.Data.Library;

namespace LibraryApp.Services
{
    public class AppDataService
    {
        private AppDataService()
        {
        }

        private static readonly Lazy<AppDataService> instance = new Lazy<AppDataService>(() => new AppDataService(), true);

        public static AppDataService Instance { get { return instance.Value; } }

        public Admin adminSigned;
        public User userSigned;
        private List<string> tessTextList = new List<string>();
        public string pathImageDirectory = System.IO.Path.Combine(Startup.pathWebRoot, "Images");

        public List<string> TessTextList
        {
            get => tessTextList;
            set
            {
                tessTextList = value;
                NotifyDataChanged();
            }
        }

        public event Action OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

        #region regex

        public string ExtractISBN(string inputStr)
        {
            string pattern = @"9\D*7\D*8\D*\d\D*\d\D*\d\D*\d\D*\d\D*\d\D*\d\D*\d\D*\d\D*\d";
            MatchCollection matches = Regex.Matches(inputStr, pattern);

            // This will print the number of matches
            Console.WriteLine("{0} matches", matches.Count);

            foreach (Match match in matches)
            {
                Console.WriteLine("Match: {0} at index [{1}, {2})",
                    match.Value,
                    match.Index,
                    match.Index + match.Length);
            }

            if (matches.Count > 0)
            {
                return new string(matches[0].Value.Where(x => char.IsDigit(x)).ToArray());
            }
            else return null;
        }

        #endregion regex
    }
}