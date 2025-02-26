using System.Collections.Generic;

namespace TrainworksReloaded.Base
{
    public class ProviderDetails
    {
        public bool IsInitialized { get; set; }
        public IProvider Provider { get; set; }

        public ProviderDetails(bool isInit, IProvider provider)
        {
            this.IsInitialized = isInit;
            this.Provider = provider;
        }
    }

    public class GameDataClient : Dictionary<string, ProviderDetails>, IClient
    {
        public void NewProviderAvailable(IProvider newProvider)
        {
            this.Add(newProvider.GetType().Name, new ProviderDetails(false, newProvider));
        }

        public void NewProviderFullyInstalled(IProvider newProvider)
        {
            if (this.ContainsKey(newProvider.GetType().Name))
            {
                this[newProvider.GetType().Name].IsInitialized = true;
            }
        }

        public void ProviderRemoved(IProvider removeProvider)
        {
            this.Remove(removeProvider.GetType().Name);
        }
    }
}
