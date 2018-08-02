namespace X330Backlight.Services.Interfaces
{
    internal interface IService
    {
        /// <summary>
        /// Gets or sets the Owner service.
        /// </summary>
        IServiceOwner Owner { get; set; }

        /// <summary>
        /// Gets the timeout value when stop the service(unit:ms), default is 200ms.
        /// </summary>
        int StopTimeout { get; set; }

        /// <summary>
        /// Start the service.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the service.
        /// </summary>
        void Stop();

        /// <summary>
        /// Close the service, this should dispose all referenced resources.
        /// </summary>
        void Close();
    }
}
