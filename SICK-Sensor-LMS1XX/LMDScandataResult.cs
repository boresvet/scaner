/*
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
using NLog;

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
        public List<int> DistancesData;


        public Logger logger = LogManager.GetCurrentClassLogger();


        public static LMDScandataResult Parse(Stream br)
        {
            //if (br.ReadByte() != StartMark)
            //    throw new LMSBadDataException("StartMark not found");
            
            var result = new LMDScandataResult();
            while (true)
            {
                if (br.ReadChar() != StartMark)
                    return null;
                if (br.ReadChar() != 's')
                    return null;
                if (br.ReadChar() != 'R')
                    return null;
                if (br.ReadChar() != 'A')
                    return null;
                if (br.ReadChar() != ' ')
                    return null;

            }
            //rawData.Write
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

            result.DistancesData = br.ReadListAsHex(result.AmountOfData ?? 0);

            while (br.ReadByte() != EndMark);
            //br.Flush();
            result.IsError = false;
            return result;
        }

        public static LMDScandataResult ParseContinious(byte[] data)
        {
            using var ms = new MemoryStream(data);
            return ParseContinious(ms);
        }
        public static LMDScandataResult ParseContinious(Stream stream)
        {
            var logger = LogManager.GetCurrentClassLogger();
            //if (br.ReadByte() != StartMark)
            //    throw new LMSBadDataException("StartMark not found");
            logger.Debug("Начинаем срать байтиками");        

            var result = new LMDScandataResult();
            logger.Debug("Стари кастрации заголовка");        
            while (true)
            {
                if (stream.ReadByte() != StartMark)
                    return null;
                if (stream.ReadChar() != 's')
                    return null;
                if (stream.ReadChar() != 'S')
                    return null;
                if (stream.ReadChar() != 'N')
                    return null;
                if (stream.ReadChar() != ' ')
                    return null;
                break;

            }
            logger.Debug("Заголовок кастрирован");        

            //result.CommandType = br.ReadWord();
            //if (result.CommandType != "sRA")
            //{
            //    br.Flush();
            //    return result;
            //}
            logger.Debug("Начало творения гадостей");        
            result.Command = stream.ReadWord();
            logger.Debug("1");        
            result.VersionNumber = stream.ReadIntAsHex();
            logger.Debug("2");        
            result.DeviceNumber = stream.ReadIntAsHex();
            logger.Debug("3");        
            result.SerialNumber = (int)stream.ReadUIntAsHex();
            logger.Debug("4");        
            result.DeviceStatus = $"{stream.ReadWord()}-{stream.ReadWord()}";
            logger.Debug("5");        
            result.TelegramCounter = (int)stream.ReadUIntAsHex();  // todo uint
            logger.Debug("6");        
            result.ScanCounter = (int)stream.ReadUIntAsHex();
            logger.Debug("7");        
            result.TimeSinceStartup = stream.ReadUIntAsHex() /*/ 1000000*/;
            logger.Debug("8");        
            result.TimeOfTransmission = stream.ReadUIntAsHex()/* / 1000000*/;
            logger.Debug("9");        
            result.StatusOfDigitalInputs = $"{stream.ReadWord()}-{stream.ReadWord()}";
            logger.Debug("10");        
            result.StatusOfDigitalOutputs = $"{stream.ReadWord()}-{stream.ReadWord()}";
            logger.Debug("11");        
            result.Reserved = stream.ReadIntAsHex();
            logger.Debug("12");        
            result.ScanFrequency = stream.ReadIntAsHex() / 100d;
            logger.Debug("13");        
            result.MeasurementFrequency = stream.ReadIntAsHex() / 10d;
            logger.Debug("14");        
            result.AmountOfEncoder = stream.ReadIntAsHex();
            logger.Debug("15");        
            if (result.AmountOfEncoder > 0)
            {
                result.EncoderPosition = stream.ReadIntAsHex();
                result.EncoderSpeed = stream.ReadIntAsHex();
            }
            logger.Debug("16");        
            result.AmountOf16BitChannels = stream.ReadIntAsHex();
            logger.Debug("17");        
            result.Content = stream.ReadWord();
            logger.Debug("18");        
            result.ScaleFactor = stream.ReadWord();
            logger.Debug("19");        
            result.ScaleFactorOffset = stream.ReadWord();
            logger.Debug("20");        
            result.StartAngle = stream.ReadIntAsHex() / 10000.0;
            logger.Debug("21");        
            result.SizeOfSingleAngularStep = stream.ReadUIntAsHex() / 10000.0;
            logger.Debug("22");        
            result.AmountOfData = stream.ReadIntAsHex();

            logger.Debug("23");        
            result.DistancesData = stream.ReadListAsHex(result.AmountOfData ?? 0);
            logger.Debug("23.1");        

            var AmountOf8BitData1 = stream.ReadIntAsHex();
            logger.Debug("23.2");        
            var Position = stream.ReadIntAsHex();
            logger.Debug("23.3");        
            var Comment = stream.ReadIntAsHex();
            logger.Debug("23.4");        
            var Time = stream.ReadIntAsHex();
            logger.Debug("23.5");        
            var EventInfo = stream.ReadIntAsHex();
            
            logger.Debug("24");        
            while (stream.ReadByte() != EndMark);
            //br.Flush();
            result.IsError = false;
            logger.Debug("Закончили");        
            return result;
        }
        public LMDScandataResult(byte[] rawData)
        {
            IsError = true;
            ErrorException = null;
            RawData = rawData;
            RawDataString = Encoding.ASCII.GetString(rawData).Trim();
            DistancesData = new List<int>();
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
