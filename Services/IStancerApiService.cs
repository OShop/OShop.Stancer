using System.Threading.Tasks;
using Orchard;
using OShop.Stancer.Models.Api;

namespace OShop.Stancer.Services {
    public interface IStancerApiService : IDependency {
        Task PingAsync(string privateKey);
        Task<PaymentOut> CreatePaymentAsync(PaymentIn payment);
        Task<PaymentOut> GetPaymentAsync(string paymentId);
        Task<PaymentOutList> ListPaymentsAsync();
        Task<PaymentOut> CapturePaymentAsync(string paymentId);
        string GetPaymentUrl(string paymentId);
    }
}
