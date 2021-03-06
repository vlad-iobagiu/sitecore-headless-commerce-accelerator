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
    using System.Collections.Generic;
    using System.Linq;

    using Glass.Mapper.Sc;

    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;

    using Wooli.Foundation.Commerce.Context;
    using Wooli.Foundation.Commerce.Models;
    using Wooli.Foundation.Commerce.Providers;
    using Wooli.Foundation.Connect.Managers;
    using Wooli.Foundation.Connect.Models;

    using ProductModel = Wooli.Foundation.Commerce.Models.ProductModel;

    public class BaseCatalogRepository
    {
        public const string CurrentCatalogItemRenderingModelKey = "CurrentCatalogItemRenderingModel";

        public BaseCatalogRepository(
            ICurrencyProvider currencyProvider,
            ISiteContext siteContext,
            IStorefrontContext storefrontContext,
            IVisitorContext visitorContext,
            ICatalogManager catalogManager,
            ISitecoreContext sitecoreContext)
        {
            this.CurrencyProvider = currencyProvider;
            this.SiteContext = siteContext;
            this.StorefrontContext = storefrontContext;
            this.VisitorContext = visitorContext;
            this.CatalogManager = catalogManager;
            this.SitecoreContext = sitecoreContext;
        }

        public ICurrencyProvider CurrencyProvider { get; }

        public ICatalogManager CatalogManager { get; }

        public ISiteContext SiteContext { get; }

        public ISitecoreContext SitecoreContext { get; }

        public IStorefrontContext StorefrontContext { get; }

        public IVisitorContext VisitorContext { get; }

        protected virtual ProductModel GetProductModel(IVisitorContext visitorContext, Item productItem)
        {
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));

            if (productItem == null)
            {
                return null;
            }

            var variantEntityList = new List<Variant>();
            if (productItem.HasChildren)
            {
                variantEntityList = this.LoadVariants(productItem);
            }

            var product = new Product(productItem, variantEntityList);
            product.CatalogName = this.StorefrontContext.CatalogName;

            product.CustomerAverageRating = this.CatalogManager.GetProductRating(productItem);

            this.CatalogManager.GetProductPrice(product);
            this.CatalogManager.GetStockInfo(product, this.StorefrontContext.ShopName);

            var renderingModel = new ProductModel();
            var model = this.SitecoreContext.Cast<ICommerceProductModel>(productItem);

            renderingModel.Initialize(model);
            renderingModel.CurrencySymbol = this.CurrencyProvider.GetCurrencySymbolByCode(product.CurrencyCode);
            renderingModel.ListPrice = product.ListPrice;
            renderingModel.AdjustedPrice = product.AdjustedPrice;
            renderingModel.StockStatusName = product.StockStatusName;
            renderingModel.CustomerAverageRating = product.CustomerAverageRating;

            foreach (ProductVariantModel renderingModelVariant in renderingModel.Variants)
            {
                var variant = product.Variants.FirstOrDefault(x => x.VariantId == renderingModelVariant.ProductVariantId);
                if (variant == null)
                {
                    continue;
                }

                renderingModelVariant.CurrencySymbol = this.CurrencyProvider.GetCurrencySymbolByCode(variant.CurrencyCode);
                renderingModelVariant.ListPrice = variant.ListPrice;
                renderingModelVariant.AdjustedPrice = variant.AdjustedPrice;
                renderingModelVariant.StockStatusName = variant.StockStatusName;
                renderingModelVariant.CustomerAverageRating = variant.CustomerAverageRating;
            }

            return renderingModel;
        }

        private List<Variant> LoadVariants(Item productItem)
        {
            var variants = new List<Variant>();
            foreach (Item variantItem in productItem.Children)
            {
                var variantEntity = new Variant(variantItem);
                variantEntity.CustomerAverageRating = this.CatalogManager.GetProductRating(variantItem);

                variants.Add(variantEntity);
            }

            return variants;
        }

        protected CategoryModel GetCategoryModel(Item categoryItem)
        {
            var glassModel = this.SitecoreContext.Cast<IConnectCategoryModel>(categoryItem);
            if (glassModel == null)
            {
                return null;
            }

            var categoryModel = new CategoryModel();
            categoryModel.Initialize(glassModel);

            return categoryModel;
        }
    }
}
