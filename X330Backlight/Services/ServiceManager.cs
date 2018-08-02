using System;
using System.Collections.Generic;
using System.Linq;
using X330Backlight.Services.Interfaces;

namespace X330Backlight.Services
{
    internal class ServiceManager
    {
        private static readonly Dictionary<Type, IService> Services = new Dictionary<Type, IService>();

        /// <summary>
        /// Gets all registered services.
        /// </summary>
        public static IService[] RegisteredServices => Services.Values.ToArray();

        /// <summary>
        /// Register one service into the ServiceManager, so that everyone can get the service from the ServiceManage.
        /// </summary>
        /// <param name="service">The service to register.</param>
        public static void RegisterService<T>(T service) where T:IService
        {
            var type = typeof(T);
            Services.Add(type, service);
        }


        /// <summary>
        /// UnRegister a service from ServiceManager.
        /// </summary>
        public static void UnRegisterService<T>() where T: IService
        {
            var type = typeof(T);
            if (Services.ContainsKey(type))
            {
                Services.Remove(type);
            }
        }

        /// <summary>
        /// Gets one service from the ServiceManager by given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T: IService
        {
            var type = typeof(T);
            if (!Services.ContainsKey(type))
            {
                throw new KeyNotFoundException(typeof(T).Name);
            }
            return (T)Services[type];
        }

        /// <summary>
        /// Clear all services from the ServiceManager.
        /// </summary>
        public static void Clear()
        {
            Services.Clear();
        }
    }
}
