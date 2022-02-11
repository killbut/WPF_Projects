namespace Animation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel DataContext = new ViewModel();
            this.DataContext = DataContext;
            One.Command = DataContext.OpenFileCommand;
            One.CommandParameter = ListViewBox;

            RecordButton.Command = DataContext.ScreenRecordCommand;
            RecordButton.CommandParameter = StopRecordButton;

            StopRecordButton.Command = DataContext.StopScreenRecordCommand;
            StopRecordButton.CommandParameter = StopRecordButton;

            SaveButton.Command = DataContext.SaveCommand;

            Three.Command = DataContext.ImageToAviCommand;
        }
       
    }
}
