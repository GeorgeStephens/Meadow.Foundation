﻿using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Meadow.Foundation.Sensors.Power;

/// <summary>
/// Represents a INA2xx Series Precision Digital Current and Power Monitor
/// </summary>
public abstract partial class Ina2xx
    : ByteCommsSensorBase<(Units.Current? Current, Units.Voltage? Voltage, Units.Power? Power)>, II2cPeripheral
{
    /// <summary>
    /// The default I2C address for the peripheral
    /// </summary>
    public byte DefaultI2cAddress => (byte)Addresses.Default;

    /// <summary>
    /// Default I2C Bus Speed to use for communication.
    /// </summary>
    public static I2cBusSpeed DefaultBusSpeed => I2cBusSpeed.Fast;

    /// <summary> Value of the LSB of the voltage register. </summary>
    internal Units.Voltage _voltageScale;
    /// <summary> Value of the LSB of the current register. </summary>
    internal Units.Current _currentScale;
    /// <summary> Value of the LSB of the power register. </summary>
    internal Units.Power _powerScale;
    /// <summary> Value of the shunt resistor used for current measurements. </summary>
    internal Units.Resistance _shuntResistor;

    /// <summary>
    /// Create a new INA2xx object.
    /// </summary>
    /// <param name="i2cBus">The I2C bus</param>
    /// <param name="address">The I2C address</param>
    protected Ina2xx(II2cBus i2cBus, byte address = (byte)Addresses.Default)
        : base(i2cBus, address)
    { }

    /// <summary>
    /// Create a new INA2xx object, using the address pin connections to calculate the correct I2C address.
    /// </summary>
    /// <param name="i2cBus">The I2C bus</param>
    /// <param name="a0"><see cref="AddressConnection"/> specifying what A0 is connected to.</param>
    /// <param name="a1"><see cref="AddressConnection"/> specifying what A1 is connected to.</param>
    protected Ina2xx(II2cBus i2cBus, AddressConnection a0 = AddressConnection.GND, AddressConnection a1 = AddressConnection.GND)
        : this(i2cBus, GetAddress(a0, a1))
    { }

    /// <summary>
    /// Lookup the correct address to use for the INA2xx based on the address pin connections.
    /// </summary>
    /// <param name="a0"><see cref="AddressConnection"/> specifying what A0 is connected to.</param>
    /// <param name="a1"><see cref="AddressConnection"/> specifying what A1 is connected to.</param>
    /// <returns>correct <see cref="Addresses"/> value to use, as a <c>byte</c></returns>
    public static byte GetAddress(AddressConnection a0, AddressConnection a1) => (byte)((byte)Addresses.Address_0x40 | (byte)a0 | ((byte)a1 << 2));

    /// <summary>
    /// Sets the sensor Configuration to default values. Each implementation should provide overloads for specific available options.
    /// </summary>
    public abstract void Configure();

    /// <summary>
    /// Resets Ina2xx to default settings.
    /// </summary>
    public void Reset()
    {
        BusComms.WriteRegister(ConfigRegister, ResetIna2xx, ByteOrder.BigEndian);
    }

    #region Sensor Values and Events
    /// <summary>
    /// The value of the current (in Amps) flowing through the shunt resistor from the last reading.
    /// </summary>
    public Units.Current? Current => Conditions.Current;

    /// <summary>
    /// Raised when the current value changes.
    /// </summary>
    public event EventHandler<IChangeResult<Current>> CurrentUpdated = default!;

    /// <summary>
    /// The voltage from the last reading.
    /// </summary>
    public Units.Voltage? Voltage => Conditions.Voltage;

    /// <summary>
    /// Raised when the voltage value changes.
    /// </summary>
    public event EventHandler<IChangeResult<Voltage>> VoltageUpdated = default!;

    /// <summary>
    /// The power from the last reading.
    /// </summary>
    public Units.Power? Power => Conditions.Power;

    /// <summary>
    /// Raised when the power value changes.
    /// </summary>
    public event EventHandler<IChangeResult<Units.Power>> PowerUpdated = default!;

    /// <summary>
    /// Raise events for subscribers and notify of value changes.
    /// </summary>
    /// <param name="changeResult">The updated sensor data</param>
    protected override void RaiseEventsAndNotify(IChangeResult<(Units.Current? Current, Units.Voltage? Voltage, Units.Power? Power)> changeResult)
    {
        if (changeResult.New.Current is { } amps)
        {
            CurrentUpdated?.Invoke(this, new ChangeResult<Current>(amps, changeResult.Old?.Current));
        }
        if (changeResult.New.Voltage is { } volts)
        {
            VoltageUpdated?.Invoke(this, new ChangeResult<Voltage>(volts, changeResult.Old?.Voltage));
        }
        if (changeResult.New.Power is { } power)
        {
            PowerUpdated?.Invoke(this, new ChangeResult<Units.Power>(power, changeResult.Old?.Power));
        }
        base.RaiseEventsAndNotify(changeResult);
    }

    /// <inheritdoc/>
    protected override Task<(Units.Current? Current, Units.Voltage? Voltage, Units.Power? Power)> ReadSensor()
    {
        (Units.Current? Current, Units.Voltage? Voltage, Units.Power? Power) conditions;

        // TODO: What if Mode is not ContinuousAll, so some data might be stale?
        conditions.Current = ReadCurrent();
        conditions.Voltage = ReadBusVoltage();
        conditions.Power = ReadPower();

        return Task.FromResult(conditions);
    }

    /// <summary> Read the Current measurement from the power monitor IC. </summary>
    public abstract Units.Current ReadCurrent();
    
    /// <summary> Read the Voltage measurement from the power monitor IC. </summary>
    public abstract Units.Voltage ReadBusVoltage();
    
    /// <summary> Read the Voltage across the Shunt (sense) resistor from the power monitor IC. </summary>
    public abstract Units.Voltage ReadShuntVoltage();
    
    /// <summary> Read the Power measurement from the power monitor IC. </summary>
    public abstract Units.Power ReadPower();
    #endregion

    /// <summary> The manufacturer identification, if supported. Otherwise returns an empty string. </summary>
    public string ManufacturerID { get; internal set; } = string.Empty;

    /// <summary> The device identification number, if supported. Otherwise returns 0. </summary>
    public ushort DeviceID { get; internal set; }

    /// <summary> The Device Revision code, if supported. Otherwise returns 0. </summary>
    public byte DeviceRevision { get; internal set; }

    /// <summary> Method for reading the <see cref="ManufacturerID"/>, <see cref="DeviceID"/>, and <see cref="DeviceRevision"/> </summary>
    internal abstract void ReadDeviceInfo();
}