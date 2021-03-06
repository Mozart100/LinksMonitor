﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace LinksMonitor.Interfaces.Stateless
{
    public interface IValidationUrlGrain : IGrainWithIntegerKey
    {
        Task<bool> Validate(string uri);

        Task<IList<string>> ExtractValidUrls(string htmlContent);
    }
}
