using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls.Selection;
using Microsoft.CodeAnalysis.CSharp;
using Pickle.TransferLite.Helper;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Pickle.TransferLite.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        // CopyCommand = ReactiveCommand.Create(Copy);
        CopyCommand = ReactiveCommand.CreateFromTask(CopyAsync);
        MoveCommand = ReactiveCommand.Create(Move);
        DeleteCommand = ReactiveCommand.Create(Delete);
        RenameCommand = ReactiveCommand.Create(Rename);
        BackCommand = ReactiveCommand.Create(Back);
        ProgressBarCommand = ReactiveCommand.Create(ShowDownload);

        RightPanel.Selection.SelectionChanged += RightPanelOnSelectionChanged;
        LeftPanel.Selection.SelectionChanged += LeftPanelOnSelectionChanged;
    }

    private async void ShowDownload()
    {
        for (var i = 1; i <= 100; i++)
        {
            CircleValue = i;
            await Task.Delay(100);
        }
    }

    public DirectoryPanelViewModel LeftPanel { get; set; } = new();

    public DirectoryPanelViewModel RightPanel { get; set; } = new();

    [Reactive] public decimal Size { get; set; }

    [Reactive] public double CircleValue { get; set; } = 1D;
    
    [Reactive] public double MinValue { get; set; } = 0D;
    
    [Reactive] public double MaxValue { get; set; } = 100D;

    [Reactive] public ReactiveCommand<Unit, Unit> CopyCommand { get; set; }

    [Reactive] public ReactiveCommand<Unit, Unit> MoveCommand { get; set; }

    [Reactive] public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }

    [Reactive] public ReactiveCommand<Unit, Unit> BackCommand { get; set; }

    [Reactive] public ReactiveCommand<Unit, Unit> RenameCommand { get; set; }
    
    [Reactive] public ReactiveCommand<Unit, Unit> ProgressBarCommand { get; set; }

    private void LeftPanelOnSelectionChanged(object? sender, SelectionModelSelectionChangedEventArgs<FileEntityViewModel> e)
    {
        if (LeftPanel.IsSelected)
        {
            RightPanel.Selection.DeselectRange(0, RightPanel.DirectoriesAndFiles.Count);
        }
    }

    private void RightPanelOnSelectionChanged(object? sender, SelectionModelSelectionChangedEventArgs<FileEntityViewModel> e)
    {
        if (RightPanel.IsSelected)
        {
            LeftPanel.Selection.DeselectRange(0, LeftPanel.DirectoriesAndFiles.Count);
        }
    }

    private void Back()
    {
        if (LeftPanel.IsSelected)
        {
            BackDirectory(LeftPanel);
        } else if (RightPanel.IsSelected)
        {
            BackDirectory(RightPanel);
        }
    }

    private void Copy()
    {
        if (LeftPanel.IsSelected)
        {
            CopyFileEntity(LeftPanel, RightPanel);
        } else if (RightPanel.IsSelected)
        {
            CopyFileEntity(RightPanel, LeftPanel);
        }
    }
    
    private async Task CopyAsync()
    {
        if (LeftPanel.IsSelected)
        {
            await CopyFileEntityAsync(LeftPanel, RightPanel);
        } else if (RightPanel.IsSelected)
        {
            await CopyFileEntityAsync(RightPanel, LeftPanel);
        }
    }

    private void Move()
    {
        if (LeftPanel.IsSelected)
        {
            MoveFileEntity(LeftPanel, RightPanel);
        } else if (RightPanel.IsSelected)
        {
            MoveFileEntity(RightPanel, LeftPanel);
        }
    }

    private void Delete()
    {
        if (LeftPanel.IsSelected)
        {
            DeleteFileEntity(LeftPanel);
        } else if (RightPanel.IsSelected)
        {
            DeleteFileEntity(RightPanel);
        }
    }

    private async void Rename()
    {
        if (!LeftPanel.IsSelected || LeftPanel.Selection.SelectedItem is not FileViewModel) return;

        var fileSize = 0M;
        Size = 0M;
        foreach (var file in LeftPanel.Selection.SelectedItems)
        {
            Size += Math.Round(file.Size / 1024 / 1024, 2);
        }

        foreach (var file in LeftPanel.Selection.SelectedItems)
        {
            fileSize += Math.Round(file.Size / 1024 / 1024, 2);
            ProgressValue = (int)Math.Round(100 * fileSize / Size);
            await Task.Delay(1000);
        }
        
        // for (var i = 0; i <= Size; i++)
        // {
        //     ProgressValue = Math.Round(100 * i / Size);
        //     await Task.Delay(1000);
        // }
    }

    [Reactive] public int ProgressValue { get; set; }

    private static void BackDirectory(DirectoryPanelViewModel panelViewModel)
    {
        if (panelViewModel.DirectoryStack.Count is 0 or 1)
        {
            Generator.LocalDrives(panelViewModel);
            panelViewModel.DirectoryStack.Clear();
            return;
        }

        panelViewModel.DirectoryStack.Pop();
        panelViewModel.DirectoryPath = panelViewModel.DirectoryStack.Peek();

        Generator.DirectoriesAndFiles(panelViewModel);
    }

    private static void CopyFileEntity(DirectoryPanelViewModel sourcePanelViewModel, DirectoryPanelViewModel destPanelViewModel)
    {
        var selectedItems = new List<FileEntityViewModel>(sourcePanelViewModel.Selection.SelectedItems);
        foreach (var selectedItem in selectedItems)
        {
            switch (selectedItem)
            {
                case FileViewModel file:
                    try
                    {
                        File.Copy(file.FullName!, Path.Combine(destPanelViewModel.DirectoryPath!, file.Name!));
                        Generator.DirectoriesAndFiles(destPanelViewModel);
                    }
                    catch (IOException ioException)
                    {
                        Console.WriteLine(ioException);
                        throw;
                    }

                    break;
                case DirectoryViewModel directory:
                    try
                    {
                        CopyDirectory(directory.FullName!, Path.Combine(destPanelViewModel.DirectoryPath!, directory.Name!));
                        Generator.DirectoriesAndFiles(destPanelViewModel);
                    }
                    catch (IOException ioException)
                    {
                        Console.WriteLine(ioException);
                        throw;
                    }

                    break;
            }
        }
    }
    
    private static async Task CopyFileEntityAsync(DirectoryPanelViewModel sourcePanelViewModel, DirectoryPanelViewModel destPanelViewModel)
    {
        var selectedItems = new List<FileEntityViewModel>(sourcePanelViewModel.Selection.SelectedItems);
        foreach (var selectedItem in selectedItems)
        {
            switch (selectedItem)
            {
                case FileViewModel file:
                    try
                    {
                        await Task.Run(() =>
                            File.Copy(file.FullName!, Path.Combine(destPanelViewModel.DirectoryPath!, file.Name!)));
                        destPanelViewModel.DirectoriesAndFiles.Add(file);
                    }
                    catch (IOException ioException)
                    {
                        Console.WriteLine(ioException);
                    }

                    break;
                case DirectoryViewModel directory:
                    try
                    {
                        await Task.Run(() => CopyDirectoryAsync(directory.FullName!,
                            Path.Combine(destPanelViewModel.DirectoryPath!, directory.Name!), destPanelViewModel));
                    }
                    catch (IOException ioException)
                    {
                        Console.WriteLine(ioException);
                    }

                    break;
            }
        }
    }

    private static void MoveFileEntity(DirectoryPanelViewModel sourcePanelViewModel, DirectoryPanelViewModel destPanelViewModel)
    {
        var selectedItems = new List<FileEntityViewModel>(sourcePanelViewModel.Selection.SelectedItems);
        foreach (var selectedItem in selectedItems)
        {
            switch (selectedItem)
            {
                case FileViewModel file:
                    try
                    {
                        File.Move(file.FullName!, Path.Combine(destPanelViewModel.DirectoryPath!, file.Name!));
                        Generator.DirectoriesAndFiles(sourcePanelViewModel);
                        Generator.DirectoriesAndFiles(destPanelViewModel);
                    }
                    catch (IOException ioException)
                    {
                        Console.WriteLine(ioException);
                        throw;
                    }

                    break;
                case DirectoryViewModel directory:
                    try
                    {
                        Directory.Move(directory.FullName!, Path.Combine(destPanelViewModel.DirectoryPath!, directory.Name!));
                        Generator.DirectoriesAndFiles(sourcePanelViewModel);
                        Generator.DirectoriesAndFiles(destPanelViewModel);
                    }
                    catch (IOException ioException)
                    {
                        Console.WriteLine(ioException);
                        throw;
                    }

                    break;
            }
        }
    }
    
    private static void DeleteFileEntity(DirectoryPanelViewModel panelViewModel)
    {
        var selectedItems = new List<FileEntityViewModel>(panelViewModel.Selection.SelectedItems);
        foreach (var selectedItem in selectedItems)
        {
            switch (selectedItem)
            {
                case FileViewModel file:
                    try
                    {
                        File.Delete(file.FullName!);
                        // Generator.DirectoriesAndFiles(panelViewModel);
                        panelViewModel.DirectoriesAndFiles.Remove(file);
                    }
                    catch (IOException ioException)
                    {
                        Console.WriteLine(ioException);
                        throw;
                    }

                    break;
                case DirectoryViewModel directory:
                    try
                    {
                        Directory.Delete(directory.FullName!, true);
                        // Generator.DirectoriesAndFiles(panelViewModel);
                        panelViewModel.DirectoriesAndFiles.Remove(directory);
                    }
                    catch (IOException ioException)
                    {
                        Console.WriteLine(ioException);
                        throw;
                    }

                    break;
            }
        }
    }

    private static void CopyDirectory(string source, string dest)
    {
        Directory.CreateDirectory(dest);
        
        foreach (var file in Directory.GetFiles(source))
        {
            var fileName = Path.Combine(dest, Path.GetFileName(file));
            File.Copy(file, fileName);
        }

        foreach (var directory in Directory.GetDirectories(source))
        {
            var dirName = Path.GetFileName(directory);
            if (!Directory.Exists(Path.Combine(dest, dirName)))
            {
                Directory.CreateDirectory(Path.Combine(dest, dirName));
            }
            CopyDirectory(directory, Path.Combine(dest, dirName));
        }
    }
    
    private static async Task CopyDirectoryAsync(string source, string dest, DirectoryPanelViewModel destPanelViewModel)
    {
        if (!Directory.Exists(dest))
        {
            await Task.Run(() => Directory.CreateDirectory(dest));
            destPanelViewModel.DirectoriesAndFiles.Add(new DirectoryViewModel(dest));
        }
        
        foreach (var file in Directory.GetFiles(source))
        {
            var fileName = Path.Combine(dest, Path.GetFileName(file));
            await Task.Run(() => File.Copy(file, fileName));
        }

        foreach (var directory in Directory.GetDirectories(source))
        {
            var dirName = Path.GetFileName(directory);
            if (!Directory.Exists(Path.Combine(dest, dirName)))
            {
                await Task.Run(() => Directory.CreateDirectory(Path.Combine(dest, dirName)));
            }
            await CopyDirectoryAsync(directory, Path.Combine(dest, dirName), destPanelViewModel);
        }
    }

}