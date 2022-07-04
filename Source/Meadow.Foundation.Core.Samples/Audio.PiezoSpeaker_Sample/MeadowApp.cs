﻿using System;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Audio;
using Meadow.Units;

namespace Audio.PiezoSpeaker_Sample
{
    public class MeadowApp : App<F7FeatherV2>
    {
        //<!=SNIP=>

        protected PiezoSpeaker piezoSpeaker;

        public MeadowApp()
        {
            Console.WriteLine("Initializing...");

            piezoSpeaker = new PiezoSpeaker(Device.CreatePwmPort(Device.Pins.D05, new Frequency(100, Frequency.UnitType.Hertz)));

            _ = PlayTriad();
        }

        async Task PlayTriad()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Playing A major triad starting at A4");
                await piezoSpeaker.PlayTone(new Frequency(440, Frequency.UnitType.Hertz), 500); //A
                await piezoSpeaker.PlayTone(new Frequency(554.37f, Frequency.UnitType.Hertz), 500); //C#
                await piezoSpeaker.PlayTone(new Frequency(659.25f, Frequency.UnitType.Hertz), 500); //E

                await Task.Delay(2500);
            }
        }

        //<!=SNOP=>
    }
}