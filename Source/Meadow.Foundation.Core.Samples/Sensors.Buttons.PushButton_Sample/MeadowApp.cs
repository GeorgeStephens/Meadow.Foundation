﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Sensors.Buttons.PushButton_Sample
{
    public class MeadowApp : App<F7FeatherV2>
    {
        //<!=SNIP=>

        RgbPwmLed led;
        List<PushButton> pushButtons;

        public MeadowApp()
        {
            Console.WriteLine("Initializing...");

            //TestAllResistorTypes();
            TestMultiplePorts();

            Console.WriteLine("PushButton(s) ready!!!");
        }

        void TestAllResistorTypes()
        {
            led = new RgbPwmLed(
                Device,
                Device.Pins.D12,
                Device.Pins.D11,
                Device.Pins.D10);
            led.SetColor(Color.Red);

            pushButtons = new List<PushButton>();

            var inputInternalPullUp = Device.CreateDigitalInputPort(
                Device.Pins.MISO,
                InterruptMode.EdgeBoth,
                ResistorMode.InternalPullUp, 20);
            var buttonInternalPullUp = new PushButton(inputInternalPullUp);

            pushButtons.Add(buttonInternalPullUp);

            var inputInternalPullDown = Device.CreateDigitalInputPort(
                pin: Device.Pins.D02,
                InterruptMode.EdgeBoth,
                resistorMode: ResistorMode.InternalPullDown, 20);
            var buttonInternalPullDown = new PushButton(inputInternalPullDown);

            pushButtons.Add(buttonInternalPullDown);

            var inputExternalPullUp = Device.CreateDigitalInputPort(
                pin: Device.Pins.D03,
                InterruptMode.EdgeBoth,
                resistorMode: ResistorMode.ExternalPullUp);
            var buttonExternalPullUp = new PushButton(inputExternalPullUp);

            pushButtons.Add(buttonExternalPullUp);

            var inputExternalPullDown = Device.CreateDigitalInputPort(
                pin: Device.Pins.D04,
                InterruptMode.EdgeBoth,
                resistorMode: ResistorMode.ExternalPullDown);
            var buttonExternalPullDown = new PushButton(inputExternalPullDown);

            pushButtons.Add(buttonExternalPullDown);

            foreach (var pushButton in pushButtons)
            {
                pushButton.LongClickedThreshold = new TimeSpan(0,0,1);

                pushButton.Clicked += PushButtonClicked;
                pushButton.PressStarted += PushButtonPressStarted;
                pushButton.PressEnded += PushButtonPressEnded;
                pushButton.LongClicked += PushButtonLongClicked;
            }

            led.SetColor(Color.Green);
        }

        void TestMultiplePorts() 
        {
            // Important note: You can only use on Push Button per Group Set (GSXX)
            pushButtons = new List<PushButton>
            {
                new PushButton(Device, Device.Pins.D06),         // <- GS00

                new PushButton(Device, Device.Pins.D09),         // <- GS01

                new PushButton(Device, Device.Pins.D14),         // <- GS03
 
                new PushButton(Device, Device.Pins.A00),         // <- GS04

                new PushButton(Device, Device.Pins.COPI),        // <- GS05

                new PushButton(Device, Device.Pins.D02),         // <- GS06

                new PushButton(Device, Device.Pins.D05),         // <- GS07

                new PushButton(Device, Device.Pins.D03),         // <- GS08

                new PushButton(Device, Device.Pins.D00),         // <- GS09
                
                new PushButton(Device, Device.Pins.SCK),         // <- GS10

                new PushButton(Device, Device.Pins.CIPO),        // <- GS11
                
                new PushButton(Device, Device.Pins.D01),         // <- GS13

                new PushButton(Device, Device.Pins.D12),         // <- GS14

                new PushButton(Device, Device.Pins.D13),         // <- GS15
            };

            foreach (var pushButton in pushButtons)
            {
                pushButton.Clicked += PushButtonClicked;
            }
        }

        void PushButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine($"PushButton Clicked!");
            led.SetColor(Color.Orange);
            Thread.Sleep(500);
            led.SetColor(Color.Green);
        }

        void PushButtonPressStarted(object sender, EventArgs e)
        {
            Console.WriteLine($"PushButton PressStarted!");
            led.SetColor(Color.Red);
        }

        void PushButtonPressEnded(object sender, EventArgs e)
        {
            Console.WriteLine($"PushButton PressEnded!");
            led.SetColor(Color.Green);
        }

        void PushButtonLongClicked(object sender, EventArgs e)
        {
            Console.WriteLine($"PushButton LongClicked!");
            led.SetColor(Color.Blue);
            Thread.Sleep(500);
            led.SetColor(Color.Green);
        }

        //<!=SNOP=>
    }
}