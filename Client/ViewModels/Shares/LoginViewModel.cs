using TradeUp.Client.Services;
using TradeUp.Shared.Models;

namespace TradeUp.Client.ViewModels.Shares
{
    public class LoginViewModel: BaseViewModel
    {
        public readonly ThemeManagerService ThemeManager;
        private readonly UsersService _usersService;

        private UserDto? _user;
        public UserDto? User { 
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }
        public LoginViewModel(ThemeManagerService themeManagerService, UsersService usersService)
        {
            ThemeManager = themeManagerService;
            _usersService = usersService;
        }

        public override void Initialize()
        {
            Task.Run(async () => 
            {
                User = await _usersService.GetCurrentUserAsync();
            });
            base.Initialize();
        }
        
    }
}
