using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;

namespace Daarto.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetClaimValue(this IPrincipal principal, string claimType) {
            var user = principal as ClaimsPrincipal;

            var values = user.Identities
                             .SelectMany(identities => identities.Claims.Where(claim => claim.Type == claimType)
                             .Select(claim => claim.Value));

            return values.FirstOrDefault();
        }

        public static T GetClaimValue<T>(this IPrincipal principal, string claimType) where T : struct {
            var user = principal as ClaimsPrincipal;

            return user.Identities
                       .SelectMany(i => i.Claims.Where(c => c.Type == claimType))
                       .FindFirstValue<T>(claimType);
        }

        public static T? GetOptionalClaimValue<T>(this IPrincipal principal, string claimType) where T : struct {
            var user = principal as ClaimsPrincipal;

            if (TryFindFirstValue(user.Identities.SelectMany(identities => identities.Claims.Where(claim => claim.Type == claimType)), claimType, out T value)) {
                return value;
            } else {
                return null;
            }
        }

        public static T FindFirstValue<T>(this IEnumerable<Claim> claims, string claimType) where T : struct {
            TryFindFirstValue(claims, claimType, out T value);
            return value;
        }

        public static bool TryFindFirstValue<T>(IEnumerable<Claim> claims, string claimType, out T result) where T : struct {
            result = default(T);
            var values = claims.Where(c => c.Type == claimType).Select(c => c.Value);
            var valueString = values.FirstOrDefault();
            object value = default(T);

            if (valueString == null) {
                result = (T)value;
                return false;
            }

            var type = typeof(T);

            if (type.GetTypeInfo().IsEnum) {
                value = Enum.Parse(type, valueString, true);
            } else if (type == typeof(bool)) {
                value = bool.Parse(valueString);
            } else if (type == typeof(int)) {
                value = int.Parse(valueString);
            } else if (type == typeof(Guid)) {
                value = Guid.Parse(valueString);
            } else if (type == typeof(double)) {
                value = double.Parse(valueString, CultureInfo.InvariantCulture);
            } else if (type == typeof(DateTime)) {
                value = DateTime.Parse(valueString, CultureInfo.InvariantCulture);
            } else if (type == typeof(TimeSpan)) {
                value = TimeSpan.Parse(valueString, CultureInfo.InvariantCulture);
            }

            result = (T)value;
            return true;
        }

        public static string GetPhotoUrl(this ClaimsPrincipal user) => GetClaimValue(user, ClaimTypes.Uri);

        public static string GetDisplayName(this ClaimsPrincipal principal) {
            var displayName = default(string);
            var name = principal.FindFirst(ClaimTypes.Name)?.Value;
            var firstName = principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = principal.FindFirst(ClaimTypes.Surname)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            if (!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName)) {
                displayName = $"{firstName} {lastName}".Trim();
            } else if (!string.IsNullOrEmpty(name)) {
                displayName = name;
            } else if (!string.IsNullOrEmpty(email)) {
                displayName = email;
            }

            return displayName;
        }
    }
}
