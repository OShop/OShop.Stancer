using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Mvc.Html;
using Orchard.Services;
using Orchard.Settings;
using Orchard.UI.Notify;
using OShop.Models;
using OShop.Services;
using OShop.Stancer.Models.Api;
using OShop.Stancer.Services;

namespace OShop.Stancer.Controllers {
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IStancerApiService _apiService;
        private readonly IClock _clock;
        private readonly ISiteService _siteService;

        public PaymentController(
            IPaymentService paymentService,
            ICurrencyProvider currencyProvider,
            IStancerApiService apiService,
            IClock clock,
            ISiteService siteService,
            IOrchardServices services) {
            _paymentService = paymentService;
            _currencyProvider = currencyProvider;
            _apiService = apiService;
            _clock = clock;
            _siteService = siteService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        /// <summary>
        /// Create Stancer payment for a ContentItem with a PaymentPart
        /// </summary>
        /// <param name="Id">ContentItem Id</param>
        public async Task<ActionResult> Create(int Id)
        {
            var paymentPart = Services.ContentManager.Get<PaymentPart>(Id);
            if (paymentPart == null) {
                return new HttpNotFoundResult();
            }

            decimal OutstandingAmount = paymentPart.PayableAmount - paymentPart.AmountPaid;
            if (OutstandingAmount <= 0) {
                Services.Notifier.Information(T("Nothing left to pay on this document."));
                return Redirect(Url.ItemDisplayUrl(paymentPart));
            }

            var transaction = new PaymentTransactionRecord() {
                Method = "Stancer",
                Amount = OutstandingAmount,
                Date = _clock.UtcNow,
                Status = TransactionStatus.Pending
            };

            _paymentService.AddTransaction(paymentPart, transaction);

            var siteSettings = _siteService.GetSiteSettings();
            string baseUrl = siteSettings.BaseUrl.TrimEnd(new char[] {' ', '/'});

            try {
                // Create payment
                var payment = await _apiService.CreatePaymentAsync(new PaymentIn() {
                    Amount = Convert.ToInt32(OutstandingAmount * 100),
                    Currency = _currencyProvider.IsoCode?.ToLower(),
                    Auth = true,
                    MethodsAllowed = new[] { PaymentMethods.Card },
                    Capture = false,
                    Description = T("Order {0}", paymentPart.Reference).Text,
                    OrderId = paymentPart.Reference,
                    ReturnUrl = baseUrl + Url.Action("Execute", new { id = transaction.Id })
                });

                if (payment != null && !string.IsNullOrEmpty(payment.Id)) {
                    transaction.TransactionId = payment.Id;
                    transaction.Data = JsonConvert.SerializeObject(payment);
                    _paymentService.UpdateTransaction(transaction);
                    return Redirect(_apiService.GetPaymentUrl(payment.Id));
                }
                else {
                    Services.Notifier.Error(T("Payment creation failed."));
                }
            }
            catch {
                Services.Notifier.Error(T("We are encountering issues with Stancer service."));
            }
            return Redirect(Url.ItemDisplayUrl(paymentPart));
        }

        /// <summary>
        /// Execute an approved Stancer payment
        /// </summary>
        /// <param name="Id">PaymentTransactionRecord Id</param>
        public async Task<ActionResult> Execute(int Id) {
            var transactionRecord = _paymentService.GetTransaction(Id);
            if (transactionRecord == null) {
                return new HttpNotFoundResult();
            }

            var payment = JsonConvert.DeserializeObject<PaymentOut>(transactionRecord.Data);
            if(payment != null) {
                try {
                    payment = await _apiService.GetPaymentAsync(payment.Id);
                    if (payment.Status == Models.Api.PaymentStatus.Authorized) {
                        payment = await _apiService.CapturePaymentAsync(payment.Id);
                    }
                }
                catch {
                    Services.Notifier.Error(T("We are encountering issues with Stancer service."));
                }
                switch (payment.Status) {
                    case Models.Api.PaymentStatus.ToCapture:
                    case Models.Api.PaymentStatus.CaptureSent:
                    case Models.Api.PaymentStatus.Captured:
                        transactionRecord.Status = TransactionStatus.Validated;
                        Services.Notifier.Information(T("Your payment was successfully registered."));
                        break;
                    default:
                        transactionRecord.Status = TransactionStatus.Canceled;
                        Services.Notifier.Error(T("We were unable to execute your payment."));
                        break;
                }

                transactionRecord.Data = JsonConvert.SerializeObject(payment);
                transactionRecord.Amount = payment.Amount / 100m;
                _paymentService.UpdateTransaction(transactionRecord);
            }
            else {
                Services.Notifier.Error(T("We were unable to execute your payment."));
            }

            var content = Services.ContentManager.Get(transactionRecord.PaymentPartRecord.ContentItemRecord.Id);
            return Redirect(Url.ItemDisplayUrl(content));
        }
    }
}