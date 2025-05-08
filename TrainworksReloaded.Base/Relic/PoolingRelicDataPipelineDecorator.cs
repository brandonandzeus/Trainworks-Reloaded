using System.Collections.Generic;
using System.Linq;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Relic
{
    public class PoolingRelicDataPipelineDecorator : IDataPipeline<IRegister<RelicData>, RelicData>
    {
        private readonly IDataPipeline<IRegister<RelicData>, RelicData> decoratee;
        private readonly VanillaRelicPoolDelegator collectableDelegator;
        private readonly VanillaEnhancerPoolDelegator enhancerDelegator;

        public PoolingRelicDataPipelineDecorator(
            IDataPipeline<IRegister<RelicData>, RelicData> decoratee,
            VanillaRelicPoolDelegator collectableDelegator,
            VanillaEnhancerPoolDelegator enhancerDelegator
        )
        {
            this.decoratee = decoratee;
            this.collectableDelegator = collectableDelegator;
            this.enhancerDelegator = enhancerDelegator;
        }

        public List<IDefinition<RelicData>> Run(IRegister<RelicData> service)
        {
            var definitions = decoratee.Run(service);
            foreach (var definition in definitions)
            {
                var data = definition.Data;
                var pools = definition
                    .Configuration.GetSection("pools")
                    .GetChildren()
                    .Where(xs => xs.Value != null)
                    .Select(xs => xs.Value!)
                    .ToList()!;
                if (data is CollectableRelicData collectableRelicData)
                {
                    foreach (var pool in pools)
                    {
                        if (collectableDelegator.RelicPoolToData.ContainsKey(pool))
                        {
                            collectableDelegator.RelicPoolToData[pool].Add(collectableRelicData);
                        }
                        else
                        {
                            collectableDelegator.RelicPoolToData.Add(pool, [collectableRelicData]);
                        }
                    }
                }
                else if (data is EnhancerData enhancerData)
                {
                    foreach (var pool in pools)
                    {
                        if (enhancerDelegator.EnhancerPoolToData.ContainsKey(pool))
                        {
                            enhancerDelegator.EnhancerPoolToData[pool].Add(enhancerData);
                        }
                        else
                        {
                            enhancerDelegator.EnhancerPoolToData.Add(pool, [enhancerData]);
                        }
                    }
                }

            }
            return definitions;
        }
    }
}
