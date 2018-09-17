﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GC_Final.Models
{
    public partial class PCCase
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public PCCase(string name)
        {
            CaseID = Guid.NewGuid().ToString("D");
            Style = " ";
            Name = name;
            Description = " ";
            Brand = " ";
            Manufacturer = " ";
            ImageLink = " ";
        }
    }

    public partial class CPU
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public CPU(string name)
        {
            CPUID = Guid.NewGuid().ToString("D");
            ProductID = "xxxxxxxxxx";
            Cache = " ";
            Name = name;
            Description = " ";
            Brand = " ";
            Manufacturer = " ";
            ImageLink = " ";
        }
    }

    public partial class GPU
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public GPU(string name)
        {
            GPUID = Guid.NewGuid().ToString("D");
            ProductID = "xxxxxxxxxx";
            RAMType = " ";
            Name = name;
            Description = " ";
            Brand = " ";
            Manufacturer = " ";
            ImageLink = " ";
        }
    }

    public partial class HardDrive
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public HardDrive(string name)
        {
            HardDriveID = Guid.NewGuid().ToString("D");
            ProductID = "xxxxxxxxxx";
            Interface = " ";
            SlotSize = true;
            Capacity = 0;
            CapacityUnits = " ";
            Description = " ";
            Brand = " ";
            Price = 0;
            Stars = 0.0F;
            ImageLink = " ";
            Manufacturer = " ";
        }
    }

    public partial class Monitor
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public Monitor(string name)
        {
            ProductID = "xxxxxxxxxx";
            MonitorID = Guid.NewGuid().ToString("D");
            Name = name;
            Description = " ";
            Brand = " ";
            Price = 0;
            Stars = 0.0F;
            Manufacturer = " ";
            ImageLink = " ";
        }
    }

    public partial class Motherboard
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public Motherboard(string name)
        {
            MotherboardID = Guid.NewGuid().ToString("D");
            ProductID = "xxxxxxxxxx";
            Socket = " ";
            Chipset = " ";
            RAMType = " ";
            FormFactor = " ";
            Name = name;
            Description = " ";
            Brand = " ";
            Price = 0;
            Stars = 0.0F;
            Manufacturer = " ";
            ImageLink = " ";
        }
    }

    public partial class OpticalDriver
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public OpticalDriver(string name)
        {
            OpticalDriverID = Guid.NewGuid().ToString("D");
            ProductID = "xxxxxxxxxx";
            Rewritable = false;
            Interface = " ";
            ReadSpeed = 0;
            WriteSpeed = 0;
            Wattage = 0;
            Name = name;
            Description = " ";
            Brand = " ";
            Price = 0;
            Stars = 0.0F;
            ImageLink = " ";
            Manufacturer = " ";
        }
    }

    public partial class PCICard
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public PCICard(string name)
        {
            PCIcardID = Guid.NewGuid().ToString("D");
            Description = " ";
            Name = name;
            ProductID = "xxxxxxxxxx";
            Brand = " ";
            Price = 0;
            Stars = 0.0F;
            ImageLink = " ";
            Manufacturer = " ";
        }
    }

    public partial class Peripheral
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public Peripheral(string name)
        {
            PeripheralsID = Guid.NewGuid().ToString("D");
            ProductID = "xxxxxxxxxx";
            Name = name;
            Description = " ";
            Brand = " ";
            Price = 0;
            Stars = 0.0F;
            ImageLink =  " ";
            Manufacturer = " ";
        }
    }

    public partial class PSU
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public PSU(string name)
        {
            PSUID = Guid.NewGuid().ToString("D");
            Name = name;
        }
    }

    public partial class RAM
    {
        public double GetPrice()
        {
            return Convert.ToDouble(Price) / 100;
        }

        public RAM(string name)
        {
            RAMID = Guid.NewGuid().ToString("D");
            Name = name;
        }
    }

    public partial class Build
    {
        public void Glorp(Build _goop)
        {

            if (_goop.Motherboard != null && Motherboard == null)
            {
                MBID = _goop.Motherboard.MotherboardID;
                Motherboard = _goop.Motherboard;
            }
            if (_goop.CPU != null && CPU == null)
            {
                CPUID = _goop.CPU.CPUID;
                CPU = _goop.CPU;
            }
            if (_goop.GPU != null && GPU == null)
            {
                GPUID = _goop.GPU.GPUID;
                GPU = _goop.GPU;
                GPUCount = _goop.GPUCount;
            }
            if (_goop.PSU != null && PSU == null)
            {
                PSUID = _goop.PSU.PSUID;
                PSU = _goop.PSU;
            }
            if (_goop.PCCase != null && PCCase == null)
            {
                CaseID = _goop.PCCase.CaseID;
                PCCase = _goop.PCCase;
            }

        }

        public double GetPrice()
        {
            double _total = 0.0;

            _total += PCCase.GetPrice();
            _total += Motherboard.GetPrice();
            _total += CPU.GetPrice();
            _total += GPU.GetPrice() * (double)GPUCount;
            _total += PSU.GetPrice();
            foreach (RAM r in BuildsRAMs.Select(x => x.RAM).ToArray())
            {
                _total += r.GetPrice();
            }
            if (BuildDisks != null)
            {
                foreach (HardDrive d in BuildDisks.Select(x => x.HardDrive).ToArray())
                {
                    _total += d.GetPrice();
                }
            }
            if (BuildODs != null)
            {
                foreach (OpticalDriver o in BuildODs.Select(x => x.OpticalDriver).ToArray())
                {
                    _total += o.GetPrice();
                }
            }
            if (BuildMonitors != null)
            {
                foreach (Monitor m in BuildMonitors.Select(x => x.Monitor).ToArray())
                {
                    m.GetPrice();
                }
            }
            if (BuildPCIs != null)
            {
                foreach (PCICard c in BuildPCIs.Select(x => x.PCICard).ToArray())
                {
                    _total += c.GetPrice();
                }
            }
            if (BuildsPeripherals != null)
            {
                foreach (Peripheral p in BuildsPeripherals.Select(x => x.Peripheral).ToArray())
                {
                    _total += p.GetPrice();
                }
            }

            return _total;
        }

        public Build(Controllers.BuildDetails bass)
        {

            BuildName = bass.Name;
            BuildID = bass.BuildID;
            OwnerID = bass.OwnerID;
            CaseID = bass.Case.CaseID;
            MBID = bass.MB.MBID;
            CPUID = bass.CPU.CPUID;
            PSUID = bass.PSU.PSUID;
            GPUID = bass.GPU.GPUID;
            PSUID = bass.PSU.PSUID;
            GPUCount = bass.GPUCount;

        }

        public Build(string name)
        {
            BuildName = name;
            PCCase = new PCCase();
            Motherboard = new Motherboard();
            CPU = new CPU();
            PSU = new PSU();
            GPU = new GPU();
            GPUCount = 0;
        }
    }
}