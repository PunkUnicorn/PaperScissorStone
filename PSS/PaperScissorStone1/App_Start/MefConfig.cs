using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PaperScissorStone1
{
    public class MefConfig
    {
        
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

            var composition = new CompositionContainer(both);
            IControllerFactory mefControllerFactory = new MefControllerFactory(composition); //Get Factory to be used
            ControllerBuilder.Current.SetControllerFactory(mefControllerFactory); //Set Factory to be used

        }
    }
}