﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MobiFlight;

namespace MobiFlight
{
    public class OutputConfigItem : IBaseConfigItem, IFsuipcConfigItem, IXmlSerializable, ICloneable
    {
        // we initialize a cultureInfo object 
        // which is used for serialization
        // independently from current cultureInfo
        // @see: https://forge.simple-solutions.de/issues/275
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("de");

        public const int    FSUIPCOffsetNull = 0;        
        public int          FSUIPCOffset                { get; set; }
        public byte         FSUIPCSize                  { get; set; }
        public FSUIPCOffsetType   
                            FSUIPCOffsetType            { get; set; }
        public long         FSUIPCMask                  { get; set; }
        public double       FSUIPCMultiplier            { get; set; }
        public bool         FSUIPCBcdMode               { get; set; }
        public bool         ComparisonActive            { get; set; }
        public string       ComparisonOperand           { get; set; }
        public string       ComparisonValue             { get; set; }
        public string       ComparisonIfValue           { get; set; }
        public string       ComparisonElseValue         { get; set; }
        public string       DisplayType                 { get; set; }
        public string       DisplaySerial               { get; set; }
        public string       DisplayPin                  { get; set; }
        public byte         DisplayPinBrightness        { get; set; }
        // the display stuff
        public string       DisplayLedAddress           { get; set; }
        public byte         DisplayLedConnector         { get; set; }
        public byte         DisplayLedModuleSize        { get; set; }
        public bool         DisplayLedPadding           { get; set; }
        public string       DisplayLedPaddingChar       { get; set; }
        public List<string> DisplayLedDigits            { get; set; }
        public List<string> DisplayLedDecimalPoints     { get; set; }
        // the bcd driver stuff
        public List<string> BcdPins                     { get; set; }
        // the servo stuff
        public string       ServoAddress                { get; set; }
        public string       ServoMin                    { get; set; }
        public string       ServoMax                    { get; set; }
        public string       ServoMaxRotationPercent     { get; set; }
        public string       DisplayTrigger              { get; set; }


        public List<Precondition> Preconditions         { get; set; }

