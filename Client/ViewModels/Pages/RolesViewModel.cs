using System.Collections.ObjectModel;
using TradeUp.Client.Services;
using TradeUp.Client.ViewModels.Shares;
using TradeUp.Shared.Models;

namespace TradeUp.Client.ViewModels.Pages
{
    public class RolesViewModel : BaseViewModel
    {
        public RolesViewModel(HttpClient httpClient): base(httpClient)
        {
        }

        public override void Initialize()
        {
            if (!UserLoggedService.IsUserLogged())
            {
                return;
            }

            base.Initialize();

            Task.Run(async () => await GetRolesAsync());

            if(ToolBarService is null)
                return;
            ToolBarService.AddListener(this);
            ToolBarService.SetReloadButton();
        }

        private async Task GetRolesAsync()
        {
            LogInfo("Fetching roles from API...");

            ObservableCollection<RoleDto>? roles = null;

            string query = BuildQueryApiV1String("roles"); 
            roles = await HandleRequest<ObservableCollection<RoleDto>>(query);

            if (roles != null)
            {
                Roles = roles;
                OnPropertyChanged(nameof(Roles));

                NotificationService?.AddSystemErrorNotification("Roles loaded successfully");
            }
            else
            {
                Roles = new ObservableCollection<RoleDto>();
                OnPropertyChanged(nameof(Roles));
            }
        }

        public ObservableCollection<RoleDto>? Roles { get; set; }   

        public void DeleteRole(string roleId)
        {
            if (Roles == null)
                return;

            var roleToDelete = Roles.FirstOrDefault(r => r.Id == roleId);
            if (roleToDelete != null)
            {
                Roles.Remove(roleToDelete);
            }

            //TODO: Appel API pour supprimer le rôle côté serveur
        }
    }
}
