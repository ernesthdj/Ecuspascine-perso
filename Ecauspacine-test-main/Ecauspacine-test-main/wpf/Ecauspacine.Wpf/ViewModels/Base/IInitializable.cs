using System.Threading.Tasks;

namespace Ecauspacine.Wpf.ViewModels.Base;

public interface IInitializable
{
    Task InitializeAsync();
    void Reset();
}
