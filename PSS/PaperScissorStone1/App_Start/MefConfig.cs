using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PaperScissorStone1
{
    public class MefConfig
    {
        /// <summary>
        /// This is used for signalR composition
        /// </summary>
        internal static CompositionContainer Container { get; private set; }

        public static void Compose()
        {
            // Based on  // http://christianjvella.com/wordpress/mef-mvc-defining-controllerfactory/

            // The dependant assembly PaperScissorStoneCore is also project referenced to this executable so the core dll 
            // will be copied to a known location (the .\bin folder). This behaviour is convienient for the below assembly 
            // catalogging. The use of MEF componant detatchment is primaraly to facilitate easy unit tests 
            var selfCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            var targetPath = Path.Combine(HttpRuntime.AppDomainAppPath, @"bin\");
            var targetAssemblyFullName = Path.Combine(targetPath, "PaperScissorStoneCore.dll");
            var coreCatalog = new AssemblyCatalog(targetAssemblyFullName);

            var both = new AggregateCatalog(selfCatalog, coreCatalog);
            Container = new CompositionContainer(both);

            Container.ComposeParts();
            IControllerFactory mefControllerFactory = new MefControllerFactory(Container); 
            ControllerBuilder.Current.SetControllerFactory(mefControllerFactory); 
        }
    }
}