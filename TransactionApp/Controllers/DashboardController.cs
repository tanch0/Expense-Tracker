using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TransactionApp.FilterModel;
using TransactionApp.Models.Domain;
using TransactionApp.Repositories.Abstract;

[Authorize]
public class DashboardController : Controller
{

    public readonly UserManager<ApplicationUser> _userManager;
    public readonly ITransactionRepository _transactionRepository;

    public DashboardController(UserManager<ApplicationUser> userManager, ITransactionRepository transactionRepository)
    {
        _userManager = userManager;
        _transactionRepository = transactionRepository;
    }

    [Authorize]
    public IActionResult Index()
    {
        String UserId = _userManager.GetUserId(HttpContext.User);
        var transactions = _transactionRepository.GetQueryable().Where(x => x.UserId == UserId);
        return View(transactions);
    }
    [Authorize]
    public IActionResult Display()
    {
        return View();
    }
    [HttpGet]
    public IActionResult ChartData()
    {
        string userId = _userManager.GetUserId(HttpContext.User);
        var transactions = _transactionRepository.GetQueryable()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedDateTime)
            .ToList();

        var groupedTransactions = transactions
            .GroupBy(x => new { x.TransactionType, x.CreatedDateTime.Date })
            .Select(group => new
            {
                Label = $"{(group.Key.TransactionType ? "Income" : "Expense")} - {group.Key.Date.ToShortDateString()}",
                TotalAmount = group.Sum(x => x.TransactionAmount),
            })
            .ToList();

        var chartData = new
        {
            labels = groupedTransactions.Select(x => x.Label),
            datasets = new[]
            {
            new
            {
                label = "Total Transaction Amount",
                data = groupedTransactions.Select(x => x.TotalAmount),
                backgroundColor = "rgba(75, 192, 192, 0.2)",
                borderColor = "rgba(75, 192, 192, 1)",
                borderWidth = 1
            }
        }
        };

        return Ok(chartData);
    }

    [HttpGet]
    public IActionResult FilterData(string period)
    {
        string userId = _userManager.GetUserId(HttpContext.User);
        var transactions = _transactionRepository.GetQueryable()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedDateTime)
            .ToList();

        IEnumerable<dynamic> groupedTransactions = null;

        switch (period)
        {
            case "Daily":
                groupedTransactions = transactions
                    .GroupBy(x => new { x.TransactionType, x.CreatedDateTime.Date })
                    .Select(group => new
                    {
                        Label = $"{(group.Key.TransactionType ? "Income" : "Expense")} - {group.Key.Date.ToShortDateString()}",
                        TotalAmount = group.Sum(x => x.TransactionAmount),
                    });
                break;
            case "Weekly":
                groupedTransactions = transactions
                    .GroupBy(x => new { x.TransactionType, WeekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(x.CreatedDateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday) })
                    .Select(group => new
                    {
                        Label = $"{(group.Key.TransactionType ? "Income" : "Expense")} - Week {group.Key.WeekOfYear}",
                        TotalAmount = group.Sum(x => x.TransactionAmount),
                    });
                break;
            case "Monthly":
                groupedTransactions = transactions
                    .GroupBy(x => new { x.TransactionType, x.CreatedDateTime.Year, x.CreatedDateTime.Month })
                    .Select(group => new
                    {
                        Label = $"{(group.Key.TransactionType ? "Income" : "Expense")} - {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key.Month)} {group.Key.Year}",
                        TotalAmount = group.Sum(x => x.TransactionAmount),
                    });
                break;
            case "Yearly":
                groupedTransactions = transactions
                    .GroupBy(x => new { x.TransactionType, x.CreatedDateTime.Year })
                    .Select(group => new
                    {
                        Label = $"{(group.Key.TransactionType ? "Income" : "Expense")} - Year {group.Key.Year}",
                        TotalAmount = group.Sum(x => x.TransactionAmount),
                    });
                break;
            case "Quarterly":
                groupedTransactions = transactions
                    .GroupBy(x => new { x.TransactionType, Quarter = (x.CreatedDateTime.Month - 1) / 3 + 1, x.CreatedDateTime.Year })
                    .Select(group => new
                    {
                        Label = $"{(group.Key.TransactionType ? "Income" : "Expense")} - Q{group.Key.Quarter} {group.Key.Year}",
                        TotalAmount = group.Sum(x => x.TransactionAmount),
                    });
                break;
            default:
                return RedirectToAction("Display");
        }

        var chartData = new
        {
            labels = groupedTransactions.Select(x => x.Label),
            datasets = new[]
            {
            new
            {
                label = "Total Transaction Amount",
                data = groupedTransactions.Select(x => x.TotalAmount),
                backgroundColor = "rgba(75, 192, 192, 0.2)",
                borderColor = "rgba(75, 192, 192, 1)",
                borderWidth = 1
            }
        }
        };

        return Ok(chartData);
    }


    [Authorize]
    public IActionResult Details(int id)
    {
        var transaction = _transactionRepository.Find(id);

        if (transaction == null)
        {
            return NotFound();
        }

        return View(transaction);
    }


    [Authorize]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(Transaction transaction)
    {
        transaction.UserId = _userManager.GetUserId(HttpContext.User);
        transaction.CreatedDateTime = DateTime.Now;

        transaction.EditedDateTime = DateTime.Now;
        _transactionRepository.Insert(transaction);
        TempData["success"] = "Created successfully";
        return RedirectToAction("Index");
    }

    [Authorize]
    public IActionResult Edit(int id)
    {
        Transaction transaction = _transactionRepository.Find(id);
        return View(transaction);
    }
    [HttpPost]
    public IActionResult Edit(Transaction transaction)
    {
        transaction.EditedDateTime = DateTime.Now;
        _transactionRepository.Update(transaction);
        TempData["success"] = "Edited successfully";
        return RedirectToAction("Index");
    }

    [Authorize]
    public IActionResult Delete(int id)
    {
        Transaction transaction = _transactionRepository.Find(id);
        _transactionRepository.Delete(transaction);
        TempData["success"] = "Deleted successfully";
        return RedirectToAction("Index");
    }
}