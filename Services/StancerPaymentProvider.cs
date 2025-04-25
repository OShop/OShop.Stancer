using System;
using System.Web.Routing;
using Orchard.Localization;
using OShop.Models;
using OShop.Services;

namespace OShop.Stancer.Services {
    public class StancerPaymentProvider : IPaymentProvider {
        public StancerPaymentProvider(){
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public int Priority {
            get { return 55; }
        }

        public string Name {
            get { return "Stancer"; }
        }

        public LocalizedString Label {
            get { return T("Stancer"); }
        }

        public LocalizedString Description {
            get { return T("Pay with your credit card"); }
        }

        public RouteValueDictionary GetPaymentRoute(PaymentPart Part) {
            if (Part == null) {
                throw new ArgumentNullException("Part", "PaymentPart cannot be null.");
            }
            return new RouteValueDictionary(new {
                Area = "OShop.Stancer",
                Controller = "Payment",
                Action = "Create",
                Id = Part.ContentItem.Id
            });
        }
    }
}