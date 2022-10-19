using System.IO;

namespace Pickle.TransferLite.ViewModels;

public sealed class FileViewModel : FileEntityViewModel
{
    public FileViewModel(string? fullName) : base(fullName)
    {
    }

    public FileViewModel(FileInfo fileInfo) : base(fileInfo.FullName)
    {
        Size = fileInfo.Length;
    }
}