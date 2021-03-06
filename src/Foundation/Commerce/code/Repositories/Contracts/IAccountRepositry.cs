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

    using Wooli.Foundation.Commerce.Models;
    using Wooli.Foundation.Commerce.Models.Account;

    public interface IAccountRepositry
    {
        Result<CreateAccountResultModel> CreateAccount(CreateAccountModel createAccountModel);

        Result<CommerceUserModel> UpdateAccountInfo(CommerceUserModel user);

        Result<ValidateAccountResultModel> ValidateAccount(ValidateAccountModel validateAccountModel);

        Result<ChangePasswordResultModel> ChangePassword(ChangePasswordModel changePasswordModel);

        Result<IEnumerable<AddressModel>> AddCustomerAddress(string userName, AddressModel address);

        Result<IEnumerable<AddressModel>> RemoveCustomerAddress(string userName, AddressModel address);

        Result<IEnumerable<AddressModel>> UpdateAddress(string userName, AddressModel address);

        Result<IEnumerable<AddressModel>> GetAddressList(string userName);
    }
}
