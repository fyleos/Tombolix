using TradeUp.Server.Data;
using TradeUp.Server.Models;
using TradeUp.Shared.Models;

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
                }
                else
                {
                    var tmp_new = new List<string>(dataItem.Details);
                    tmp_new.AddRange(data.DataValue);
                    dataItem.Details = tmp_new.ToArray();
                }
            }

            return result;
        }

        public List<DrawResult> GetContextDrawResult(string contextId)
        {
            if (string.IsNullOrWhiteSpace(contextId) || !_userContextService.IsUserAuthenticated())
                return new List<DrawResult>();

            return _dbContext.DrawResults.Where(d => d.ContextId == contextId).ToList();
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
                return string.Empty;

            _dbContext.DrawContexts.Add(context);
            _dbContext.SaveChanges();
            return context.ID;
        }

        internal bool AddContextData(DrawData newData)
        {
            int tries = 50;
            while (tries > 0 && _dbContext.DrawDatas.Any(d => d.Id == newData.Id))
            {
                tries--;
                newData.Id = Guid.NewGuid().ToString();
            }

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
            int tries = 50;
            while (tries > 0 && _dbContext.DrawDatas.Any(d => d.Id == newItem.Id))
            {
                tries--;
                newItem.Id = Guid.NewGuid().ToString();
            }

            if (_dbContext.DrawItems.Any(d => d.Id == newItem.Id))
            {
                return false;
            }

            _dbContext.DrawItems.Add(newItem);
            _dbContext.SaveChanges();

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

        internal string GetDataRawId(TombolaData? info)
        {
            string tmp_rawId = string.Empty;

            foreach (var data in info.Details)
            {
                if (data is null)
                    continue;

                string tmp_tmp_rawId = _dbContext.DrawDatas.FirstOrDefault(d => d.DataValue == data).RawId;

                if(tmp_tmp_rawId != tmp_rawId)
                    tmp_rawId = tmp_tmp_rawId;
            }

            return tmp_rawId;
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
    }
}
