using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.Program.Interfaces
{
    public interface IDocumentSizeCalculator
    {
        Task<long> CalculateTotalSizeAsync();
    }
}