        public OutputConfigItem()
        {            
            FSUIPCOffset = FSUIPCOffsetNull;
            FSUIPCMask = 0xFF;
            FSUIPCMultiplier = 1.0;
            FSUIPCOffsetType = FSUIPCOffsetType.Integer;
            FSUIPCSize = 1;
            FSUIPCBcdMode = false;
            ComparisonActive = false;
            ComparisonOperand = "";
            ComparisonValue = "";
            ComparisonIfValue = "";
            ComparisonElseValue = "";

            //DisplayPin = "A01";
            DisplayPinBrightness = byte.MaxValue;
            DisplayLedConnector = 1;
            DisplayLedAddress = "0";
            DisplayLedPadding = false;
            DisplayLedPaddingChar = "0";
            DisplayLedModuleSize = 8;
            DisplayLedDigits = new List<string>();
            DisplayLedDecimalPoints = new List<string>();
            
            BcdPins = new List<string>() { "A01", "A02", "A03", "A04", "A05" };

            Preconditions = new List<Precondition>();
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public virtual void ReadXml(XmlReader reader)
        {  
            if (reader.ReadToDescendant("source"))
            {
                FSUIPCOffset = Int32.Parse(reader["offset"].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);                
                FSUIPCSize = Byte.Parse(reader["size"]);
                if (reader["offsetType"] != null && reader["offsetType"] != "")
                {
                    try
                    {
                        FSUIPCOffsetType = (FSUIPCOffsetType)Enum.Parse(typeof(FSUIPCOffsetType), reader["offsetType"]);
                    }
                    catch (Exception e)
                    {
                        FSUIPCOffsetType = MobiFlight.FSUIPCOffsetType.Integer;
                    }
                }
                else
                {
                    // Backward compatibility
                    // byte 1,2,4 -> int, this already is default
                    // exception
                    // byte 8 -> float
                    if (FSUIPCSize == 8) FSUIPCOffsetType = MobiFlight.FSUIPCOffsetType.Float;
                }
                FSUIPCMask = Int64.Parse(reader["mask"].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                FSUIPCMultiplier = Double.Parse(reader["multiplier"] , serializationCulture );
                if (reader["bcdMode"] != null && reader["bcdMode"] != "")
                {
                    FSUIPCBcdMode = Boolean.Parse(reader["bcdMode"]);
                }

            }

            if (reader.ReadToNextSibling("comparison"))
            {
                ComparisonActive = Boolean.Parse(reader["active"]);
                ComparisonValue = reader["value"];
                ComparisonOperand = reader["operand"];
                ComparisonIfValue = reader["ifValue"];
                ComparisonElseValue = reader["elseValue"];
            }

            if (reader.ReadToNextSibling("display"))
            {
                DisplayType = reader["type"];
                // preserve backward compatibility
                if (DisplayType == ArcazeLedDigit.OLDTYPE) DisplayType = ArcazeLedDigit.TYPE;

                DisplayPin = reader["pin"];
                DisplaySerial = reader["serial"];
                DisplayTrigger = reader["trigger"];

                if (reader["pinBrightness"] != null && reader["pinBrightness"] != "")
                {
                    DisplayPinBrightness = byte.Parse(reader["pinBrightness"]);
                }
                
                if (reader["ledAddress"] != null && reader["ledAddress"] != "")
                {
                    DisplayLedAddress = reader["ledAddress"];
                }
                
                if (reader["ledConnector"] != null && reader["ledConnector"] != "")
                {
                    DisplayLedConnector = byte.Parse(reader["ledConnector"]);                    
                }

                if (reader["ledModuleSize"] != null && reader["ledModuleSize"] != "")
                {
                    DisplayLedModuleSize = byte.Parse(reader["ledModuleSize"]);
                }

                if (reader["ledPadding"] != null && reader["ledPadding"] != "")
                {
                    DisplayLedPadding = Boolean.Parse(reader["ledPadding"]);
                }

                if (reader["ledPaddingChar"] != null && reader["ledPaddingChar"] != "")
                {
                    DisplayLedPaddingChar = reader["ledPaddingChar"];
                }

                // ignore empty values
                if (reader["ledDigits"] != null && reader["ledDigits"]!="")
                {
                    DisplayLedDigits = reader["ledDigits"].Split(',').ToList();
                }

                // ignore empty values
                if (reader["ledDecimalPoints"] != null && reader["ledDecimalPoints"]!="")
                {
                    DisplayLedDecimalPoints = reader["ledDecimalPoints"].Split(',').ToList();
                }

                // ignore empty values
                if (reader["bcdPins"] != null && reader["bcdPins"] != "")
                {
                    BcdPins = reader["bcdPins"].Split(',').ToList();
                }

                // ignore empty values
                if (reader["servoAddress"] != null && reader["servoAddress"] != "")
                {
                    ServoAddress = reader["servoAddress"];
                }
                if (reader["servoMin"] != null && reader["servoMin"] != "")
                {
                    ServoMin = reader["servoMin"];
                }
                if (reader["servoMax"] != null && reader["servoMax"] != "")
                {
                    ServoMax = reader["servoMax"];
                }

                if (reader["servoMaxRotationPercent"] != null && reader["servoMaxRotationPercent"] != "")
                {
                    ServoMaxRotationPercent = reader["servoMaxRotationPercent"];
                }
            }
            
            reader.ReadStartElement();

            // read precondition settings if present
            if (reader.LocalName == "precondition")
            {
                // do so
                Precondition tmp = new Precondition();
                tmp.ReadXml(reader);
                Preconditions.Add(tmp);
            }

            if (reader.LocalName == "preconditions")
            {
                bool atPosition = false;
                // read precondition settings if present
                if (reader.ReadToDescendant("precondition"))
                {
                    // load a list
                    do
                    {
                        Precondition tmp = new Precondition();
                        tmp.ReadXml(reader);
                        Preconditions.Add(tmp);
                        reader.ReadStartElement();
                    } while (reader.LocalName == "precondition");
                }
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("source");
                writer.WriteAttributeString("type", "FSUIPC");
                writer.WriteAttributeString("offset", "0x" + FSUIPCOffset.ToString("X4"));
                writer.WriteAttributeString("offsetType", FSUIPCOffsetType.ToString());
                writer.WriteAttributeString("size", FSUIPCSize.ToString());
                writer.WriteAttributeString("mask", "0x" + FSUIPCMask.ToString("X4"));
                writer.WriteAttributeString("multiplier", FSUIPCMultiplier.ToString(serializationCulture));
                writer.WriteAttributeString("bcdMode", FSUIPCBcdMode.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("comparison");
                writer.WriteAttributeString("active", ComparisonActive.ToString());
                writer.WriteAttributeString("value", ComparisonValue);
                writer.WriteAttributeString("operand", ComparisonOperand);
                writer.WriteAttributeString("ifValue", ComparisonIfValue);
                writer.WriteAttributeString("elseValue", ComparisonElseValue);
            writer.WriteEndElement();

            writer.WriteStartElement("display");
                writer.WriteAttributeString("type", DisplayType);
                writer.WriteAttributeString("serial", DisplaySerial);

                if ( DisplayTrigger != null)
                    writer.WriteAttributeString("trigger", DisplayTrigger);

                if (DisplayType == ArcazeLedDigit.TYPE)
                {
                    writer.WriteAttributeString("ledAddress", DisplayLedAddress);
                    writer.WriteAttributeString("ledConnector", DisplayLedConnector.ToString());
                    writer.WriteAttributeString("ledModuleSize", DisplayLedModuleSize.ToString());
                    writer.WriteAttributeString("ledPadding", DisplayLedPadding.ToString());
                    writer.WriteAttributeString("ledPaddingChar", DisplayLedPaddingChar);

                    if (DisplayLedDigits.Count > 0)
                    {
                        writer.WriteAttributeString("ledDigits", String.Join(",", DisplayLedDigits));
                    }

                    if (DisplayLedDecimalPoints.Count > 0)
                    {
                        writer.WriteAttributeString("ledDecimalPoints", String.Join(",", DisplayLedDecimalPoints));
                    }
                }
                else if (DisplayType == ArcazeBcd4056.TYPE)
                {
                    writer.WriteAttributeString("bcdPins", String.Join(",",BcdPins));
                }
                else if (DisplayType == MobiFlight.DeviceType.Servo.ToString("F"))
                {
                    writer.WriteAttributeString("servoAddress", ServoAddress);
                    writer.WriteAttributeString("servoMin", ServoMin);
                    writer.WriteAttributeString("servoMax", ServoMax);
                    writer.WriteAttributeString("servoMaxRotationPercent", ServoMaxRotationPercent);
                }
                else if (DisplayType == MobiFlight.DeviceType.Stepper.ToString("F"))
                {
                    
                }
                else
                {
                    writer.WriteAttributeString("pin", DisplayPin);
                    writer.WriteAttributeString("pinBrightness", DisplayPinBrightness.ToString());

                }
            writer.WriteEndElement();

            writer.WriteStartElement("preconditions");
            foreach (Precondition p in Preconditions)
            {
                p.WriteXml(writer);
            }
            writer.WriteEndElement();
        }

        public object Clone()
        {
            OutputConfigItem clone = new OutputConfigItem();
            clone.FSUIPCOffset              = this.FSUIPCOffset;
            clone.FSUIPCOffsetType          = this.FSUIPCOffsetType;
            clone.FSUIPCSize                = this.FSUIPCSize;
            clone.FSUIPCMask                = this.FSUIPCMask;
            clone.FSUIPCMultiplier          = this.FSUIPCMultiplier;
            clone.FSUIPCBcdMode             = this.FSUIPCBcdMode;
            clone.ComparisonActive          = this.ComparisonActive;
            clone.ComparisonOperand         = this.ComparisonOperand;
            clone.ComparisonValue           = this.ComparisonValue;
            clone.ComparisonIfValue         = this.ComparisonIfValue;
            clone.ComparisonElseValue       = this.ComparisonElseValue;
            clone.DisplayType               = this.DisplayType;
            clone.DisplaySerial             = this.DisplaySerial;
            clone.DisplayPin                = this.DisplayPin;
            clone.DisplayPinBrightness      = this.DisplayPinBrightness;
            // the display stuff
            clone.DisplayLedAddress         = this.DisplayLedAddress;
            clone.DisplayLedConnector       = this.DisplayLedConnector;
            clone.DisplayLedModuleSize      = this.DisplayLedModuleSize;
            clone.DisplayLedPadding         = this.DisplayLedPadding;
            clone.DisplayLedDigits          = new List<string>(this.DisplayLedDigits); // we have to create an new object to clone, fix for https://forge.simple-solutions.de/issues/307
            clone.DisplayLedDecimalPoints   = new List<string>(this.DisplayLedDecimalPoints);
            // the bcd driver stuff
            clone.BcdPins                   = new List<string>(this.BcdPins);

            clone.DisplayTrigger            = this.DisplayTrigger;

            clone.ServoAddress              = this.ServoAddress;
            clone.ServoMax                  = this.ServoMax;
            clone.ServoMin                  = this.ServoMin;
            clone.ServoMaxRotationPercent   = this.ServoMaxRotationPercent;

            foreach (Precondition p in Preconditions)
            {
                clone.Preconditions.Add(p.Clone() as Precondition);
            }

            return clone;
        }
    }
}