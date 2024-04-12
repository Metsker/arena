using System.Threading.Tasks;

namespace Arena.__Scripts.Core.Entities.Common.Interfaces
{
    public interface ICommand
    {
        Task Execute();
    }
}
