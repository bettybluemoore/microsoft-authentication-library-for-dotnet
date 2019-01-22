﻿// ------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Utils;
using Microsoft.Identity.Client.ApiConfig.Parameters;
using Microsoft.Identity.Client.AppConfig;
using Microsoft.Identity.Client.TelemetryCore;

namespace Microsoft.Identity.Client.ApiConfig
{
    /// <summary>
    /// Base class for builders of token requests, which attempt to acquire a token
    /// based on the provided parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractAcquireTokenParameterBuilder<T>
        where T : AbstractAcquireTokenParameterBuilder<T>
    {
        internal AcquireTokenCommonParameters CommonParameters { get; } = new AcquireTokenCommonParameters();

        /// <summary>
        /// Executes the Token request asynchronously, with a possibility of cancelling the
        /// asynchronous method.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token. See <see cref="CancellationToken"/> </param>
        /// <returns>Authentication result containing a token for the requested scopes and parameters
        /// set in the builder</returns>
        public abstract Task<AuthenticationResult> ExecuteAsync(CancellationToken cancellationToken);

        internal abstract ApiEvent.ApiIds CalculateApiEventId();

        /// <summary>
        /// Executes the Token request asynchronously.
        /// </summary>
        /// <returns>Authentication result containing a token for the requested scopes and parameters
        /// set in the builder</returns>
        public Task<AuthenticationResult> ExecuteAsync()
        {
            return ExecuteAsync(CancellationToken.None);
        }

        /// <summary>
        /// Specifies which scopes to request
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <returns>The builder to chain the .With methods</returns>
        protected T WithScopes(IEnumerable<string> scopes)
        {
            CommonParameters.Scopes = scopes;
            return (T)this;
        }

        /// <summary>
        /// Sets Extra Query Parameters for the query string in the HTTP authentication request
        /// </summary>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority
        /// as a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithExtraQueryParameters(Dictionary<string, string> extraQueryParameters)
        {
            CommonParameters.ExtraQueryParameters = extraQueryParameters ?? new Dictionary<string, string>();
            return (T)this;
        }

        // This exists for back compat with old-style API.  Once we deprecate it, we can remove this.
        internal T WithExtraQueryParameters(string extraQueryParameters)
        {
            return WithExtraQueryParameters(CoreHelpers.ParseKeyValueList(extraQueryParameters, '&', true, null));
        }

        /// <summary>
        /// Specific authority for which the token is requested. Passing a different value than configured
        /// at the application constructor narrows down the selection to a specific tenant.
        /// This does not change the configured value in the application. This is specific
        /// to applications managing several accounts (like a mail client with several mailboxes)
        /// </summary>
        /// <param name="authorityUri">Uri for the authority</param>
        /// <param name="validateAuthority"></param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithAuthority(Uri authorityUri, bool validateAuthority = false)
        {
            CommonParameters.AuthorityOverride = AuthorityInfo.FromAuthorityUri(authorityUri.ToString(), validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloudInstanceUri"></param>
        /// <param name="tenantId"></param>
        /// <param name="validateAuthority"></param>
        /// <returns></returns>
        public T WithAadAuthority(
            Uri cloudInstanceUri,
            Guid tenantId,
            bool validateAuthority = true)
        {
            CommonParameters.AuthorityOverride = AuthorityInfo.FromAadAuthority(cloudInstanceUri, tenantId, validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloudInstanceUri"></param>
        /// <param name="tenant"></param>
        /// <param name="validateAuthority"></param>
        /// <returns></returns>
        public T WithAadAuthority(
            Uri cloudInstanceUri,
            string tenant,
            bool validateAuthority = true)
        {
            CommonParameters.AuthorityOverride = AuthorityInfo.FromAadAuthority(cloudInstanceUri, tenant, validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="azureCloudInstance"></param>
        /// <param name="tenantId"></param>
        /// <param name="validateAuthority"></param>
        /// <returns></returns>
        public T WithAadAuthority(
            AzureCloudInstance azureCloudInstance,
            Guid tenantId,
            bool validateAuthority = true)
        {
            CommonParameters.AuthorityOverride = AuthorityInfo.FromAadAuthority(azureCloudInstance, tenantId, validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="azureCloudInstance"></param>
        /// <param name="tenant"></param>
        /// <param name="validateAuthority"></param>
        /// <returns></returns>
        public T WithAadAuthority(
            AzureCloudInstance azureCloudInstance,
            string tenant,
            bool validateAuthority = true)
        {
            CommonParameters.AuthorityOverride = AuthorityInfo.FromAadAuthority(azureCloudInstance, tenant, validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="azureCloudInstance"></param>
        /// <param name="authorityAudience"></param>
        /// <param name="validateAuthority"></param>
        /// <returns></returns>
        public T WithAadAuthority(AzureCloudInstance azureCloudInstance, AadAuthorityAudience authorityAudience, bool validateAuthority = true)
        {
            CommonParameters.AuthorityOverride = AuthorityInfo.FromAadAuthority(azureCloudInstance, authorityAudience, validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorityAudience"></param>
        /// <param name="validateAuthority"></param>
        /// <returns></returns>
        public T WithAadAuthority(AadAuthorityAudience authorityAudience, bool validateAuthority = true)
        {
            CommonParameters.AuthorityOverride = AuthorityInfo.FromAadAuthority(authorityAudience, validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorityUri"></param>
        /// <param name="validateAuthority"></param>
        /// <returns></returns>
        public T WithAadAuthority(string authorityUri, bool validateAuthority = true)
        {
            CommonParameters.AuthorityOverride = AuthorityInfo.FromAadAuthority(authorityUri, validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorityUri"></param>
        /// <param name="validateAuthority"></param>
        /// <returns></returns>
        public T WithAdfsAuthority(string authorityUri, bool validateAuthority = true)
        {
            CommonParameters.AuthorityOverride = new AuthorityInfo(AuthorityType.Adfs, authorityUri, validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorityUri"></param>
        /// <returns></returns>
        public T WithB2CAuthority(string authorityUri)
        {
            CommonParameters.AuthorityOverride = new AuthorityInfo(AuthorityType.B2C, authorityUri, false);
            return (T)this;
        }

        /// <summary>
        ///
        /// </summary>
        protected virtual void Validate()
        {
        }

        internal void ValidateAndCalculateApiId()
        {
            Validate();
            CommonParameters.ApiId = CalculateApiEventId();
        }
    }
}