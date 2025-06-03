using System.Collections.Generic;
using System.Linq;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Card
{
    public class PoolingCardDataPipelineDecorator : IDataPipeline<IRegister<CardData>, CardData>
    {
        private readonly IDataPipeline<IRegister<CardData>, CardData> decoratee;
        private readonly VanillaCardPoolDelegator delegator;

        public PoolingCardDataPipelineDecorator(
            IDataPipeline<IRegister<CardData>, CardData> decoratee,
            VanillaCardPoolDelegator delegator
        )
        {
            this.decoratee = decoratee;
            this.delegator = delegator;
        }

        public List<IDefinition<CardData>> Run(IRegister<CardData> service)
        {
            var definitions = decoratee.Run(service);
            foreach (var definition in definitions)
            {
                var data = definition.Data;
                // TODO remove this class and add to the CardDatafinalizer and directly add to the CardPool
                var pools = definition
                    .Configuration.GetSection("pools")
                    .GetChildren()
                    .Where(xs => xs.Value != null)
                    .Select(xs => xs.Value!)
                    .ToList()!;
                foreach (var pool in pools)
                {
                    if (delegator.CardPoolToData.ContainsKey(pool))
                    {
                        delegator.CardPoolToData[pool].Add(data);
                    }
                    else
                    {
                        delegator.CardPoolToData.Add(pool, [data]);
                    }
                }
            }
            return definitions;
        }
    }
}
