//using System.Collections.Generic;
//using System.Threading.Tasks;
//using LinksMonitor.Interfaces.Stateful;
//using LinksMonitor.Interfaces.Stateless;
//using Orleans;
//using Orleans.Concurrency;

//namespace LinksMonitor.Grains.Stateless
//{
//    [StatelessWorker, Reentrant]
//    public class DiscoveryGrain : Grain, IDiscoveryGrain
//    {
//        private ILinkControllerGrain _linkController;
//        private IValidationUrlGrain _validationUrl;

//        public override Task OnActivateAsync()
//        {
//            _linkController = GrainFactory.GetGrain<ILinkControllerGrain>(0);
//            _validationUrl = GrainFactory.GetGrain<IValidationUrlGrain>(0);
//            return base.OnActivateAsync();
//        }

//        public async Task<LinkStatistics> GetStatisctics(string uri)
//        {
//            var isvalid = await _validationUrl.Validate(uri);
//            if (isvalid == false)
//            {
//                return new LinkStatistics { IsValid = false };
//            }

//            var response = await _linkController.Store(uri, withSubUrls: false);
//            return response.LinkStatistics;
//        }

//        public async Task<IList<string>> Dir(string uri)
//        {
//            var validUrls = new List<string>();

//            try
//            {
//                var response = await _linkController.Store(uri, withSubUrls: false);
//                var tasks = new List<Task<LinkInfo>>();

//                int loop = 0;
//                foreach (var item in await _validationUrl.ExtractValidUrls(htmlContent: response.HtmlContent))
//                {
//                    tasks.Add(_linkController.Store(item, withSubUrls: false));
//                    loop++;

//                    if (loop == 10)
//                    {
//                        await Task.WhenAll(tasks.ToArray());
//                        foreach (var task in tasks)
//                        {
//                            response = task.Result;
//                            if (response.LinkStatistics.IsValid)
//                            {
//                                validUrls.Add(response.LinkStatistics)
//                            }
//                        }
//                    }
//                }

//                if (loop < 10)
//                {
//                    await Task.WhenAll(tasks.ToArray());
//                }



//            }
//            catch (System.Exception)
//            {

//                int x = 0;
//            }


//            return validUrls;
//        }
//    }
//}



////public async Task<IList<string>> Dir(string uri)
////{
////    var validUrls = new List<string>();

////    try
////    {
////        var response = await _linkController.Store(uri, withSubUrls: false);
////        var tasks = new List<Task<LinkInfo>>();

////        int loop = 0;
////        foreach (var item in await _validationUrl.ExtractValidUrls(htmlContent: response.HtmlContent))
////        {
////            tasks.Add(_linkController.Store(item, withSubUrls: false));
////            loop++;

////            if (loop == 10)
////            {
////                await Task.WhenAll(tasks.ToArray());
////                foreach (var task in tasks)
////                {
////                    response = task.Result;
////                    if (response.LinkStatistics.IsValid)
////                    {
////                        validUrls.Add(response.LinkStatistics)
////                            }
////                }
////            }
////        }

////        if (loop < 10)
////        {
////            await Task.WhenAll(tasks.ToArray());
////        }



////    }
////    catch (System.Exception)
////    {

////        int x = 0;
////    }


////    return validUrls;
////}