﻿using Meadow.Peripherals.Sensors.Buttons;
using System;

namespace Meadow.Foundation.Sensors.Hid
{
    internal class WiiExtensionButton : IButton
    {
        /// <summary>
        /// The minimum duration for a long press.
        /// </summary>
        public TimeSpan LongClickedThreshold { get; set; } = TimeSpan.Zero;

        public bool State { get; protected set; } = false;

        /// <summary>
        /// Raised when a press starts (the button is pushed down).
        /// </summary>
        public event EventHandler PressStarted;

        /// <summary>
        /// Raised when a press ends (the button is released).
        /// </summary>
        public event EventHandler PressEnded;

        /// <summary>
        /// Raised when the button circuit is re-opened after it has been closed (at the end of a press).
        /// </summary>
        public event EventHandler Clicked;
        
        /// <summary>
        /// Raised when the button circuit is pressed for LongPressDuration.
        /// </summary>
        public event EventHandler LongClicked;

        /// <summary>
        /// Maximum DateTime value when the button was just pushed
        /// </summary>
        protected DateTime buttonPressStart = DateTime.MaxValue;

        public void Update(bool state)
        {
            //Console.WriteLine($"{state} {State}");

            if (state == true && State == false)
            {   // save our press start time (for long press event)
                buttonPressStart = DateTime.Now;

                RaisePressStarted();
            }
            else if(state == false && State == true)
            {   // calculate the press duration
                TimeSpan pressDuration = DateTime.Now - buttonPressStart;

                // reset press start time
                buttonPressStart = DateTime.MaxValue;

                // if it's a long press, raise our long press event
                if (LongClickedThreshold > TimeSpan.Zero && pressDuration > LongClickedThreshold)
                {
                    RaiseLongClicked();
                }
                else
                {
                //    Console.WriteLine($"{state} {State}");

                    RaiseClicked();
                }

                if (pressDuration.TotalMilliseconds > 0)
                {   // raise the other events
                    RaisePressEnded();
                }
            }

            State = state;
        }

        /// <summary>
        /// Raised when the button circuit is re-opened after it has been closed (at the end of a �press�).
        /// </summary>
        protected virtual void RaiseClicked()
        {
            //Console.WriteLine("RaiseClicked");
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raised when a press starts (the button is pushed down; circuit is closed).
        /// </summary>
        protected virtual void RaisePressStarted()
        {
            //Console.WriteLine("RaisePressStarted");
            PressStarted?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Raised when a press ends (the button is released; circuit is opened).
        /// </summary>
        protected virtual void RaisePressEnded()
        {
            //Console.WriteLine("RaisePressEnded");
            PressEnded?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Raised when the button circuit is pressed for at least 500ms.
        /// </summary>
        protected virtual void RaiseLongClicked()
        {
            //Console.WriteLine("RaiseLongClicked");
            LongClicked?.Invoke(this, new EventArgs());
        }
    }
}