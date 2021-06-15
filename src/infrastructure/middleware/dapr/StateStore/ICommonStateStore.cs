using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateStore
{
    //public interface ICommonStateStore<in TCommonState> : ICommonStateStore
    //     where TCommonState : CommonState
    //{
    //}

    public interface ICommonStateStore
    {
        Task<TCommonState> GetStateAsync<TCommonState>(string id)
            where TCommonState : CommonState;
        Task DeleteStateAsync(string id);
        Task UpdateStateAsync<TCommonState>(TCommonState @state, string id)
            where TCommonState : CommonState;
    }
}
