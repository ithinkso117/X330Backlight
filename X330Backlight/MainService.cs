﻿using System;
using System.Windows.Interop;
using X330Backlight.Services;
using X330Backlight.Services.Interfaces;
using X330Backlight.Utils;

namespace X330Backlight
{
    internal class MainService : IServiceOwner
    {
        /// <summary>
        /// Gets the HwndSource of the owner.
        /// </summary>
        public HwndSource Source { get; }


        public MainService(HwndSource source)
        {
            Source = source;

            var backlightService = new BacklightService();

            var powerService = new PowerService();
            powerService.LidSwitchStatusChanged += OnLidSwitchStatusChanged;
            powerService.PowerStatusChanged += OnPowerChangeStatusChanged;

            var hotkeyservice = new HotkeyService();
            hotkeyservice.HotKeyTriggered += OnHotkeyTriggered;

            var idleService = new IdleService();
            idleService.IdleStateChanged += OnIdleStateChanged;

            ServiceManager.RegisterService<IBacklightService>(backlightService);
            ServiceManager.RegisterService<IPowerService>(powerService);
            ServiceManager.RegisterService<IHotkeyService>(hotkeyservice);
            ServiceManager.RegisterService<IIdleService>(idleService);


            foreach (var registeredService in ServiceManager.RegisteredServices)
            {
                registeredService.Owner = this;
                registeredService.Start();
                Logger.Write($"{registeredService.GetType().Name} started.");
            }

            backlightService.Mode = powerService.AcPowerPluggedIn ? BacklightMode.Ac : BacklightMode.Battery;
        }

        private void OnIdleStateChanged(object sender, EventArgs e)
        {
            var idleService = (IIdleService) sender;
            var backlightService = ServiceManager.GetService<IBacklightService>();
            if (idleService.IsIdle)
            {
                backlightService.EnterSavingMode();
            }
            else
            {
                backlightService.ExitSavingMode();
            }
        }

        /// <summary>
        /// When lid switch is opend, just trun on the monitor otherwise turn off the minitor.
        /// </summary>
        /// <param name="sender">Shoud be the IBacklightService</param>
        /// <param name="e">The lid switch's status</param>
        private void OnLidSwitchStatusChanged(object sender, LidSwitchStatusChangedEventArgs e)
        {
            var backlightService = ServiceManager.GetService<IBacklightService>();
            switch (e.Status)
            {
                case LidSwitchStatus.Opened:
                    backlightService.TurnOnBacklight();
                    break;
                case LidSwitchStatus.Closed:
                    backlightService.TurnOffBacklight();
                    break;
            }
        }

        /// <summary>
        /// If system is about the suspend, just stop all services, otherwise, start all services.
        /// </summary>
        /// <param name="sender">Should be the power service.</param>
        /// <param name="e">The power status.</param>
        private void OnPowerChangeStatusChanged(object sender, PowerChangeStatusEventArgs e)
        {
            switch (e.Status)
            {
                case PowerChangeStatus.Resuming:
                    Logger.Write("Resuming...");
                    foreach (var registeredService in ServiceManager.RegisteredServices)
                    {
                        registeredService.Start();
                        Logger.Write($"{registeredService.GetType().Name} started.");
                    }
                    break;
                case PowerChangeStatus.Suspending:
                    Logger.Write("Suspending...");
                    foreach (var registeredService in ServiceManager.RegisteredServices)
                    {
                        registeredService.Stop();
                        Logger.Write($"{registeredService.GetType().Name} stopped.");
                    }
                    break;
                case PowerChangeStatus.StatusChanged:
                {
                    var powerService = (IPowerService) sender;
                    var backlightService = ServiceManager.GetService<IBacklightService>();
                    backlightService.Mode = powerService.AcPowerPluggedIn? BacklightMode.Ac:BacklightMode.Battery;
                    break;
                }
            }
        }
      


        /// <summary>
        /// When ThinkPad hotkey is pressed, handle it.
        /// </summary>
        /// <param name="sender">Should be the HotkeyService.</param>
        /// <param name="e">The hotkey pressed.</param>
        private void OnHotkeyTriggered(object sender, HotkeyEventArgs e)
        {
            var backlightService = ServiceManager.GetService<IBacklightService>();
            switch (e.Key)
            {
                case TpHotkey.BrightnessIncrease:
                    backlightService.Brightness++;
                    //We should save the brightness here, when user adjust the brightness.
                    backlightService.SaveBrightness();
                    break;
                case TpHotkey.BrightnessDecrease:
                    backlightService.Brightness--;
                    //We should save the brightness here, when user adjust the brightness.
                    backlightService.SaveBrightness();
                    break;
                case TpHotkey.ThinkVantage:
                    if (SettingManager.TurnOffMonitorByThinkVantage)
                    {
                        if (backlightService.BacklightClosed)
                        {
                            backlightService.TurnOnBacklight();
                        }
                        else
                        {
                            backlightService.TurnOffBacklight();
                        }
                    }
                    break;
            }
        }


        /// <summary>
        /// Stop and close all functions, this should be called when application terminated.
        /// </summary>
        public void Close()
        {
            foreach (var registeredService in ServiceManager.RegisteredServices)
            {
                registeredService.Stop();
                registeredService.Close();
            }

            var idleService = ServiceManager.GetService<IIdleService>();
            idleService.IdleStateChanged -= OnIdleStateChanged;

            var hotkeyservice = ServiceManager.GetService<IHotkeyService>();
            hotkeyservice.HotKeyTriggered -= OnHotkeyTriggered;

            var powerService = ServiceManager.GetService<IPowerService>();
            powerService.LidSwitchStatusChanged -= OnLidSwitchStatusChanged;
            powerService.PowerStatusChanged -= OnPowerChangeStatusChanged;

            ServiceManager.Clear();
        }
    }
}
