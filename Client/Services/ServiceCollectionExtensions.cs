using System.Reflection;

namespace TradeUp.Client.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddViewModels(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var viewModels = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("ViewModel")
                            && t.IsClass
                            && !t.IsAbstract);

            foreach (var viewModel in viewModels)
            {
                services.AddScoped(viewModel);
            }
        }
    }
}
