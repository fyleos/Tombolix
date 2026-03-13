using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Text;
using TradeUp.Client.Enums;
using TradeUp.Client.Services;
using TradeUp.Shared.Models;

namespace TradeUp.Client.ViewModels.Shares
{
    public class HomeContentViewModel : BaseViewModel
    {
        private DrawService _drawService;

        public AppScreen AppScreen { get; private set; } = AppScreen.LoadFile;

        public HomeContentViewModel(DrawService drawService)
        {
            _drawService = drawService;
        }

        public DrawContextDTO? EditableContext {  get; set; }
        public DrawContextDTO DrawContext { get; private set; } = new DrawContextDTO();
        public bool isProcessing { get; private set; } = false; 
        public bool IsFirstLineHeader { get; set; } = true;
        public bool IsChoiceOnScreen => IsChoiceRequired();
        public bool IsChoiceAsked { get; set; } = false;

        public bool IsDrawContextEdition { get; set; } = false;

        private bool isTombolaDataFileRequired = false;

        public bool IsTombolaDataFileRequired 
        { 
            get 
            {
                return isTombolaDataFileRequired;
            }
            set
            {
                isTombolaDataFileRequired = value;
                OnPropertyChanged(nameof(IsTombolaDataFileRequired));
            }
        }

        public bool TombolaIsProgress { get; private set; } = false;

        public int? PrimaryInfoColumnIndex { get; private set; }
        public int? SecondaryInfoColumnIndex { get;private set; }

        private long maxFileSize = 1024 * 1024 * 5;

        private List<int> indexResults = new List<int>();
        public int? ResultIndex { get; private set; }

        public List<ResultDTO> Results 
        { 
            get
            {
                return DrawContext.Results;
            }
            set
            {
                DrawContext.Results = value;
                OnPropertyChanged(nameof(Results));
            }
        }

        public List<TombolaData>? TombolaDataList => DrawContext.DrawnItemsDatas;
        public TombolaData? TombolaDataHeader => DrawContext.DrawInfos;

        public string NewItemName { get; set; } = string.Empty;

        override public void Initialize()
        {
            base.Initialize();
            isProcessing = false;

            if (_drawService is not null && _drawService.CurrentDraw is not null)
            {
                DrawContext = _drawService.CurrentDraw;
                UpdateAppScreen(AppScreen.SetInfo);
            }
            else if(_drawService is not null)
            {
                DrawContext = _drawService.NewDraw();
                isTombolaDataFileRequired = true;

                UpdateAppScreen(AppScreen.LoadFile);
            }
            else
            {
                string tmp_name = $"tmpdraft-{DateTime.Now:yyyyMMdd-HHmmss}";
                DrawContext = new DrawContextDTO()
                {
                    Name = tmp_name,
                    DrawnItems = new(),
                    DrawnItemsDatas = null,
                    Results = new List<ResultDTO>()
                };

                isTombolaDataFileRequired = true;
                UpdateAppScreen(AppScreen.LoadFile);
            }

            OnPropertyChanged(nameof(Results));
            OnPropertyChanged(nameof(DrawContext));
            OnPropertyChanged(nameof(DrawContext.DrawInfos));
            OnPropertyChanged(nameof(isTombolaDataFileRequired));
            OnPropertyChanged(nameof(TombolaDataList));
        }

        public async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            bool isErrored = false;
            isProcessing = true;
            var file = e.File;
            DrawContext.DrawnItemsDatas = new List<TombolaData>();

            try
            {
                // 1. On ouvre un flux de lecture sur le fichier sélectionné
                using var stream = file.OpenReadStream(maxFileSize);
                using var reader = new StreamReader(stream, Encoding.UTF8);

                // 2. Lecture ligne par ligne
                string? line;
                int rowCount = 0;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var columns = line.Split(';'); // Ou ',' selon ton format
                    if(DrawContext.DrawInfos is null && IsFirstLineHeader)
                    {
                        DrawContext.DrawInfos = new TombolaData { Details = columns };
                        continue;
                    }

                    await ProcessCsvLine(columns);

                    rowCount++;
                }

