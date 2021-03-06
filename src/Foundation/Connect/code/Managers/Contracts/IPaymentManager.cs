//    Copyright 2019 EPAM Systems, Inc.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

namespace Wooli.Foundation.Connect.Managers
{
    using System.Collections.Generic;

    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Payments;
    using Entities = Sitecore.Commerce.Entities;

    public interface IPaymentManager
    {
        ManagerResponse<GetPaymentMethodsResult, IEnumerable<Entities.Payments.PaymentMethod>> GetPaymentMethods(Cart cart, Entities.Payments.PaymentOption paymentOption);

        ManagerResponse<GetPaymentOptionsResult, IEnumerable<Entities.Payments.PaymentOption>> GetPaymentOptions(string shopName, Cart cart);

        ManagerResponse<ServiceProviderResult, string> GetPaymentClientToken();
    }
}
