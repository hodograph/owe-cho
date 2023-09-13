using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using BlazorApp.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class ReadReceipt
    {
        private readonly ILogger _logger;

        public ReadReceipt(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ReadReceipt>();
        }

        [Function("ReadReceipt")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "receipt")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            HttpResponseData response;
            Transaction generated = new Transaction();

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (string.IsNullOrWhiteSpace(claim.Identity.Name))
            {
                response = req.CreateResponse(HttpStatusCode.NotFound);
                return response;
            }

            string key = Environment.GetEnvironmentVariable("OCR_KEY");
            string endpoint = Environment.GetEnvironmentVariable("OCR_ENDPOINT");

            AzureKeyCredential credential = new AzureKeyCredential(key);
            DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);
            Stream file = req.Body;

            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-receipt", file);
            AnalyzeResult receipts = operation.Value;

            foreach (AnalyzedDocument receipt in receipts.Documents)
            {
                if (receipt.Fields.TryGetValue("MerchantName", out DocumentField merchantNameField))
                {
                    if (merchantNameField.FieldType == DocumentFieldType.String)
                    {
                        string merchantName = merchantNameField.Value.AsString();

                        generated.TransactionName = merchantName;
                    }
                }

                if (receipt.Fields.TryGetValue("Total", out DocumentField totalField))
                {
                    if (totalField.FieldType == DocumentFieldType.Double)
                    {
                        double total = totalField.Value.AsDouble();

                        generated.Total = total;
                    }
                }

                if (receipt.Fields.TryGetValue("Items", out DocumentField itemsField))
                {
                    if (itemsField.FieldType == DocumentFieldType.List)
                    {
                        foreach (DocumentField itemField in itemsField.Value.AsList())
                        {
                            if (itemField.FieldType == DocumentFieldType.Dictionary)
                            {
                                IReadOnlyDictionary<string, DocumentField> itemFields = itemField.Value.AsDictionary();

                                if (itemFields.TryGetValue("Quantity", out DocumentField itemQuantityField))
                                {
                                    if (itemQuantityField.FieldType == DocumentFieldType.Double)
                                    {
                                        double itemQuantity = itemQuantityField.Value.AsDouble();

                                        for (int i = 0; i < itemQuantity; i++)
                                        {
                                            Debt debt = new Debt();
                                            if (itemFields.TryGetValue("Description", out DocumentField itemDescriptionField))
                                            {
                                                if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                                {
                                                    string itemDescription = itemDescriptionField.Value.AsString();

                                                    debt.Memo = itemDescription;
                                                }
                                            }

                                            if (itemFields.TryGetValue("TotalPrice", out DocumentField itemTotalPriceField))
                                            {
                                                if (itemTotalPriceField.FieldType == DocumentFieldType.Double)
                                                {
                                                    double itemTotalPrice = itemTotalPriceField.Value.AsDouble();

                                                    debt.Amount = itemTotalPrice/itemQuantity;
                                                }
                                            }
                                            generated.Debts.Add(debt);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteAsJsonAsync(generated);

            return response;
        }
    }
}
