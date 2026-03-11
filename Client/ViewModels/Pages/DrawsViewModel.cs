using TradeUp.Client.Services;
using TradeUp.Client.ViewModels.Shares;
using TradeUp.Shared.Models;

namespace TradeUp.Client.ViewModels.Pages
{
    public class DrawsViewModel: BaseViewModel
    {
        public List<DrawContextDTO> Draws
        {
            get
            {
                if (_drawService is not null)
                {
                    return _drawService.UserDraws;
                }

                return new List<DrawContextDTO>();
            }
        }

        private DrawService _drawService { get; set; }
        public DrawsViewModel(DrawService drawService) 
        {
            _drawService = drawService;
        }

        public override void Initialize()
        {
            if(_drawService is not null)
            {
                Task.Run(async () => 
                {
                    await _drawService.RefreshUserDrawsAsync();
                    OnPropertyChanged(nameof(Draws));
                });;
            }
            
            base.Initialize();
        }
    }
}
