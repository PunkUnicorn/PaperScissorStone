using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;
using SignalR = Microsoft.AspNet.SignalR;

namespace PaperScissorStone1
{
    /// <summary>
    /// Resolve dependencies for MVC / Web API using MEF.
    /// source: http://stackoverflow.com/questions/13566688/how-to-integrate-mef-with-asp-net-mvc-4-and-asp-net-web-api
    /// and: http://www.asp.net/signalr/overview/advanced/dependency-injection
    /// </summary>
    public class MefToSignalRDependencyResolver : SignalR.DefaultDependencyResolver, SignalR.IDependencyResolver
    {
        private readonly CompositionContainer _container;

        public MefToSignalRDependencyResolver(CompositionContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Called to request a service implementation.
        /// 
        /// Here we call upon MEF to instantiate implementations of dependencies.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Service implementation or null.</returns>
        public override object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            var name = AttributedModelServices.GetContractName(serviceType);
            var export = _container.GetExportedValueOrDefault<object>(name);
            if (export == null)
                return base.GetService(serviceType);

            return export;
        }

        /// <summary>
        /// Called to request service implementations.
        /// 
        /// Here we call upon MEF to instantiate implementations of dependencies.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Service implementations.</returns>
        public override IEnumerable<object> GetServices(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            var exports = _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
            if (exports == null)
                return base.GetServices(serviceType);

            return exports;
        }
    }
}