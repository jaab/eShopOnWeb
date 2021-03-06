using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Infrastructure.Services.CurrencyService
{
    public class CurrencyServiceExternal : ICurrencyService
    {
        /// <inheritdoc />
        public Task<decimal> Convert(decimal value, Currency source, Currency target, CancellationToken cancellationToken = default)
        {
            //throw new System.NotImplementedException();
            return Task.FromResult(value);
        }
    }
}