                NotificationService.AddUserInfoNotification($"Ficher importé avec succès: {rowCount} lignes importées.");
            }
            catch (Exception ex)
            {
                isErrored = true;
                NotificationService.AddUserErrorNotification($"Erreur : {ex.Message}");
            }
            finally
            {
                if (!isErrored)
                {
                    IsTombolaDataFileRequired = false;
                    IsChoiceAsked = true;

                    UpdateAppScreen(AppScreen.SetInfo);
                }

                OnPropertyChanged(nameof(TombolaDataList)); 
                OnPropertyChanged(nameof(IsChoiceOnScreen));
                isProcessing = false;
            }
        }

        private void UpdateAppScreen(AppScreen newScreen)
        {
            AppScreen = newScreen;
            OnPropertyChanged(nameof(AppScreen));
        }

        private async Task ProcessCsvLine(string[] columns)
        {
            DrawContext.DrawnItemsDatas.Add(new TombolaData
            {
                Details = columns
            });
        }

        private bool IsChoiceRequired()
        {
            if(IsChoiceAsked)
            {
                return true;
            }

            return false;
        }

        public void HandleInfoSelected(int columnIndex)
        {
            if (PrimaryInfoColumnIndex is null)
                PrimaryInfoColumnIndex = columnIndex;
            else
                SecondaryInfoColumnIndex = columnIndex;

            OnPropertyChanged(nameof(Results));
            OnPropertyChanged(nameof(PrimaryInfoColumnIndex));
            OnPropertyChanged(nameof(SecondaryInfoColumnIndex));
            OnPropertyChanged(nameof(TombolaDataHeader));
        }

        public void HandlePrimaryInfoSelected(int columnIndex)
        {
            PrimaryInfoColumnIndex = columnIndex;

            if(SecondaryInfoColumnIndex == columnIndex)
                SecondaryInfoColumnIndex = null;

            OnPropertyChanged(nameof(PrimaryInfoColumnIndex));
            OnPropertyChanged(nameof(SecondaryInfoColumnIndex));
            OnPropertyChanged(nameof(TombolaDataHeader));
            OnPropertyChanged(nameof(Results));
        }

        public void ValidChoice()
        {
            IsChoiceAsked = false;
            OnPropertyChanged(nameof(IsChoiceOnScreen));

            UpdateAppScreen(AppScreen.LuckyDraw);
        }

        public void AskNewFile()
        {
            UpdateAppScreen(AppScreen.LoadFile);
        }

        public void AskChoice()
        {
            IsChoiceAsked = true;
            OnPropertyChanged(nameof(IsChoiceOnScreen));

            UpdateAppScreen(AppScreen.SetInfo);
        }

        public void SetUpDrawContext()
        {
            EditableContext = DrawContext.Clone();

            IsDrawContextEdition = true;
            OnPropertyChanged(nameof(EditableContext));
            OnPropertyChanged(nameof(IsDrawContextEdition));

            UpdateAppScreen(AppScreen.EditDraw);
        }
        public void HandleSaveAndCloseContext()
        {
            HandleSaveContext(true);
        }

        public void HandleSaveContext(bool closeOnSave = false)
        {
            if(EditableContext == null)
            {
                _drawService.SaveContextAsync(DrawContext);
                return;
            }

            DrawContext = EditableContext.Clone();

            if(closeOnSave)
            {
                EditableContext = null;
                IsDrawContextEdition = false;

                UpdateAppScreen(AppScreen.LuckyDraw);
            }

            OnPropertyChanged(nameof(EditableContext));
            OnPropertyChanged(nameof(IsDrawContextEdition));

            _drawService.SaveContextAsync(DrawContext);
        }

        public void RandomChoice()
        {
            TombolaIsProgress = true;

            int tries = 0;
            bool found = false;
            int result = -1;

            do
            {
                int tmp_result = new Random().Next(0, TombolaDataList?.Count ?? 0);
                tries++;

                if (!indexResults.Contains(tmp_result))
                {
                    found = true;
                    result = tmp_result;
                }
            }
            while (!found && tries < 200);

            if(result >= 0)
            {
                indexResults.Add(result);

                Task.Run(async () =>
                {
                    await Task.Delay(2000);

                    ResultIndex = result;

                    var tmp_result = new List<ResultDTO>(Results);
                    tmp_result.Add(new ResultDTO { TirageIndex = Results.Count + 1, Info = TombolaDataList?[ResultIndex ?? 0] });

                    Results = tmp_result;

                    HandleSaveContext();

                    TombolaIsProgress = false;

                    OnPropertyChanged(nameof(TombolaIsProgress));
                    OnPropertyChanged(nameof(ResultIndex));
                });
            }

            return;
        }

        public void HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                AddItem();
                HandleSaveContext();
            }
        }

        public void AddItem()
        {
            if (!string.IsNullOrWhiteSpace(NewItemName))
            {
                if(DrawContext.DrawnItems is null)
                {
                    EditableContext!.DrawnItems = new List<string>();
                }

                if (EditableContext!.DrawnItems.Contains(NewItemName))
                {
                    return;
                }

                EditableContext!.DrawnItems.Add(NewItemName);
                NewItemName = string.Empty;
                OnPropertyChanged(nameof(NewItemName));
                OnPropertyChanged(nameof(TombolaDataList));
            }
        }

        public void RemoveItem(string item)
        {
            EditableContext?.DrawnItems?.Remove(item);
            OnPropertyChanged(nameof(TombolaDataList));
        }
    }
}
