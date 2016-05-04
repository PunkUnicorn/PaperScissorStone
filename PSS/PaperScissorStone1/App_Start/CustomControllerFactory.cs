using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PaperScissorStone1
{
    public interface IControllerMetaData
    {
        string ControllerName { get; }
    }

    public class MefControllerFactory : DefaultControllerFactory
    {
        // (Taken from http://christianjvella.com/wordpress/mef-mvc-defining-controllerfactory/)

        private readonly CompositionContainer _container; // This container will work like an IOC container
        public MefControllerFactory(CompositionContainer container)
        {
            _container = container;
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            var controllerType = base.GetControllerType(requestContext, controllerName);

            if (controllerType == null)
            {
                var controller = _container.GetExports<IController, IControllerMetaData>().SingleOrDefault(x => x.Metadata.ControllerName == controllerName).Value;

                if (controller != null)
                {
                    return controller.GetType();
                }
            }
            return controllerType;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            //here if the controller object is not found (resulted as null) we can go ahead and get the default controller.
            //This means that in order to get the Controller we have to Export it first which will be shown later in this post
            if (controllerType == null)
            {
                // http://stackoverflow.com/questions/13836673/mvc4-mef-plugins-and-controllers-namespaces
                return base.GetControllerInstance(requestContext, controllerType);
            }
            else
            {
                Lazy<object, object> export = _container.GetExports(controllerType, null, null).FirstOrDefault();
                return (null == export) ? base.GetControllerInstance(requestContext, controllerType) : (IController)export.Value;
            }
        }
        public override void ReleaseController(IController controller)
        {
            //this is were the controller gets disposed
            ((IDisposable)controller).Dispose();
        }
    }
}