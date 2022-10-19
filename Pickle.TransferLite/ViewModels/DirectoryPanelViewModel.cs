using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls.Selection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pickle.TransferLite.Helper;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Pickle.TransferLite.ViewModels;

public class DirectoryPanelViewModel : ViewModelBase
{
    public DirectoryPanelViewModel()
    {
        Generator.LocalDrives(this);

        OpenDirectory = ReactiveCommand.Create(() => Open(SelectedFileEntity!));
        BackDirectory = ReactiveCommand.Create(Back);

        Selection.SelectionChanged += SelectionChanged;

        Selection.SingleSelect = false;
    }

    public ReactiveCommand<Unit, Unit> OpenDirectory { get; }

    [Reactive] public ObservableCollection<FileEntityViewModel> DirectoriesAndFiles { get; set; } = new();

    [Reactive] public string? DirectoryPath { get; set; }

    public FileEntityViewModel? SelectedFileEntity { get; set; }
    public ReactiveCommand<Unit, Unit> BackDirectory { get; }

    public Stack<string> DirectoryStack { get; } = new();

    public SelectionModel<FileEntityViewModel> Selection { get; set; } = new();

    #region Methods

    public void SelectFirst() => Selection.Select(0);

    public bool IsSelected
    {
        get
        {
            for (var i = 0; i < DirectoriesAndFiles.Count; i++)
            {
                if (Selection.IsSelected(i))
                {
                    return true;
                }
            }
            return false;
        }
    }

    private void SelectionChanged(object? sender, SelectionModelSelectionChangedEventArgs<FileEntityViewModel> e)
    {
    }

    private void Open(FileEntityViewModel? fileEntity)
    {
        if (fileEntity is not DirectoryViewModel directoryViewModel) return;
        
        DirectoryPath = directoryViewModel.FullName;

        DirectoryStack.Push(DirectoryPath!);

        Generator.DirectoriesAndFiles(this);
    }

    private void Back()
    {
        if (DirectoryStack.Count is 0 or 1)
        {
            Generator.LocalDrives(this);
            DirectoryStack.Clear();
            return;
        }

        DirectoryStack.Pop();
        DirectoryPath = DirectoryStack.Peek();

        Generator.DirectoriesAndFiles(this);
    }
    #endregion
}