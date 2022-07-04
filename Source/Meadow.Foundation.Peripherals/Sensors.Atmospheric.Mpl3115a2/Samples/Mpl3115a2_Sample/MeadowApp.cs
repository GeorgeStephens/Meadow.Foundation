﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Atmospheric;
using System;
using System.Threading.Tasks;

namespace Sensors.Atmospheric.Mpl3115A2_Sample
{
    public class MeadowApp : App<F7FeatherV2>
    {
        //<!=SNIP=>

        readonly Mpl3115a2 sensor;

        public MeadowApp()
        {
            Console.WriteLine("Initializing...");

            sensor = new Mpl3115a2(Device.CreateI2cBus());

            var consumer = Mpl3115a2.CreateObserver(
                handler: result => 
                {
                    Console.WriteLine($"Observer: Temp changed by threshold; new temp: {result.New.Temperature?.Celsius:N2}C, old: {result.Old?.Temperature?.Celsius:N2}C");
                },
                filter: result =>
                {
                    //c# 8 pattern match syntax. checks for !null and assigns var.
                    if (result.Old?.Temperature is { } oldTemp &&
                        result.New.Temperature is { } newTemp)
                    {
                        return (newTemp - oldTemp).Abs().Celsius > 0.5; // returns true if > 0.5°C change.
                    }
                    return false;
                }
            );
            sensor.Subscribe(consumer);

            sensor.Updated += (sender, result) => {
                Console.WriteLine($"  Temperature: {result.New.Temperature?.Celsius:N2}C");
                Console.WriteLine($"  Pressure: {result.New.Pressure?.Bar:N2}bar");
            };

            ReadConditions().Wait();

            sensor.StartUpdating(TimeSpan.FromSeconds(1));
        }

        async Task ReadConditions()
        {
            var conditions = await sensor.Read();
            Console.WriteLine($"Temperature: {conditions.Temperature?.Celsius}°C, Pressure: {conditions.Pressure?.Pascal}Pa");
        }

        //<!=SNOP=>
    }
}