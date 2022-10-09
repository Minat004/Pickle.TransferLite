using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Media;
using Pickle.TransferLite.Helper;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Pickle.TransferLite.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        CopyCommand = ReactiveCommand.Create(Copy);
        MoveCommand = ReactiveCommand.Create(Move);
        DeleteCommand = ReactiveCommand.Create(Delete);
        RenameCommand = ReactiveCommand.Create(Rename);
        BackCommand = ReactiveCommand.Create(Back);

        RightPanel.Selection.SelectionChanged += RightPanelOnSelectionChanged;
        LeftPanel.Selection.SelectionChanged += LeftPanelOnSelectionChanged;
    }

    public DirectoryPanelViewModel LeftPanel { get; set; } = new();

    public DirectoryPanelViewModel RightPanel { get; set; } = new();

    [Reactive] public ReactiveCommand<Unit, Unit> CopyCommand { get; set; }

    [Reactive] public ReactiveCommand<Unit, Unit> MoveCommand { get; set; }

    [Reactive] public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }

    [Reactive] public ReactiveCommand<Unit, Unit> BackCommand { get; set; }

    [Reactive] public ReactiveCommand<Unit, Unit> RenameCommand { get; set; }

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

    private void Rename()
    {

    }

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
                        Generator.DirectoriesAndFiles(panelViewModel);
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
                        Generator.DirectoriesAndFiles(panelViewModel);
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
}