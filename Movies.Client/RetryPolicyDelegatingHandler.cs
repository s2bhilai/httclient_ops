using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client
{
    public class RetryPolicyDelegatingHandler: DelegatingHandler
    {
        private readonly int _maximumAmountOfRetries = 3;

        public RetryPolicyDelegatingHandler(int maximum)
        {

        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}
