using System.IO;

namespace Pickle.TransferLite.ViewModels;

public sealed class DirectoryViewModel : FileEntityViewModel
{
    public DirectoryViewModel(string? fullName) : base(fullName)
    {
    }

    public DirectoryViewModel(DirectoryInfo directoryInfo) : base(directoryInfo.FullName)
    {
    }
}