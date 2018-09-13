﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GC_Final.Models;

namespace GC_Final.Controllers
{

    public class PartDetails
    {

        public string Name { set; get; }
        public string Desc { set; get; }
        public string Brand { set; get; }
        public double Price { set { Price = value / 100; } get { return Price; } }
        public string Stars { set; get; }
        public string Manufacturer { set; get; }

        public PartDetails()
        {
            Name = "";
            Desc = "";
            Brand = "";
            Price = 0.00;
            Stars = "";
            Manufacturer = "";
        }

    }

    public class PartBag
    {
        private int _Count;
        private List<PartDetails> _Bag;

        public int Count { get { return _Bag.Count; } }
        public Dictionary<string, int> Quantities {
            get
            {
                Dictionary<string, int> _out = new Dictionary<string, int>();

                foreach (PartDetails p in _Bag)
                {
                    if (_out[p.Name] != 0)
                    {
                        _out[p.Name]++;
                    }
                    else
                    {
                        _out.Add(p.Name, 1);
                    }
                }

                return _out;
            }
        }

        public void Add(PartDetails part)
        {
            _Bag.Add(part);
        }
        //Add method to add by ID
        public void Remove(PartDetails part)
        {
            _Bag.Remove(part);
        }
    }

    public class BuildDetails
    {
        
        public string Name { set; get; }
        public string OwnerID { set; get; }
        public CaseDetails Case { set; get; }
        public MotherBoardDetails MB { set; get; }
        public CPUDetails CPU { set; get; }
        public PartBag GPU { set; get; }
        public PSUDetails PSU { set; get; }
        public PartBag RAM { set; get; }
        public PartBag Monitor { set; get; }
        public PartBag OD { set; get; }
        public PartBag HD { set; get; }
        public PartBag Peripherals { set; get; }
        public PartBag PCI { set; get; }

    }

    public class CaseDetails : PartDetails
    {

        public string CaseID { set; get; }
        public string PID;
        public int Size;
        public int Dimensions;
        public byte PortCount;
        public string Drives;
        public bool SSDSupport;
        public string Ports;
        public string Style;

        public CaseDetails() : base()
        {
            CaseID = "";
            PID = "";
            Size = 0;
            Dimensions = 0;
            PortCount = 0;
            Drives = "";
            SSDSupport = false;
            Ports = "";
            Style = "";
        }

    }

    public class MotherBoardDetails : PartDetails
    {
        
        public string MBID { set; get; }
        public string PID { set; get; }
        public string Socket { set; get; }
        public string Chipset { set; get; }
        public byte RAMSlots { set; get; }
        public byte SLI { set; get; }
        public byte XFIRE { set; get; }
        public string FormFactor { set; get; }

        public MotherBoardDetails() : base()
        {

            MBID = "";
            PID = "";
            Socket = "";
            Chipset = "";
            RAMSlots = 0;
            SLI = 0;
            XFIRE = 0;
            FormFactor = "";

        }

    }

    public class CPUDetails : PartDetails
    {

        public string CPUID { set; get; }
        public string PID { set; get; }
        public string Cache { set; get; }
        public int Wattage { set { Wattage = Convert.ToInt32(value); } get { return Wattage; } }
        public bool Fan { set; get; }
        public byte Threads { set { Threads = Convert.ToByte(value); } get { return Threads; } }
        public string ProcessingUnits { set; get; }
        public double Speed { set { Speed = Convert.ToDouble(value); } get { return Speed; } }

        public CPUDetails() : base()
        {

            CPUID = "";
            PID = "";
            Cache = "";
            Wattage = 0;
            Fan = false;
            Threads = 0;
            ProcessingUnits = "";
            Speed = 0.0;

        }

    }

    public class PSUDetails
    {
        public string PSUID;
        public string PID;
        public string PowerSource;
        public string Dimensions;
        public int Wattage;

        public PSUDetails() : base()
        {

        }
    }

    public partial class ZincParseController : ApiController
    {



    }
}