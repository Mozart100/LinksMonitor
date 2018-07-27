using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinksMonitor.Interfaces.Stateless;
using Orleans;
using Orleans.Concurrency;

namespace LinksMonitor.Grains.Stateless
{
    [StatelessWorker]
    public class ValidationUrlGrain : Grain, IValidationUrlGrain
    {
        private IList<Func<string, bool>> _filters;

        public ValidationUrlGrain()
        {
            _filters = new List<Func<string, bool>> { RegularSites, HttpSites };
        }

        public async Task<bool> Validate(string uri)
        {
            foreach (var filter in _filters)
            {
                var result = filter(uri);

                if (result == true)
                {
                    return true;
                }

            }
            return false;
        }


        private bool RegularSites(string uri)
        {
            //walla.co.il
            var regex = new Regex(@"^[\w.]+\.\w{2,6}\.\w{2,4}", RegexOptions.IgnoreCase);
            var result = regex.Match(uri);

            return result.Success;
        }

        private bool HttpSites(string uri)
        {
            //http://en.wikipedia.org/
            var regex = new Regex(@"^https?://.+\.\w{2,4}", RegexOptions.IgnoreCase);
            var result = regex.Match(uri);

            return result.Success;
        }
    }
}
