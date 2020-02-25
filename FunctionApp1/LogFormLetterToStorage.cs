using System;
using System.Threading.Tasks;
using FunctionApp1.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionApp1
{
    public class LogFormLetterToStorage
    {
        [FunctionName("LogFormLetterToStorage")]
        public async Task Run([QueueTrigger("outputletter", Connection = "")]FormLetter myQueueItem,
            [Table("letters")] IAsyncCollector<LetterEntity> letterTableCollector,
            ILogger log)
        {

            Random rand = new Random();
            LetterEntity letterEntity = new LetterEntity
            {
                Heading = myQueueItem.Heading,
                Likelihood = myQueueItem.Likelihood,
                ExpectedDate = myQueueItem.ExpectedDate,
                RequestedDate = myQueueItem.RequestedDate,
                Body = myQueueItem.Body,
                PartitionKey = myQueueItem.Heading.ToLower(),
                RowKey = ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString() +
                         rand.Next(10000, 99000).ToString() // Unix timestamp + random 5 digit integer
            };

            await letterTableCollector.AddAsync(letterEntity);

            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");


        }
    }

    public class LetterEntity : TableEntity {
        public string Heading { get; set; }
        public double Likelihood { get; set; }
        public DateTime ExpectedDate { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Body { get; set; }
    }
}
