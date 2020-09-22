using System.Collections.Generic;
using System.Threading.Tasks;
using KubeClient;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Kubernetes;
using Ocelot.ServiceDiscovery;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;

namespace APIGateway.Extensions
{
    public static class OcelotBuilderExtensions
    {
        private static readonly ServiceDiscoveryFinderDelegate FixedKubernetesProviderFactoryGet = (provider, config, reroute) =>
        {
            var serviceDiscoveryProvider = KubernetesProviderFactory.Get(provider, config, reroute);

            if (serviceDiscoveryProvider is KubernetesServiceDiscoveryProvider)
            {
                serviceDiscoveryProvider = new Kube(serviceDiscoveryProvider);
            }
            else if (serviceDiscoveryProvider is PollKubernetes)
            {
                serviceDiscoveryProvider = new PollKube(serviceDiscoveryProvider);
            }

            return serviceDiscoveryProvider;
        };

        public static IOcelotBuilder AddKubernetesFixed(this IOcelotBuilder builder, bool usePodServiceAccount = true)
        {
            builder.Services.AddSingleton(FixedKubernetesProviderFactoryGet);
            builder.Services.AddKubeClient(usePodServiceAccount);

            return builder;
        }

        private class Kube : IServiceDiscoveryProvider
        {
            private readonly IServiceDiscoveryProvider _serviceDiscoveryProvider;

            public Kube(IServiceDiscoveryProvider serviceDiscoveryProvider)
            {
                this._serviceDiscoveryProvider = serviceDiscoveryProvider;
            }

            public Task<List<Service>> Get()
            {
                return this._serviceDiscoveryProvider.Get();
            }
        }

        private class PollKube : IServiceDiscoveryProvider
        {
            private readonly IServiceDiscoveryProvider _serviceDiscoveryProvider;

            public PollKube(IServiceDiscoveryProvider serviceDiscoveryProvider)
            {
                this._serviceDiscoveryProvider = serviceDiscoveryProvider;
            }

            public Task<List<Service>> Get()
            {
                return this._serviceDiscoveryProvider.Get();
            }
        }
    }
}
