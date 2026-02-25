using Microsoft.AspNetCore.Components.Forms;
using System.Text;
using TradeUp.Client.Models;

namespace TradeUp.Client.ViewModels.Shares
{
    public class HomeContentViewModel : BaseViewModel
    {
        public bool isProcessing { get; private set; } = false; 
        public bool IsFirstLineHeader { get; set; } = true;
        public bool IsChoiceOnScreen => IsChoiceRequired();
        public bool IsChoiceAsked { get; set; } = false;
        public bool TombolaIsProgress { get; private set; } = false;

        public int? PrimaryInfoColumnIndex { get; private set; }
        public int? SecondaryInfoColumnIndex { get;private set; }

        private long maxFileSize = 1024 * 1024 * 5;

        private List<int> indexResults = new List<int>();
        public int? ResultIndex { get; private set; }

        public List<TombolaData>? TombolaDataList { get; set; }
        public TombolaData? TombolaDataHeader { get; set; }

        override public void Initialize()
        {
            base.Initialize();
            isProcessing = false;

            TombolaDataList = null;
            TombolaDataHeader = null;
        }

        public async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            isProcessing = true;
            var file = e.File;

            TombolaDataList = new List<TombolaData>();

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
                    if(TombolaDataHeader is null && IsFirstLineHeader)
                    {
                        TombolaDataHeader = new TombolaData { Details = columns };
                        continue;
                    }

                    await ProcessCsvLine(columns);

                    rowCount++;
                }

                NotificationService.AddUserInfoNotification($"Ficher importé avec succès: {rowCount} lignes importées.");
            }
            catch (Exception ex)
            {
                NotificationService.AddUserErrorNotification($"Erreur : {ex.Message}");
            }
            finally
            {
                IsChoiceAsked = true;
                OnPropertyChanged(nameof(TombolaDataList)); 
                OnPropertyChanged(nameof(IsChoiceOnScreen));
                isProcessing = false;
            }
        }

        private async Task ProcessCsvLine(string[] columns)
        {
            TombolaDataList.Add(new TombolaData
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
        }

        public void ValidChoice()
        {
            IsChoiceAsked = false;
            OnPropertyChanged(nameof(IsChoiceOnScreen));
        }

        public void AskChoice()
        {
            IsChoiceAsked = true;
            OnPropertyChanged(nameof(IsChoiceOnScreen));
        }

        public void RandomChoice()
        {
            TombolaIsProgress = true;

            int tries = 0;
            int result;
            do
            {
                result = new Random().Next(0, TombolaDataList?.Count ?? 0);
                tries++;
            }
            while (indexResults.Contains(result) && tries < 200);

            indexResults.Add(result);

            Task.Run(async () =>
            {
                await Task.Delay(2000);
                TombolaIsProgress = false;

                ResultIndex = result;
                OnPropertyChanged(nameof(TombolaIsProgress));
                OnPropertyChanged(nameof(ResultIndex));
            });
        }
    }
}
