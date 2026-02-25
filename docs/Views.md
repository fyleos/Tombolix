# Documentation technique — Vues (Pages) côté client

Ce document décrit le fonctionnement des vues Razor côté client en se basant sur la vue `Client/Pages/Backend/AdminRoles.razor`. Il explique la structure, le cycle de vie, les liaisons avec le ViewModel, les interactions utilisateur et propose des bonnes pratiques applicables aux autres vues du projet Blazor / WebAssembly.

## Aperçu rapide
La vue `AdminRoles.razor` :
- Déclare des routes via `@page`.
- Hérite d'un composant conteneur : `@inherits StateContainerComponent<RolesViewModel>`.
- Utilise un layout dédié : `@layout AdminOverlayLayout`.
- Affiche un état de chargement si `ViewModel.Roles` est `null`.
- Rend une table des rôles et appelle `ViewModel.DeleteRole(id)` depuis un bouton.

## Structure et responsabilités
- Routes : définies par `@page "/admin/roles"` et `@page "/roles"`.
- Layout : `AdminOverlayLayout` fournit l'enrobage visuel/structurel commun.
- Héritage : `StateContainerComponent<TViewModel>` fournit une propriété `ViewModel` (instance de `RolesViewModel`), la gestion du cycle d'initialisation et la liaison des notifications vers l'UI (probablement via `INotifyPropertyChanged` → `StateHasChanged()`).
- Localisation : `T.TEXT("...")` centralise les libellés (i18n).

## Rendu et liaison de données
- Vérification de chargement :
  - Si `ViewModel.Roles is null` → message `LOADING`.
  - Sinon → boucle `@foreach (var role in ViewModel.Roles)` pour générer les lignes.
- `ViewModel.Roles` est une `ObservableCollection<RoleDto>` : les modifications d'items (ajout/suppression) déclenchent des notifications UI.
- Quand la collection entière est remplacée, le ViewModel appelle `OnPropertyChanged(nameof(Roles))` pour forcer le rafraîchissement.

## Actions utilisateur
- Suppression : le bouton appelle `@onclick="() => ViewModel.DeleteRole(role.Id)"`.
  - Dans l'implémentation actuelle, `DeleteRole` modifie uniquement la collection côté client et contient un TODO pour l'appel API serveur.

## Points d'attention et recommandations
1. Initialisation asynchrone
   - Éviter `Task.Run(...)` dans `Initialize()` : préférer une méthode asynchrone `Task InitializeAsync(CancellationToken ct)` exposée par le composant conteneur, pour propager erreurs/annulation.
2. Suppression (Delete)
   - Rendre la méthode `DeleteRole` asynchrone et appeler l'API serveur (`DELETE api/admin/roles/{id}`).
   - Valider le statut HTTP avant de retirer l'item localement, ou utiliser une UI optimiste avec rollback en cas d'échec.
   - Ajouter confirmation utilisateur (modal ou `confirm`) avant suppression.
   - Exemple minimal recommandé dans le ViewModel :
     ```csharp
     // Exemple pseudo-code à placer dans RolesViewModel
     public async Task DeleteRoleAsync(string roleId)
     {
         var resp = await _httpClient.DeleteAsync($"api/admin/roles/{roleId}");
         if (resp.IsSuccessStatusCode)
         {
             var role = Roles?.FirstOrDefault(r => r.Id == roleId);
             if (role != null) Roles.Remove(role);
         }
         else
         {
             DebugLog($"Delete failed: {resp.StatusCode}");
             // exposer ErrorMessage pour affichage
         }
     }
     ```
3. Gestion des erreurs et état
   - Exposer `IsLoading`, `IsBusy`, `ErrorMessage` dans les ViewModels.
   - Gérer les exceptions réseau et afficher des messages utilisateur clairs.
4. Concurrence et annulation
   - Accepter un `CancellationToken` pour les appels réseau et annuler lors du disposal du composant.
5. Accessibilité et UX
   - Ajouter attributs ARIA et feedback visuel (spinners, disabled buttons) lors d'opérations longues.
6. Testabilité
   - Extraire les appels HTTP derrière un service testable ou utiliser `HttpClient` injecté avec handler mockable.
7. Cohérence
   - Standardiser l'initialisation et la notification d'état pour tous les ViewModels (modèle unique : `InitializeAsync`, `DisposeAsync`, `IsLoading`).

## Checklist d'implémentation pour une autre vue
- [ ] Déclarer routes et layout.
- [ ] Hériter de `StateContainerComponent<TViewModel>` ou utiliser injection explicite.
- [ ] Afficher `IsLoading`/placeholder tant que les données ne sont pas disponibles.
- [ ] Utiliser `ObservableCollection<T>` pour collections mutables ou notifier remplacement d'instance.
- [ ] Toutes les actions modifiant le serveur : implémenter en async, traiter les erreurs et confirmer avant suppression destructive.
- [ ] Ajouter tests unitaires / intégration pour logique ViewModel.

## Questions utiles pour compléter la documentation
- Quelle est la signature exacte de `StateContainerComponent<T>` (gestion de `ViewModel`, abonnements) ?
- `BaseViewModel` expose-t-il `IsLoading`, `ErrorMessage`, `ToolBarService` ? Quelle est l'orthographe exacte de `AddListenner` ?
- Existe-t-il une politique d'annulation partagée (CancellationToken) ou lifecycle pour les pages ?
- Convention de gestion d'erreurs et localisation des messages d'erreur.

---
Ce document sert de template technique pour documenter les autres vues du projet : expliquer structure, liaisons ViewModel, cycle d'initialisation, gestion d'état et recommandations pratiques.