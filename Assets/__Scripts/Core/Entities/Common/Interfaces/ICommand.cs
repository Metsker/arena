using System.Threading.Tasks;

namespace Tower.Core.Entities.Common.Interfaces
{
    public interface ICommand
    {
        Task Execute();
    }
}
