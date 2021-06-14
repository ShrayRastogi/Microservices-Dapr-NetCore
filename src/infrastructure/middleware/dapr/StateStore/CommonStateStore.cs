using Dapr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateStore
{
    public class CommonStateStore : ICommonStateStore
    {
        private const string StoreName = "statestore";
        private readonly DaprClient _dapr;

        public CommonStateStore(DaprClient dapr)
        {
            _dapr = dapr;
        }

        public async Task DeleteStateAsync(string id)
        {
            await _dapr.DeleteStateAsync(StoreName, id);
        }

        public async Task<TCommonState> GetStateAsync<TCommonState>(string id) where TCommonState: CommonState
        {
            try
            {
                return await _dapr.GetStateAsync<TCommonState>(StoreName, id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task UpdateStateAsync<TCommonState>(TCommonState state, string id)
            where TCommonState : CommonState
        {
            await _dapr.SaveStateAsync(StoreName, id, (dynamic)state);
            //var prevState = await _dapr.GetStateEntryAsync<TCommonState>(StoreName, id);
            //// We need to make sure that we pass the concrete type to UpdateStateAsync,
            //// which can be accomplished by casting the state to dynamic. This ensures
            //// that all state fields are properly serialized.
            //if (prevState != null && prevState.Value != null)
            //{
            //    prevState.Value = state;
            //    await _dapr.SaveStateAsync(StoreName, id, (dynamic)prevState);
            //} else
            //{
            //    await _dapr.SaveStateAsync(StoreName, id, (dynamic)state);
            //}
        }
    }
}
