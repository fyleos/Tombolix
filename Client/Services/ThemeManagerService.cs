
namespace TradeUp.Client.Services
{
    public class ThemeManagerService
    {
        private StateContainerService StateContainerService { get; set; }
        protected UserOptionsService UserOptionsService { get; set; }

        private string _currentTheme = "theme-light";
        public string CurrentTheme 
        { 
            get 
            {
                if(UserOptionsService is not null && UserOptionsService.UserOption is not null)
                {
                    return UserOptionsService.UserOption.CurrentTheme;
                }
                else
                {
                    return _currentTheme;
                }
            }
            private set
            {
                if(UserOptionsService is not null && UserOptionsService.UserOption is not null)
                {
                    UserOptionsService.UpdateUserOptionItem("CurrentTheme", value);
                }
                else
                {
                    if (UserOptionsService is not null)
                    {
                        UserOptionsService.TryGetUserOption();
                    }
                }

                _currentTheme = value;
                StateContainerService.ShouldRefresh = true;
            }
        }

        public ThemeManagerService(UserOptionsService userOptionsService, StateContainerService stateContainerService) 
        {
            StateContainerService = stateContainerService;
            UserOptionsService = userOptionsService;
        }


        public void SwitchTheme()
        {
            if(UserOptionsService is not null)
            {
                SwitchThemeFromUserOption();
            }
            else
            {
                SwitchLocalTheme();
            }
        }

        private void SwitchThemeFromUserOption()
        {
            if(UserOptionsService.UserOption is null)
            {
                UserOptionsService.TryGetUserOption();

                if(UserOptionsService.UserOption is null)
                {
                    SwitchLocalTheme();
                    return;
                }
            }

            UserOptionsService.UpdateUserOptionItem("CurrentTheme", CurrentTheme == "theme-light" ? "theme-dark" : "theme-light");            
        }

        private void SwitchLocalTheme()
        {
            CurrentTheme = CurrentTheme == "theme-light" ? "theme-dark" : "theme-light";
        }
    }
}
