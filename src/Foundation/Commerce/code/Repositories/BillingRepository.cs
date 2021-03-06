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

namespace Wooli.Foundation.Commerce.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Sitecore;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Commerce.Services.Payments;
    using Sitecore.Diagnostics;

    using Wooli.Foundation.Commerce.Context;
    using Wooli.Foundation.Commerce.ModelInitilizers;
    using Wooli.Foundation.Commerce.ModelMappers;
    using Wooli.Foundation.Commerce.Models;
    using Wooli.Foundation.Connect.Managers;
    using Wooli.Foundation.DependencyInjection;

    using Entities = Sitecore.Commerce.Entities;

    [Service(typeof(IBillingRepository), Lifetime = Lifetime.Singleton)]
    public class BillingRepository : BaseCheckoutRepository, IBillingRepository
    {
        public BillingRepository(
            IPaymentManager paymentManager,
            ICartManager cartManager,
            ICatalogRepository catalogRepository,
            IAccountManager accountManager,
            ICartModelBuilder cartModelBuilder,
            IEntityMapper entityMapper,
            IStorefrontContext storefrontContext,
            IVisitorContext visitorContext)
            : base(cartManager, catalogRepository, accountManager, cartModelBuilder, entityMapper, storefrontContext, visitorContext)
        {
            this.PaymentManager = paymentManager;
        }

        protected IPaymentManager PaymentManager { get; }


        public virtual Result<BillingModel> GetBillingData()
        {
            var result = new Result<BillingModel>();
            var model = new BillingModel();

            if (!Context.PageMode.IsExperienceEditor)
            {
                try
                {
                    result.SetResult(model);
                    ManagerResponse<CartResult, Cart> currentCart = this.CartManager.GetCurrentCart(this.StorefrontContext.ShopName, this.VisitorContext.ContactId);
                    if (!currentCart.ServiceProviderResult.Success)
                    {
                        result.SetErrors(currentCart.ServiceProviderResult);
                        return result;
                    }

                    Cart cartResult = currentCart.Result;
                    if (cartResult.Lines != null && cartResult.Lines.Any())
                    {
                        ////result.Initialize(result, visitorContext);
                        this.AddPaymentOptions(result, cartResult);
                        if (result.Success)
                        {
                            this.AddPaymentMethods(result, cartResult);
                            if (result.Success)
                            {
                                this.AddPaymentClientToken(result);
                                if (result.Success)
                                {
                                    this.AddUserInfo(result.Data, result);
                                    ////if (result.Success)
                                    ////{
                                    ////    this.AddAvailableCountries((BaseCheckoutDataJsonResult)model);
                                    ////    if (result.Success)
                                    ////        this.CheckForDigitalProductInCart(model, cartResult);
                                    ////}
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex, (object)this);
                    result.SetErrors(nameof(this.GetBillingData), ex);
                    return result;
                }
            }
            return result;
        }

        protected virtual void AddPaymentOptions(Result<BillingModel> result, Cart cart)
        {
            ManagerResponse<GetPaymentOptionsResult, IEnumerable<Entities.Payments.PaymentOption>> paymentOptions = this.PaymentManager.GetPaymentOptions(this.StorefrontContext.ShopName, cart);

            if (paymentOptions.ServiceProviderResult.Success && paymentOptions.Result != null)
            {
                result.Data.PaymentOptions = new List<PaymentOptionModel>();
                foreach (Entities.Payments.PaymentOption paymentOption in paymentOptions.Result)
                {
                    var model = new PaymentOptionModel
                    {
                        Name = paymentOption.Name,
                        Description = paymentOption.Description,
                        PaymentOptionTypeName = paymentOption.PaymentOptionType.Name
                    };

                    result.Data.PaymentOptions.Add(model);
                }
            }
            else
            {
                result.SetErrors(paymentOptions.ServiceProviderResult);
            }
        }

        protected virtual void AddPaymentMethods(Result<BillingModel> result, Cart cart)
        {
            var paymentOption = new Entities.Payments.PaymentOption()
                {
                    PaymentOptionType = Entities.Payments.PaymentOptionType.PayCard
                };

            ManagerResponse<GetPaymentMethodsResult, IEnumerable<Entities.Payments.PaymentMethod>> paymentMethods =
                this.PaymentManager.GetPaymentMethods(cart, paymentOption);

            if (paymentMethods.ServiceProviderResult.Success && paymentMethods.Result != null)
            {
                result.Data.PaymentMethods = new List<PaymentMethodModel>();
                foreach (Entities.Payments.PaymentMethod paymentMethod in paymentMethods.Result)
                {
                    var model = new PaymentMethodModel();
                    model.Description = paymentMethod.Description;
                    model.ExternalId = paymentMethod.PaymentOptionId;
                    result.Data.PaymentMethods.Add(model);
                }
            }
            else
            {
                result.SetErrors(paymentMethods.ServiceProviderResult);
            }
        }

        protected virtual void AddPaymentClientToken(Result<BillingModel> result)
        {
            ManagerResponse<ServiceProviderResult, string> paymentClientToken = this.PaymentManager.GetPaymentClientToken();
            if (paymentClientToken.ServiceProviderResult.Success)
                result.Data.PaymentClientToken = paymentClientToken.Result;
            result.SetErrors(paymentClientToken.ServiceProviderResult);
        }

        public Result<VoidResult> SetPaymentMethods(SetPaymentArgs args)
        {
            var result = new Result<VoidResult>();
            var model = new VoidResult();

            try
            {
                result.SetResult(model);
                ManagerResponse<CartResult, Cart> currentCart = this.CartManager.GetCurrentCart(this.StorefrontContext.ShopName, this.VisitorContext.ContactId);
                if (!currentCart.ServiceProviderResult.Success)
                {
                    result.SetErrors(currentCart.ServiceProviderResult);
                    return result;
                }

                ManagerResponse<CartResult, Cart> updateCartResponse = this.CartManager.UpdateCart(
                    this.StorefrontContext.ShopName,
                    currentCart.Result,
                    new CartBase() { Email = string.IsNullOrWhiteSpace(args.BillingAddress.Email) ? this.VisitorContext.CurrentUser.Email : args.BillingAddress.Email });

                if (!updateCartResponse.ServiceProviderResult.Success && updateCartResponse.ServiceProviderResult.SystemMessages.Any())
                {
                    result.SetErrors(updateCartResponse.ServiceProviderResult);
                    return result;
                }

                var billingParty = this.EntityMapper.MapToPartyEntity(args.BillingAddress);
                var federatedPaymentArgs = this.EntityMapper.MapToFederatedPaymentArgs(args.FederatedPayment);

                ManagerResponse<AddPaymentInfoResult, Cart> paymentInfoResponse = this.CartManager.AddPaymentInfo(
                    this.StorefrontContext.ShopName,
                    updateCartResponse.Result,
                    billingParty,
                    federatedPaymentArgs);

                if (!paymentInfoResponse.ServiceProviderResult.Success)
                {
                    result.SetErrors(paymentInfoResponse.ServiceProviderResult);
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.SetErrors(nameof(this.SetPaymentMethods), ex);
            }

            return result;
        }
    }
}
