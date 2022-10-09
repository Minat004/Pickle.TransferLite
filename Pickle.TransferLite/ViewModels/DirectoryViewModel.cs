using System.IO;
using Avalonia;
using Avalonia.Media;

namespace Pickle.TransferLite.ViewModels;

public sealed class DirectoryViewModel : FileEntityViewModel
{
    public DirectoryViewModel(string? directoryName) : base(directoryName)
    {
        FullName = directoryName;
    }

    public DirectoryViewModel(DirectoryInfo directoryInfo) : base(directoryInfo.Name)
    {
        FullName = directoryInfo.FullName;
    }
}