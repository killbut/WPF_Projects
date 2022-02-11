using Microsoft.Win32;
using ReactiveUI;
using SharpAvi.Codecs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;

namespace Animation
{
    public class ViewModel : ReactiveObject
    {
        private ObservableCollection<string> _ObsCollFilePath;
        private string fileNameForAvi;
        private List<CodecInfo> _codecInfos;
        private int _framePerSecond;
        private string _folder;
        private int _quality;

        public int Quality
        {
            get => _quality;
            set => this.RaiseAndSetIfChanged(ref _quality, value);
        }
        public string Folder
        {
            get => _folder;
            set => this.RaiseAndSetIfChanged(ref _folder, value);
        }
        public ObservableCollection<string> ObsCollFilePath
        {
            get => _ObsCollFilePath;
            set => this.RaiseAndSetIfChanged(ref _ObsCollFilePath, value);
        }

        public int FramePerSecond
        {
            get => _framePerSecond;
            set => this.RaiseAndSetIfChanged(ref _framePerSecond, value);
        }

        public ReactiveCommand<ListBox, Unit> OpenFileCommand;
        public ReactiveCommand<Button, Unit> ScreenRecordCommand;
        public ReactiveCommand<Button, Unit> StopScreenRecordCommand;
        public ReactiveCommand<Unit, Unit> SaveCommand;
        public ReactiveCommand<Unit, Unit> ImageToAviCommand;

        public ViewModel()
        {
            Recorder recorder = new Recorder();
            FramePerSecond = 30;
            Quality = 70;
            Folder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Example.avi";
            OpenFileCommand = ReactiveCommand.Create<ListBox>(param =>
            {
                ObsCollFilePath = new ObservableCollection<string>();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (string filename in openFileDialog.FileNames)
                    {
                        ObsCollFilePath.Add(filename);
                    }
                }
                param.ItemsSource = ObsCollFilePath;
            });

            ScreenRecordCommand = ReactiveCommand.Create<Button>(param =>
            {
                recorder = new Recorder(Folder, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, FramePerSecond, Quality);
                recorder.ScreenRecording();
                Panel.SetZIndex(param, 1);
            });


            StopScreenRecordCommand = ReactiveCommand.Create<Button>(param =>
            {
                recorder.Dispose();
                Panel.SetZIndex(param, 0);

            });

            SaveCommand = ReactiveCommand.Create<Unit>(param =>
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "avi files (*.avi)|*.avi";
                saveFile.RestoreDirectory = false;
                saveFile.DefaultExt = ".avi";
                if (saveFile.ShowDialog() == true)
                {
                    fileNameForAvi = Path.GetFullPath(saveFile.FileName);
                    Folder = fileNameForAvi;
                }
            });

            ImageToAviCommand = ReactiveCommand.Create<Unit>(param =>
            {
                recorder = new Recorder(Folder);
                if (ObsCollFilePath.Count != 0)
                {
                    recorder.ImageToAvi(ObsCollFilePath);
                    MessageBox.Show("Готово!");
                }
                else
                {
                    MessageBox.Show("Список кадров пуст!");
                }
            });
        }
    }
}
