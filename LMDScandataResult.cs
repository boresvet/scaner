﻿/*
 * A C#.NET class to communicate with SICK SENSOR LMS1xx
 * 
 * Author : beolabs.io / Benjamin Oms
 * Update : 12/06/2017
 * Github : https://github.com/beolabs-io/SICK-Sensor-LMS1XX
 * 
 * --- MIT LICENCE ---
 * 
 * Copyright (c) 2017 beolabs.io
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BSICK.Sensors.LMS1xx
{
    public class LMDScandataResult
    {
        public const byte StartMark = 0x02;
        public const byte EndMark = 0x03;

        public bool IsError;
        public Exception ErrorException;
        public byte[] RawData;
        public String RawDataString;
        public String CommandType;
        public String Command;
        public int? VersionNumber;
        public int? DeviceNumber;
        public int? SerialNumber;
        public String DeviceStatus;
        public int? TelegramCounter;
        public int? ScanCounter;
        public uint? TimeSinceStartup;
        public uint? TimeOfTransmission;
        public String StatusOfDigitalInputs;
        public String StatusOfDigitalOutputs;
        public int? Reserved;
        public double? ScanFrequency;
        public double? MeasurementFrequency;
        public int? AmountOfEncoder;
        public int? EncoderPosition;
        public int? EncoderSpeed;
        public int? AmountOf16BitChannels;
        public String Content;
        public String ScaleFactor;
        public String ScaleFactorOffset;
        public double? StartAngle;
        public double? SizeOfSingleAngularStep;
        public int? AmountOfData;
        public List<double> DistancesData;

        public static LMDScandataResult Parse(Stream br)
        {
            //if (br.ReadByte() != StartMark)
            //    throw new LMSBadDataException("StartMark not found");
            var result = new LMDScandataResult();
            while (true)
            {
                if (br.ReadByte() != StartMark)
                    continue;
                if (br.ReadChar() != 's')
                        continue;
                if (br.ReadChar() != 'R')
                    continue;
                if (br.ReadChar() != 'A')
                    continue;
                if (br.ReadChar() != ' ')
                    continue;
                break;

            }
            //result.CommandType = br.ReadWord();
            //if (result.CommandType != "sRA")
            //{
            //    br.Flush();
            //    return result;
            //}
            result.Command = br.ReadWord();
            result.VersionNumber = br.ReadIntAsHex();
            result.DeviceNumber = br.ReadIntAsHex();
            result.SerialNumber = (int) br.ReadUIntAsHex();
            result.DeviceStatus = $"{br.ReadWord()}-{br.ReadWord()}";
            result.TelegramCounter = (int)br.ReadUIntAsHex();  // todo uint
            result.ScanCounter = (int)br.ReadUIntAsHex();
            result.TimeSinceStartup = br.ReadUIntAsHex() / 1000000;
            result.TimeOfTransmission = br.ReadUIntAsHex() / 1000000;
            result.StatusOfDigitalInputs = $"{br.ReadWord()}-{br.ReadWord()}";
            result.StatusOfDigitalOutputs = $"{br.ReadWord()}-{br.ReadWord()}";
            result.Reserved = br.ReadIntAsHex();
            result.ScanFrequency = br.ReadIntAsHex() / 100d;
            result.MeasurementFrequency = br.ReadIntAsHex() / 10d;
            result.AmountOfEncoder = br.ReadIntAsHex();
            if (result.AmountOfEncoder > 0)
            {
                result.EncoderPosition = br.ReadIntAsHex();
                result.EncoderSpeed = br.ReadIntAsHex();
            }
            result.AmountOf16BitChannels = br.ReadIntAsHex();
            result.Content = br.ReadWord();
            result.ScaleFactor = br.ReadWord();
            result.ScaleFactorOffset = br.ReadWord();
            result.StartAngle = br.ReadIntAsHex() / 10000.0;
            result.SizeOfSingleAngularStep = br.ReadUIntAsHex() / 10000.0;
            result.AmountOfData = br.ReadIntAsHex();

            result.DistancesData = br.ReadListAsHex(result.AmountOfData ?? 0, (x) => ((double)x) / 1000.0);

            //while (br.ReadByte() != EndMark);
            br.Flush();
            result.IsError = false;
            return result;
        }

        public static LMDScandataResult ParseContinious(Stream br)
        {
            //if (br.ReadByte() != StartMark)
            //    throw new LMSBadDataException("StartMark not found");
            var result = new LMDScandataResult();
            while (true)
            {
                if (br.ReadByte() != StartMark)
                    continue;
                if (br.ReadChar() != 's')
                    continue;
                if (br.ReadChar() != 'S')
                    continue;
                if (br.ReadChar() != 'N')
                    continue;
                if (br.ReadChar() != ' ')
                    continue;
                break;

            }
            //result.CommandType = br.ReadWord();
            //if (result.CommandType != "sRA")
            //{
            //    br.Flush();
            //    return result;
            //}
            result.Command = br.ReadWord();
            result.VersionNumber = br.ReadIntAsHex();
            result.DeviceNumber = br.ReadIntAsHex();
            result.SerialNumber = (int)br.ReadUIntAsHex();
            result.DeviceStatus = $"{br.ReadWord()}-{br.ReadWord()}";
            result.TelegramCounter = (int)br.ReadUIntAsHex();  // todo uint
            result.ScanCounter = (int)br.ReadUIntAsHex();
            result.TimeSinceStartup = br.ReadUIntAsHex() / 1000000;
            result.TimeOfTransmission = br.ReadUIntAsHex() / 1000000;
            result.StatusOfDigitalInputs = $"{br.ReadWord()}-{br.ReadWord()}";
            result.StatusOfDigitalOutputs = $"{br.ReadWord()}-{br.ReadWord()}";
            result.Reserved = br.ReadIntAsHex();
            result.ScanFrequency = br.ReadIntAsHex() / 100d;
            result.MeasurementFrequency = br.ReadIntAsHex() / 10d;
            result.AmountOfEncoder = br.ReadIntAsHex();
            if (result.AmountOfEncoder > 0)
            {
                result.EncoderPosition = br.ReadIntAsHex();
                result.EncoderSpeed = br.ReadIntAsHex();
            }
            result.AmountOf16BitChannels = br.ReadIntAsHex();
            result.Content = br.ReadWord();
            result.ScaleFactor = br.ReadWord();
            result.ScaleFactorOffset = br.ReadWord();
            result.StartAngle = br.ReadIntAsHex() / 10000.0;
            result.SizeOfSingleAngularStep = br.ReadUIntAsHex() / 10000.0;
            result.AmountOfData = br.ReadIntAsHex();

            result.DistancesData = br.ReadListAsHex(result.AmountOfData ?? 0, (x) => ((double)x) / 1000.0);

            var AmountOf8BitData1 = br.ReadIntAsHex();
            var Position = br.ReadIntAsHex();
            var Comment = br.ReadIntAsHex();
            var Time = br.ReadIntAsHex();
            var EventInfo = br.ReadIntAsHex();
            
            while (br.ReadByte() != EndMark);
            //br.Flush();
            result.IsError = false;
            return result;
        }
        public LMDScandataResult(byte[] rawData)
        {
            IsError = true;
            ErrorException = null;
            RawData = rawData;
            RawDataString = Encoding.ASCII.GetString(rawData).Trim();
            DistancesData = new List<double>();
            CommandType = String.Empty;
            Command = String.Empty;
            VersionNumber = null;
            DeviceNumber = null;
            SerialNumber = null;
            DeviceStatus = String.Empty;
            TelegramCounter = null;
            ScanCounter = null;
            TimeSinceStartup = null;
            TimeOfTransmission = null;
            StatusOfDigitalInputs = String.Empty;
            StatusOfDigitalOutputs = String.Empty;
            Reserved = null;
            ScanFrequency = null;
            MeasurementFrequency = null;
            AmountOfEncoder = null;
            EncoderPosition = null;
            EncoderSpeed = null;
            AmountOf16BitChannels = null;
            Content = String.Empty;
            ScaleFactor = String.Empty;
            ScaleFactorOffset = String.Empty;
            StartAngle = null;
            SizeOfSingleAngularStep = null;
            AmountOfData = null;
        }

        public LMDScandataResult()
        {
            IsError = true;
            ErrorException = null;
            RawData = null ;
            RawDataString = String.Empty;
            DistancesData = null;
            CommandType = String.Empty;
            Command = String.Empty;
            VersionNumber = null;
            DeviceNumber = null;
            SerialNumber = null;
            DeviceStatus = String.Empty;
            TelegramCounter = null;
            ScanCounter = null;
            TimeSinceStartup = null;
            TimeOfTransmission = null;
            StatusOfDigitalInputs = String.Empty;
            StatusOfDigitalOutputs = String.Empty;
            Reserved = null;
            ScanFrequency = null;
            MeasurementFrequency = null;
            AmountOfEncoder = null;
            EncoderPosition = null;
            EncoderSpeed = null;
            AmountOf16BitChannels = null;
            Content = String.Empty;
            ScaleFactor = String.Empty;
            ScaleFactorOffset = String.Empty;
            StartAngle = null;
            SizeOfSingleAngularStep = null;
            AmountOfData = null;
        }
    }

}
