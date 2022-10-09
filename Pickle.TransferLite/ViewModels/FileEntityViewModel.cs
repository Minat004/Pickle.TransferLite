using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Pickle.TransferLite.ViewModels;

public abstract class FileEntityViewModel : ViewModelBase
{
    public string? Name { get; set; }
    public string? FullName { get; set; }
    public DrawingImage? ImageIcon { get; set; }
    
    protected FileEntityViewModel(string? name)
    {
        Name = name;
    }
}