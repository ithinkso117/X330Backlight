using X330Backlight.Services.Interfaces;

namespace X330Backlight.Services
{
    internal abstract class ServiceBase:IService
    {
        /// <summary>
        /// The owner of the service, it's a window in real world.
        /// </summary>
        public IServiceOwner Owner { get; set; }

        /// <summary>
        /// The stop timeout of the service, the default value is 200.
        /// </summary>
        public int StopTimeout { get; set; } = 200;

        /// <summary>
        /// Start the service.
        /// </summary>
        public abstract void Start();


        /// <summary>
        /// Stop the service.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Close the service, this should dispose all referenced resources.
        /// </summary>
        public virtual void Close()
        {
            Stop();
        }
    }
}
