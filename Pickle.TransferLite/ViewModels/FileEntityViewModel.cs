using System.IO;

namespace Pickle.TransferLite.ViewModels;

public abstract class FileEntityViewModel : ViewModelBase
{
    public string? Name { get; set; }
    public string? FullName { get; set; }
    public decimal Size { get; set; } 
    protected FileEntityViewModel(string? fullName)
    {
        FullName = fullName;
        Name = new DirectoryInfo(fullName!).Name;
    }
}