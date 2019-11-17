using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GNB.Domain.Helpers.Interfaces
{
    public interface IRequestHelper
    {
        Task<List<T>> GetListFromUrl<T>(string url);
    }
}
