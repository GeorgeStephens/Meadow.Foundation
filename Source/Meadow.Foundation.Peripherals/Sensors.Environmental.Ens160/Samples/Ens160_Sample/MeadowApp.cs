﻿using System;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Environmental;

namespace Sensors.Environmental.Scd40_Sample
{
    public class MeadowApp : App<F7FeatherV2>
    {
        //<!=SNIP=>

        Ens160 sensor;

        public override Task Initialize()
        {
            Console.WriteLine("Initializing...");

            var i2cBus = Device.CreateI2cBus(Meadow.Hardware.I2cBusSpeed.Standard);
      
            sensor = new Ens160(i2cBus, (byte)Ens160.Addresses.Address_0x53);

            /*
            var consumer = Scd4x.CreateObserver(
                handler: result =>
                {
                    Console.WriteLine($"Observer: Temp changed by threshold; new temp: {result.New.Temperature?.Celsius:N2}C, old: {result.Old?.Temperature?.Celsius:N2}C");
                },
                filter: result =>
                {
                    if (result.Old?.Temperature is { } oldTemp &&
                        result.Old?.Humidity is { } oldHumidity &&
                        result.New.Temperature is { } newTemp &&
                        result.New.Humidity is { } newHumidity)
                    {
                        return ((newTemp - oldTemp).Abs().Celsius > 0.5 &&
                                (newHumidity - oldHumidity).Percent > 0.05);
                    }
                    return false;
                }
            );

            sensor?.Subscribe(consumer);

            if (sensor != null)
            {
                sensor.Updated += (sender, result) =>
                {
                    Console.WriteLine($"  Concentration: {result.New.Concentration?.PartsPerMillion:N0}ppm");
                    Console.WriteLine($"  Temperature: {result.New.Temperature?.Celsius:N1}C");
                    Console.WriteLine($"  Relative Humidity: {result.New.Humidity:N0}%");
                };
            }

            sensor?.StartUpdating(TimeSpan.FromSeconds(6));
            */

            return base.Initialize();
        }

        //<!=SNOP=>
    }
}