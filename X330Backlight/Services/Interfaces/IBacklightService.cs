using System;

namespace X330Backlight.Services.Interfaces
{
    internal interface IBacklightService :IService
    {
        /// <summary>
        /// Raised when the brightness changed.
        /// </summary>
        event EventHandler BrightnessChanged;

        /// <summary>
        /// Gets if the Backlight is in closed status.
        /// </summary>
        bool BacklightClosed { get;}


        /// <summary>
        /// Gets or sets the brightness value.
        /// </summary>
        int Brightness { get; set; }


        /// <summary>
        /// Save current brightness to the file in the save folder.
        /// </summary>
        void SaveBrightness();

        /// <summary>
        /// Load the saved brightness from the saved file.
        /// </summary>
        byte LoadBrightness();


        /// <summary>
        /// Open the backlight
        /// </summary>
        void TurnOnBacklight();

        /// <summary>
        /// Save current brightness and close the backlight
        /// </summary>
        void TrunOffBacklight();


        /// <summary>
        /// Reduce the backlight 
        /// </summary>
        /// <param name="batteryMode">Ture when using battrty</param>
        void EnterSavingMode(bool batteryMode);

        /// <summary>
        /// Exit the saving mode, increase the backlight.
        /// </summary>
        void ExitSavingMode();
    }
}
