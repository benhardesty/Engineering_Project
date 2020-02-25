using System;
using System.Threading.Tasks;
using FunctionApp1.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class CalculateDatesAndAmountsFunction
    {
        [FunctionName("CalculateDatesAndAmountsFunction")]
        public async Task Run([QueueTrigger("messagetomom", Connection = "")]MessageToMom myQueueItem, 
            [Queue("outputletter")] IAsyncCollector<FormLetter> letterCollector,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            decimal maximumAmount = 10000.00M;
            int businessDaysToProcessLoan = 10;

            FormLetter formLetter = new FormLetter
            {
                Heading = $"{myQueueItem.Greeting} {string.Join(", ", myQueueItem.Flattery)} Mother",
                Likelihood = calculateLoanLikelihood(maximumAmount, myQueueItem.HowMuch),
                ExpectedDate = calculateApproximateLoanDate(DateTime.UtcNow, businessDaysToProcessLoan),
                RequestedDate = myQueueItem.HowSoon ?? calculateApproximateLoanDate(DateTime.UtcNow, businessDaysToProcessLoan),
                Body = $"Really need help: I need ${myQueueItem.HowMuch} by " +
                       $"{myQueueItem.HowSoon ?? calculateApproximateLoanDate(DateTime.UtcNow, businessDaysToProcessLoan)}."
            };

            await letterCollector.AddAsync(formLetter);
        }

        // Calculate likelihood of receiving a loan based on 100 percent likelihood (initial value) minus 
        // the probability expressed from the quotient of loan request amount and the total maximum amount allowed.
        public double calculateLoanLikelihood(decimal maximumAmount, decimal requestedAmount)
        {
            return Math.Round(Math.Max(100 - (double)(requestedAmount / maximumAmount) * 100, 0), 2);
        }

        // Calculate approximate actual date of loan receipt based on funds will 
        // be made available X business days after day of submission. Business days
        // are weekdays and there are no holidays that are applicable.
        public DateTime calculateApproximateLoanDate(DateTime startDate, int businessDays)
        {
            if (startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                startDate.AddDays(-2);
            }
            if (startDate.DayOfWeek == DayOfWeek.Saturday)
            {
                startDate.AddDays(-1);
            }

            int daysUntillWeekend = 5 - (int)startDate.DayOfWeek;
            if (businessDays <= daysUntillWeekend)
            {
                return startDate.AddDays(businessDays);
            }

            startDate = startDate.AddDays(daysUntillWeekend + 2);
            businessDays = businessDays - daysUntillWeekend;
            int weekends = businessDays / 5;

            return startDate.AddDays(businessDays + weekends * 2);
        }
    }
}
