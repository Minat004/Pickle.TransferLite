using System.Collections.Generic;
using System.IO;
using Pickle.TransferLite.ViewModels;

namespace Pickle.TransferLite.Helper;

internal static class Generator
{
    internal static void DirectoriesAndFiles(string? directoryPath, ICollection<FileEntityViewModel> directoriesAndFiles)
    {
        directoriesAndFiles.Clear();

        foreach (var directory in new DirectoryInfo(directoryPath!).GetDirectories())
            directoriesAndFiles.Add(new DirectoryViewModel(directory));

        foreach (var file in new DirectoryInfo(directoryPath!).GetFiles())
            directoriesAndFiles.Add(new FileViewModel(file));
    }
    
    internal static void DirectoriesAndFiles(DirectoryPanelViewModel panelViewModel)
    {
        panelViewModel.DirectoriesAndFiles.Clear();

        foreach (var directory in new DirectoryInfo(panelViewModel.DirectoryPath!).GetDirectories())
            panelViewModel.DirectoriesAndFiles.Add(new DirectoryViewModel(directory));

        foreach (var file in new DirectoryInfo(panelViewModel.DirectoryPath!).GetFiles())
            panelViewModel.DirectoriesAndFiles.Add(new FileViewModel(file));
        
        panelViewModel.SelectFirst();
    }

    internal static void LocalDrives(DirectoryPanelViewModel panelViewModel)
    {
        panelViewModel.DirectoriesAndFiles.Clear();
        panelViewModel.DirectoryPath = string.Empty;

        foreach (var logicalDrive in Directory.GetLogicalDrives())
            panelViewModel.DirectoriesAndFiles.Add(new DirectoryViewModel(logicalDrive));
        
        panelViewModel.SelectFirst();
    }
}