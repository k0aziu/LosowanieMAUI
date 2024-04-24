namespace LosowanieMK
{
    public partial class MainPage : ContentPage
    {
        private Dictionary<string, List<string>> studentsInClass = new Dictionary<string, List<string>>();

        public MainPage()
        {
            InitializeComponent();
            LoadStudents();
        }

        private async void ButtonPickFileClicked(object sender, EventArgs e)
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".txt" } },
                });

                var options = new PickOptions
                {
                    PickerTitle = "Wybierz plik",
                    FileTypes = customFileType,
                };

                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    var filePath = result.FullPath;
                    var fileContent = await File.ReadAllTextAsync(filePath);
                    textInput.Text = fileContent;
                    LoadStudents();
                    Console.WriteLine($"Wybrany plik: {result.FileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadStudents();
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "students.txt");
            await SaveToFileAsync("students.txt", e.NewTextValue);
            var fileContent = await File.ReadAllTextAsync(filePath);
            textInput.Text = fileContent;
        }

        private async Task SaveToFileAsync(string fileName, string content)
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, fileName);
            try
            {
                await File.WriteAllTextAsync(path, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Jakiś błąd czy coś: {ex.Message}");
            }
        }

        private void ButtonDrawStudentClicked(object sender, EventArgs e)
        {
            if (classPicker.SelectedIndex < 0)
            {
                resultLabel.Text = "Proszę wybrać najpierw klasę.";
                return;
            }

            var selectedClass = classPicker.Items[classPicker.SelectedIndex];
            DrawStudent(selectedClass);
        }

        private void LoadStudents()
        {
            if (textInput == null || string.IsNullOrEmpty(textInput.Text)) return;

            studentsInClass.Clear();
            classPicker.Items.Clear();

            try
            {
                studentsInClass = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(textInput.Text);
                foreach (var classItem in studentsInClass.Keys)
                {
                    classPicker.Items.Add(classItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DrawStudent(string classItem)
        {
            if (studentsInClass.ContainsKey(classItem) && studentsInClass[classItem].Count > 0)
            {
                var random = new Random();
                var drawnStudent = studentsInClass[classItem][random.Next(studentsInClass[classItem].Count)];
                resultLabel.Text = $"Wylosowany uczeń: {drawnStudent}";
            }
            else
            {
                resultLabel.Text = "W tej klasie nie ma uczniów.";
            }
        }
    }

}
