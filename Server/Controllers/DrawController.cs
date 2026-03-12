using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeUp.Server.Models;
using TradeUp.Server.Services;
using TradeUp.Shared.Models;

namespace TradeUp.Server.Controllers
{
    public class DrawController : BaseApiV1Controller
    {
        private DrawServerService _drawService;
        public DrawController(LicenceService licenceService, UserContextService userContextService, DrawServerService drawService) : base(licenceService, userContextService)
        {
            _drawService = drawService;
        }

        [HttpGet("contexts/me")]
        [Authorize]
        public ActionResult<List<DrawContext>> GetUserContexts()
        {
            if (!IsUserLoggedIn())
                return Unauthorized();

            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
                return NotFound();

            return Ok(_drawService.GetUserDrawContexts(userId));
        }

        [HttpGet("context/results/{contextId}")]
        [Authorize]
        public ActionResult<List<DrawResult>> GetContextResults([FromRoute] string contextId)
        {
            if (!IsUserLoggedIn())
                return Unauthorized();

            var userId = _userContextService.GetCurrentUserId();
            var context = _drawService.GetContextById(contextId);

            if (userId is null ||
                context is null ||
                userId != context.UserId)
                return NotFound();

            return Ok(_drawService.GetContextDrawResult(contextId));
        }

        [HttpGet("context/{contextId}")]
        [Authorize]
        public ActionResult<DrawContext> GetContext([FromRoute] string contextId)
        {
            if (!IsUserLoggedIn())
                return Unauthorized();

            var userId = _userContextService.GetCurrentUserId();
            var context = _drawService.GetContextById(contextId);

            if (userId is null ||
                context is null ||
                userId != context.UserId)
                return NotFound();

            return Ok(context);
        }

        [HttpGet("context/datas/{contextId}")]
        [Authorize]
        public ActionResult<List<TombolaData>> GetContextDatas([FromRoute] string contextId)
        {
            if (!IsUserLoggedIn())
                return Unauthorized();

            var userId = _userContextService.GetCurrentUserId();
            var context = _drawService.GetContextById(contextId);

            if (userId is null ||
                context is null ||
                userId != context.UserId)
                return NotFound();

            return Ok(_drawService.GetContextDrawData(contextId));
        }

        [HttpGet("context/headers/{contextId}")]
        [Authorize]
        public ActionResult<List<TombolaData>> GetContextHeaders([FromRoute] string contextId)
        {
            if (!IsUserLoggedIn())
                return Unauthorized();

            var userId = _userContextService.GetCurrentUserId();
            var context = _drawService.GetContextById(contextId);

            if (userId is null ||
                context is null ||
                userId != context.UserId)
                return NotFound();

            return Ok(_drawService.GetContextDrawDataHeaders(contextId));
        }

        [HttpGet("context/items/{contextId}")]
        [Authorize]
        public ActionResult<List<DrawData>> GetContextItems([FromRoute] string contextId)
        {
            if (!IsUserLoggedIn())
                return Unauthorized();

            var userId = _userContextService.GetCurrentUserId();
            var context = _drawService.GetContextById(contextId);

            if (userId is null ||
                context is null ||
                userId != context.UserId)
                return NotFound();

            var result = _drawService.GetContextDrawItem(contextId);
            return Ok(result);
        }

