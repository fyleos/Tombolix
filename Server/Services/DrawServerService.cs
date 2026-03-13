using Microsoft.AspNetCore.Http.HttpResults;
using NuGet.Packaging.Signing;
using System.Linq;
using TradeUp.Server.Data;
using TradeUp.Server.Models;
using TradeUp.Shared.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TradeUp.Server.Services
{
    public class DrawServerService
    {
        private ApplicationDbContext _dbContext;
        private UserContextService _userContextService;
        public DrawServerService(ApplicationDbContext db, UserContextService userContext )
        {
            _dbContext = db;
            _userContextService = userContext;
        }

        public List<DrawContext> GetUserDrawContexts(string userId)
        {
            if (userId is null || userId != _userContextService.GetCurrentUserId())
                return new List<DrawContext>();

            List<DrawContext> results = _dbContext.DrawContexts.Where(c => c.UserId == userId).ToList();

            return results;
        }

        public List<TombolaData> GetContextDrawData(string contextId) 
        {
            if(string.IsNullOrWhiteSpace(contextId) || !_userContextService.IsUserAuthenticated()) 
                return new List<TombolaData>();

            var datas = _dbContext.DrawDatas.Where(d => d.DrawContextId == contextId).ToList();
            var result = new List<TombolaData>();

            datas = datas.OrderBy(r => r.RawId).ToList();

            string rawId = string.Empty;
            TombolaData dataItem = new TombolaData();

            foreach(var data in datas)
            {
                if(rawId != data.RawId)
                {
                    rawId = data.RawId;
                    result.Add(new TombolaData() { Details = dataItem.Details });

                    dataItem = new TombolaData();
;
                }

                var tmp_new = new List<string>(dataItem.Details);
                tmp_new.AddRange(data.DataValue);
                dataItem.Details = tmp_new.ToArray();
            }

            return result;
        }

        public List<ResultDTO> GetContextDrawResult(string contextId)
        {
            if (string.IsNullOrWhiteSpace(contextId) || !_userContextService.IsUserAuthenticated())
                return new List<ResultDTO>();

            var tmp_results = _dbContext.DrawResults.Where(d => d.ContextId == contextId).ToList();
            var result = new List<ResultDTO>();

            foreach (var tmpResult in tmp_results)
            {
                var rawId = tmpResult.DataRawId;
                var datasItem = _dbContext.DrawDatas.Where(d => d.RawId == rawId);

                if (datasItem is not null)
                {
                    var details = new List<string>();

                    foreach (var data in datasItem)
                    {
                        details.Add(data.DataValue);
                    }

                    result.Add(new ResultDTO()
                    {
                        Info = new TombolaData() { Details = details.ToArray() },
                        TirageIndex = tmpResult.TirageIndex
                    });
                }
            };

            return result;
        }

        public List<DrawItem> GetContextDrawItem(string contextId)
        {
            if (string.IsNullOrWhiteSpace(contextId) || !_userContextService.IsUserAuthenticated())
                return new List<DrawItem>();

            return _dbContext.DrawItems.Where(d => d.DrawContextId == contextId).ToList();
        }

        internal DrawContext GetContextById(string contextId)
        {
            if (string.IsNullOrWhiteSpace(contextId) || !_userContextService.IsUserAuthenticated())
                return new DrawContext();

            var context = _dbContext.DrawContexts.FirstOrDefault(c => c.ID == contextId && c.UserId == _userContextService.GetCurrentUserId());
            
            if(context == null)
                return new DrawContext();
            
            return context;
        }

        internal bool IsContextIdAlreadyExist(string iD)
        {
            return _dbContext.DrawContexts.Any(c => c.ID == iD);
        }

        internal string AddDrawContext(DrawContext context)
        {
            if(_dbContext.DrawContexts.Any(c => c.ID == context.ID))
                _dbContext.DrawContexts.Update(context);
            else
                _dbContext.DrawContexts.Add(context);
            
            _dbContext.SaveChanges();
            return context.ID;
        }

        internal bool AddContextData(DrawData newData)
        {
            if(_dbContext.DrawDatas.Any(d => d.Id == newData.Id))
            {
                return false;
            }

            _dbContext.DrawDatas.Add(newData);
            _dbContext.SaveChanges();

            return true;
        }

        internal bool AddContextItem(DrawItem newItem)
        {
            var oldItem = _dbContext.DrawItems.FirstOrDefault(i => i.Name == newItem.Name && i.DrawContextId == newItem.DrawContextId) ?? null;

            if(oldItem is null)
            {
                _dbContext.DrawItems.Add(newItem);
                _dbContext.SaveChanges();
            }

            return true;
        }

        internal bool AddContextResult(DrawResult newItem)
        {
            int tries = 50;
            while (tries > 0 && _dbContext.DrawResults.Any(d => d.Id == newItem.Id))
            {
                tries--;
                newItem.Id = Guid.NewGuid().ToString();
            }

            if (_dbContext.DrawResults.Any(d => d.Id == newItem.Id))
            {
                return false;
            }

            _dbContext.DrawResults.Add(newItem);
            _dbContext.SaveChanges();

            return true;
        }

        internal bool IsDataRawIdAlredyExist(string dataRawId)
        {
            return _dbContext.DrawDatas.Any(d => d.RawId == dataRawId);
        }

        internal bool IsDataIdAlredyExist(string dataId)
        {
            return _dbContext.DrawDatas.Any(d => d.Id == dataId);
        }

        internal string GetDataRawId(TombolaData? info, string contextId)
        {
            if(info is null || info!.Details.Length == 0)
                return string.Empty;

            int tries = 100;
            string tmp_rawId; 
            string result = string.Empty;

            while (tries > 0 && result == string.Empty)
            {
                tries--;
                List<DrawData> possibleResults = _dbContext.DrawDatas.Where(d => d.DataValue == info!.Details[0] && d.DrawContextId == contextId).ToList();

                if (possibleResults.Any()) 
                {
                    for(int i = possibleResults.Count() - 1; i >= 0; i--)
                    {
                        var possibleResult = possibleResults[i];
                        if (!info.Details.Any(d => d == possibleResult.DataValue))
                        {
                            string wrongRawId = possibleResult.RawId;

                            possibleResults.RemoveAll(d => d.RawId == wrongRawId);
                        }
                    }

                    result = possibleResults.First().RawId;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        internal TombolaData GetContextDrawDataHeaders(string contextId)
        {
            if (string.IsNullOrWhiteSpace(contextId) || !_userContextService.IsUserAuthenticated())
                return new TombolaData();

            var datas = _dbContext.DrawDatas.Where(d => d.DrawContextId == contextId).ToList();
            var result = new TombolaData();

            datas = datas.OrderBy(r => r.RawId).ToList();

            string rawId = datas.First().RawId;
            var tmp_data_list = datas.Where(d => d.RawId == rawId);
            List<string> tmp_headers = new List<string>();

            foreach (var data in tmp_data_list)
            {
                tmp_headers.Add(data.DataKey);
            }

            result.Details = tmp_headers.ToArray();

            return result;
        }

        internal bool IsIdAlreadyExist(string newId)
        {
            var result =
                !IsContextIdAlreadyExist(newId) &&
                !IsDataIdAlredyExist(newId) &&
                !IsDataRawIdAlredyExist(newId) &&
                !IsResultIdAlreadyExist(newId);

            return result;
        }

        private bool IsResultIdAlreadyExist(string newId)
        {
            return _dbContext.DrawResults.Any(c => c.Id == newId);
        }

        internal bool DeleteContextAndDatasById(string contextId)
        {
            DrawContext? context = _dbContext.DrawContexts.FirstOrDefault(c => c.ID == contextId);

            if (context is null) 
                return false;

            if(!RemoveRelatedData(contextId)) return false;

            _dbContext.DrawContexts.Remove(context);
            _dbContext.SaveChanges();

            return true;
        }

        private bool RemoveRelatedData(string contextId)
        {
            var relatedResults = _dbContext.DrawResults.Where(r => r.ContextId == contextId);
            var relatedDatas = _dbContext.DrawDatas.Where(r => r.DrawContextId == contextId);
            var relatedItems = _dbContext.DrawItems.Where(r => r.DrawContextId == contextId);

            _dbContext.DrawResults.RemoveRange(relatedResults);
            _dbContext.DrawDatas.RemoveRange(relatedDatas);
            _dbContext.DrawItems.RemoveRange(relatedItems);

            return true;
        }
    }
}
