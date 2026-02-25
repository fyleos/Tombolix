using System.Net.Http.Json;
using TradeUp.Client.Services;
using TradeUp.Client.ViewModels.Shares;
using TradeUp.Shared.Models;

namespace TradeUp.Client.ViewModels.Pages.Identity
{
    public class ProfileViewModel: BaseViewModel
    {
        private readonly UsersService _usersService;
        public ProfileViewModel(HttpClient http, UsersService usersService) : base(http)
        {
            IsLoading = false;
            _usersService = usersService;
        }
        private bool _isLoading;
        public bool IsLoading { get
            {
                return _isLoading;

            }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public UserDto? User { get; private set; }


        public async Task LoadUserProfile()
        {
            IsLoading = true;
            try
            {
                var user = await _httpClient?.GetFromJsonAsync<UserDto>($"{API_V1_BASE_ROUTE}/users/me");
                if (user != null)
                {
                    User = user;
                }
                else
                {
                    NotificationService?.AddUserErrorNotification("ERROR_UNKNOWN_USER");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task UpdateProfile()
        {
            IsLoading = true;
            if(User == null)
            {
                Console.WriteLine("User is null");
                return;
            }
            UserDto? user = new UserDto
            {
                Id = User.Id,
                Email = User.Email,
                Name = User.Email,
                DisplayName = User.DisplayName,
                Roles = User.Roles

            };

            await _usersService.SaveUserAsync(user);

            await LoadUserProfile();

            IsLoading = false;
        }
    }
}