        [HttpPost("context/save")]
        [Authorize]
        public ActionResult AddDrawContext([FromBody] DrawContextDTO newContext) 
        {
            if(!IsUserLoggedIn())
                return Unauthorized();

            var userId = _userContextService.GetCurrentUserId();
            if(string.IsNullOrEmpty(userId))
            {
                return BadRequest("Error during user authentication");
            }

            //string newId = string.Empty;
            //if (_drawService.IsContextIdAlreadyExist(newContext.ID))
            //{
            //    int tries = 50;
            //    do
            //    {
            //        newId = Guid.NewGuid().ToString();
            //        tries--;
            //    }
            //    while (_drawService.IsContextIdAlreadyExist(newId) && tries > 0);
            //}
            //else
            //{
            //    newId = newContext.ID;
            //}

            //if (_drawService.IsContextIdAlreadyExist(newId))
            //    return BadRequest("Unable to find free Context Id");

            DrawContext context = new DrawContext()
            {
                ID = newContext.ID,
                Name = newContext.Name,
                UserId = userId
            };

            string newContextId = _drawService.AddDrawContext(context);

            if(SaveContextDatas(newContext,newContextId) &&
                SaveContextItems(newContext, newContextId) &&
                SaveContextResults(newContext, newContextId)
                )
            {
                return Ok(newContextId);
            }

            return BadRequest("Error during save");
        }

        //[HttpPut("context/save")]
        //[Authorize]
        //public ActionResult SaveDrawContext([FromBody] DrawContextDTO newContext)
        //{
        //    return Ok();
        //}

        private bool SaveContextResults(DrawContextDTO newContext, string newContextId)
        {
            if (newContext.Results is null)
                return true;

            bool result = true;

            for (int i = 0; i < newContext.Results?.Count; i++)
            {
                var itemReference = newContext.Results[i].Info?.Details;
                var listFound = newContext.DrawnItemsDatas.FirstOrDefault(d => d.Details.SequenceEqual(itemReference));

                string rawId = _drawService.GetDataRawId(listFound);

                DrawResult newItem = new DrawResult()
                {
                    ContextId = newContextId,
                    DataRawId = rawId,
                    TirageIndex = newContext.Results[i].TirageIndex,
                    Id = Guid.NewGuid().ToString()
                };

                result = _drawService.AddContextResult(newItem);
            }

            return result;
        }

        private bool SaveContextItems(DrawContextDTO newContext, string newContextId)
        {
            if (newContext.DrawnItems is null)
                return true;

            bool result = true;

            for (int i = 0; i < newContext.DrawnItems?.Count; i++)
            {
                var item = newContext.DrawnItems[i];

                DrawItem newItem = new DrawItem()
                {
                    DrawContextId = newContextId,
                    Name = item,
                    Id = Guid.NewGuid().ToString()
                };

                result = _drawService.AddContextItem(newItem);
            }

            return result;
        }

        private bool SaveContextDatas(DrawContextDTO newContext, string newContextId)
        {
            if(newContext.DrawnItemsDatas is null)
                return true;

            bool result = true;

            for(int i = 0; i < newContext.DrawnItemsDatas?.Count; i++)
            {
                var dataRaw = newContext.DrawnItemsDatas[i];
                string dataRawId = Guid.NewGuid().ToString();

                int tries = 50;
                while (_drawService.IsDataRawIdAlredyExist(dataRawId) && tries > 0)
                {
                    tries--;
                    dataRawId = Guid.NewGuid().ToString();
                }

                if (_drawService.IsDataRawIdAlredyExist(dataRawId))
                {
                    result = false;
                    continue;
                }

                for (int j = 0; j < dataRaw.Details.Length; j++)
                {
                    string dataId = Guid.NewGuid().ToString();

                    int jTries = 50;
                    while (_drawService.IsDataIdAlredyExist(dataId) && tries > 0)
                    {
                        jTries--;
                        dataId = Guid.NewGuid().ToString();
                    }

                    if (_drawService.IsDataIdAlredyExist(dataRawId))
                    {
                        result = false;
                        continue;
                    }

                    DrawData newData = new DrawData()
                    {
                        Id = dataId,
                        DataKey = newContext.DrawInfos?.Details[j] ?? string.Empty,
                        DataValue = dataRaw.Details[j] ?? string.Empty,
                        DrawContextId = newContextId,
                        RawId = dataRawId,
                    };


                    result = _drawService.AddContextData(newData);
                }
            }

            return result;
        }
    }
}
