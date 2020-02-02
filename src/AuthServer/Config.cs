// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;

using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;

namespace AuthServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api1", "My API #1"),
                new ApiResource("eshoponweb", "EshopOnWeb") {
                    Scopes = {
                        new Scope("order:read"),
                        new Scope("order:write"),
                        new Scope("catalog-item:read"),
                        new Scope("catalog-item:write"),
                        new Scope("catalog-brand:read"),
                        new Scope("catalog-brand:write"),
                        new Scope("catalog-type:read"),
                        new Scope("catalog-type:write")
                    }
                }
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1" }
                },

                // MVC client using code flow + pkce
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    Enabled = true,

                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = true,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = {
                        "http://localhost:5001/signin-oidc",
                        "http://localhost:5001/"
                    },
                    FrontChannelLogoutUri = "http://localhost:5001/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "api1" }
                },

                // SPA client using code flow + pkce
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                    },

                    PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5002" },

                    AllowedScopes = { "openid", "profile", "api1" }
                },
                // SPA client using code flow + pkce
                new Client
                {
                    ClientId = "eshoponweb-swagger",
                    ClientName = "Swagger Client",
                    ClientUri = "https://localhost:5001/swagger",
                    ClientSecrets = { new Secret("6a1b4db6-3b95-462b-a24b-eb061ae6bad3".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                    RequirePkce = false,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowRememberConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 60 * 60,
                    

                    RedirectUris =
                    {
                        "https://localhost:5001/swagger/oauth2-redirect.html",
                    },
                    
                    PostLogoutRedirectUris = { "https://localhost:5001/swagger/index.html" },
                    AllowedCorsOrigins = { "https://localhost:5001" },
                    AllowedScopes = {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "order:read",
                        "order:write",
                        "catalog-item:read",
                        "catalog-item:write",
                        "catalog-brand:read",
                        "catalog-brand:write",
                        "catalog-type:read",
                        "catalog-type:write"
                    }
                }
            };
    }
}