using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Services;
using OShop.Stancer.Models;
using OShop.Stancer.Models.Api;

namespace OShop.Stancer.Services {
    public class StancerApiService : IStancerApiService {
        public static string ApiEndpoint = "https://api.stancer.com";
        public static string PaymentUrl = "https://payment.stancer.com";

        private StancerSettings _settings;

        private StancerSettings Settings {
            get {
                _settings = _settings ?? _settingsService.GetSettings();
                return _settings;
            }
        }

        private readonly IStancerSettingsService _settingsService;
        private readonly IClock _clock;

        public StancerApiService(
            IStancerSettingsService settingsService,
            IClock clock) {
            _settingsService = settingsService;
            _clock = clock;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

            // Enable TLS v1.2
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
        }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public async Task PingAsync(string privateKey) {
            using (var client = CreateHttpClient(privateKey)) {
                try {
                    var response = await client.GetAsync("v2/ping");
                    if (response.IsSuccessStatusCode) {
                        return;
                    }
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Logger.Error("Ping failed. ({0}) {1}\r\n{2}", response.StatusCode, response.ReasonPhrase, errorMsg);
                }
                catch (Exception ex) {
                    Logger.Error($"{ex.Message}");
                }
                throw new OrchardException(T("Ping failed."));
            }
        }

        public async Task<PaymentOut> CreatePaymentAsync(PaymentIn payment) {
            using (var client = CreateHttpClient()) {
                try {
                    var response = await client.PostAsJsonAsync("v2/payments/", payment);
                    if (response.IsSuccessStatusCode) {
                        return await response.Content.ReadAsAsync<PaymentOut>();
                    }
                    else {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        Logger.Error("Payment creation failed. ({0}) {1}\r\n{2}", response.StatusCode, response.ReasonPhrase, errorMsg);
                        throw new OrchardException(T("Payment creation failed."));
                    }
                }
                catch(Exception exp) {
                    throw new OrchardException(T("Payment creation failed."), exp);
                }
            }
        }

        public async Task<PaymentOut> GetPaymentAsync(string paymentId) {
            using (var client = CreateHttpClient()) {
                try {
                    var response = await client.GetAsync($"v2/payments/{paymentId}");
                    if (response.IsSuccessStatusCode) {
                        return await response.Content.ReadAsAsync<PaymentOut>();
                    }
                    else {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        Logger.Error("Payment retrieval failed. ({0}) {1}\r\n{2}", response.StatusCode, response.ReasonPhrase, errorMsg);
                        throw new OrchardException(T("Payment retrieval failed."));
                    }
                }
                catch (Exception exp) {
                    throw new OrchardException(T("Payment retrieval failed."), exp);
                }
            }
        }

        public async Task<PaymentOutList> ListPaymentsAsync() {
            using (var client = CreateHttpClient()) {
                try {
                    var response = await client.GetAsync($"v2/payments/");
                    if (response.IsSuccessStatusCode) {
                        return await response.Content.ReadAsAsync<PaymentOutList>();
                    }
                    else {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        Logger.Error("Payment retrieval failed. ({0}) {1}\r\n{2}", response.StatusCode, response.ReasonPhrase, errorMsg);
                        throw new OrchardException(T("Payment retrieval failed."));
                    }
                }
                catch (Exception exp) {
                    throw new OrchardException(T("Payment retrieval failed."), exp);
                }
            }
        }

        public async Task<PaymentOut> CapturePaymentAsync(string paymentId) {
            using (var client = CreateHttpClient()) {
                try {
                    var response = await client.PostAsync($"v2/payments/{paymentId}/capture", null);
                    if (response.IsSuccessStatusCode) {
                        return await response.Content.ReadAsAsync<PaymentOut>();
                    }
                    else {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        Logger.Error("Payment capture failed. ({0}) {1}\r\n{2}", response.StatusCode, response.ReasonPhrase, errorMsg);
                        throw new OrchardException(T("Payment capture failed."));
                    }
                }
                catch (Exception exp) {
                    throw new OrchardException(T("Payment capture failed."), exp);
                }
            }
        }

        private HttpClient CreateHttpClient(string privateKey = null) {
            var client = new HttpClient();
            client.BaseAddress = new Uri(ApiEndpoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                Encoding.UTF8.GetBytes((privateKey ?? Settings.PrivateKey) + ":")
            ));

            return client;
        }

        public string GetPaymentUrl(string paymentId) {
            return $"{PaymentUrl}/{Settings.PublicKey}/{paymentId}";
        }
    }
}