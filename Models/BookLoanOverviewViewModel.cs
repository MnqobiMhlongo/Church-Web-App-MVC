using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class BookLoanOverviewViewModel
    {
        public List<BookLoan> AllBorrowedBooks { get; set; }
        public List<BookLoan> AwaitingCollection { get; set; }
        public List<BookLoan> AwaitingReturn { get; set; }
    }
}