﻿using GC_Final.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using GC_Final.Controllers;

namespace GC_Final.Controllers
{
    public partial class ZincParseController : ApiController
    {
        public object ViewBag { get; private set; }


        public static JObject GetParts(string partType)
        {
            HttpWebRequest apiRequest = WebRequest.CreateHttp($"https://api.zinc.io/v1/search?query={partType}&page=1&retailer=amazon");
            apiRequest.Headers.Add("Authorization", ConfigurationManager.AppSettings["ZINCkey"]);
            apiRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)";

            HttpWebResponse apiResponse = (HttpWebResponse)apiRequest.GetResponse();

            StreamReader responseData = new StreamReader(apiResponse.GetResponseStream());

            string info = responseData.ReadToEnd();
            
            JObject jsoninfo = JObject.Parse(info);

            return jsoninfo;
        }

        public static List<JObject> GetPartData(JObject jsoninfo)
        {
            List<JObject> Parts = new List<JObject>();

            if (jsoninfo.Count < 1)
            {
                return null;
            }

            for (int i = 0; i <= 14; i++)
            {
                string x = jsoninfo["results"][i]["product_id"].ToString();

                HttpWebRequest apiRequest = WebRequest.CreateHttp($"https://api.zinc.io/v1/products/{x}?retailer=amazon");
                apiRequest.Headers.Add("Authorization", ConfigurationManager.AppSettings["ZINCKey"]); //used to add keys
                apiRequest.Headers.Add("-u", ConfigurationManager.AppSettings["apizinc"]);
                apiRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)";

                HttpWebResponse apiResponse = (HttpWebResponse)apiRequest.GetResponse();

                StreamReader responseData = new StreamReader(apiResponse.GetResponseStream());

                string partinfo = responseData.ReadToEnd();

                JObject Temp = JObject.Parse(partinfo);

                Parts.Add(Temp);
            }

            return Parts;
        }

        //overloaed searches specific part
        public static JObject GetPartData(string partid)
        {
            HttpWebRequest apiRequest = WebRequest.CreateHttp($"https://api.zinc.io/v1/products/{partid}?retailer=amazon");
            apiRequest.Headers.Add("Authorization", ConfigurationManager.AppSettings["ZINCKey"]);
            apiRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)";

            HttpWebResponse apiResponse = (HttpWebResponse)apiRequest.GetResponse();

            //NEEDS - Add if apiresponse error

            StreamReader responseData = new StreamReader(apiResponse.GetResponseStream());

            string partdetails = responseData.ReadToEnd();

            JObject ChosenPart = JObject.Parse(partdetails);

            return ChosenPart;
        }

        //for mass saves
        public static void SaveGPUsToDB()
        {
            List<JObject> gpuparts = new List<JObject>();
            gpuparts = GetPartData(GetParts("GPU"));
            Entities ORM = new Entities();

            foreach (JObject chosenpart in gpuparts)
            {
                string y = chosenpart["product_id"].ToString();
                GPU tempGPU = new GPU(chosenpart["title"].ToString());

                List<GPU> z = new List<GPU>();
                z = ORM.GPUs.Where(x => x.ProductID == y).ToList();

                if (z.Count < 1)
                {
                    tempGPU.ProductID = chosenpart["product_id"].ToString();
                    tempGPU.Description = "x"; //chosenpart["product_description"].ToString();
                    tempGPU.Brand = chosenpart["brand"].ToString();
                    try { tempGPU.Price = int.Parse(chosenpart["price"].ToString()); }
                    catch { tempGPU.Price = null; }
                    try
                    {
                        tempGPU.Stars = float.Parse(chosenpart["stars"].ToString());
                    }
                    catch
                    {
                        tempGPU.Stars = null;
                    }
                    tempGPU.ImageLink = chosenpart["main_image"].ToString();
                    tempGPU.Manufacturer = "x";
                    int[] res = GetMaxScreenResolution(ParseToArray(chosenpart["feature_bullets"]));
                    tempGPU.ResX = res[0];
                    tempGPU.ResY = res[1];
                    tempGPU.RAMType = GetRAMType(ParseToArray(chosenpart["feature_bullets"]));
                    tempGPU.RAMAmount = GetRAMSlots(ParseToArray(chosenpart["feature_bullets"]));
                    tempGPU.MultiGPULimit = null;
                    tempGPU.MultiGPUType = null;
                    tempGPU.MaxMonitors = null;

                    ORM.GPUs.Add(tempGPU);
                    ORM.SaveChanges();
                }
            }
        }

        //for single save
        public static ActionResult GetSaveGPUToDB(string partid)
        {
            JObject chosenpart= GetPartData(partid);
            Entities ORM = new Entities();

            GPU tempGPU = new GPU(chosenpart["title"].ToString());

            List<GPU> z = new List<GPU>();
            z = ORM.GPUs.Where(x => x.ProductID == partid).ToList();

            if (z.Count < 1)
            {
                tempGPU.ProductID = chosenpart["product_id"].ToString();
                tempGPU.Description = "x"; //chosenpart["product_description"].ToString();
                tempGPU.Brand = chosenpart["brand"].ToString();
                try { tempGPU.Price = int.Parse(chosenpart["price"].ToString()); }
                catch { tempGPU.Price = null; }
                try { tempGPU.Stars = float.Parse(chosenpart["stars"].ToString()); }
                catch { tempGPU.Stars = null;  }
                tempGPU.ImageLink = chosenpart["main_image"].ToString();
                tempGPU.Manufacturer = "x";
                int[] res = GetMaxScreenResolution(ParseToArray(chosenpart["feature_bullets"]));
                tempGPU.ResX = res[0];
                tempGPU.ResY = res[1];
                tempGPU.RAMType = GetRAMType(ParseToArray(chosenpart["feature_bullets"]));
                tempGPU.RAMAmount = GetRAMSlots(ParseToArray(chosenpart["feature_bullets"]));
                tempGPU.MultiGPULimit = null;
                tempGPU.MultiGPUType = null;
                tempGPU.MaxMonitors = null;

                ORM.GPUs.Add(tempGPU);
                ORM.SaveChanges();
            }
            return null;
            
        }
        public static void SaveCPUToDB(string partid)
        {
            JObject chosenpart = GetPartData(partid);
            Entities ORM = new Entities();

            CPU tempCPU = new CPU(chosenpart["title"].ToString());

            List<CPU> z = new List<CPU>();
            z = ORM.CPUs.Where(x => x.ProductID == partid).ToList();

            if (z.Count < 1)
            {
                tempCPU.ProductID = chosenpart["product_id"].ToString();
                tempCPU.Description = "x";
                tempCPU.Brand = chosenpart["brand"].ToString();
                try
                {
                    tempCPU.Price = int.Parse(chosenpart["price"].ToString());
                }
                catch
                {
                    tempCPU.Price = null;
                }
                try
                {
                    tempCPU.Stars = float.Parse(chosenpart["stars"].ToString());
                }
                catch
                {
                    tempCPU.Stars = null;
                }
                tempCPU.ImageLink = chosenpart["main_image"].ToString();
                tempCPU.Manufacturer = "x";
                try
                {
                    tempCPU.MaxSpeed = float.Parse(GetCPU_Speed(ParseToArray(chosenpart["feature_bullets"])).ToString());
                }
                catch
                {
                    tempCPU.MaxSpeed = null;
                }
                tempCPU.Wattage = null;
                tempCPU.Threads = null;
                try
                {
                    tempCPU.Speed = GetHardDrive_WriteSpeed(ParseToArray(chosenpart["feature_bullets"]));
                }
                catch
                {
                    tempCPU.Speed = null;
                }
                tempCPU.MaxRAM = null;
                tempCPU.Fan = null;
                tempCPU.Cores = null;
                tempCPU.Cache = null;
                tempCPU.Chipset = GetChipset(ParseToArray(chosenpart["feature_bullets"]));
                tempCPU.Socket = GetSocketType(ParseToArray(chosenpart["feature_bullets"]));

                ORM.CPUs.Add(tempCPU);
                ORM.SaveChanges();
            }
        }
        public static void SaveCPUsToDB()
        {
            List<JObject> searchedparts = new List<JObject>();
            searchedparts = GetPartData(GetParts("CPU"));
            Entities ORM = new Entities();

            foreach (JObject part in searchedparts)
            {
                string y = part["product_id"].ToString();
                CPU tempCPU = new CPU(part["title"].ToString());

                List<CPU> z = new List<CPU>();
                z = ORM.CPUs.Where(x => x.ProductID == y).ToList();

                if (z.Count < 1)
                {
                    tempCPU.ProductID = part["product_id"].ToString();
                    tempCPU.Description = "x";
                    tempCPU.Brand = part["brand"].ToString();
                    try
                    {
                        tempCPU.Price = int.Parse(part["price"].ToString());
                    }
                    catch
                    {
                        tempCPU.Price = null;
                    }
                    try
                    {
                        tempCPU.Stars = float.Parse(part["stars"].ToString());
                    }
                    catch
                    {
                        tempCPU.Stars = null;
                    }
                    tempCPU.ImageLink = part["main_image"].ToString();
                    tempCPU.Manufacturer = "x";
                    try
                    {
                        tempCPU.MaxSpeed = float.Parse(GetCPU_Speed(ParseToArray(part["feature_bullets"])).ToString());
                    }
                    catch
                    {
                        tempCPU.MaxSpeed = null;
                    }
                    tempCPU.Wattage = null;
                    tempCPU.Threads = null;
                    try
                    {
                        tempCPU.Speed = null;//GetCPU_Speed(ParseToArray(part["feature_bullets"]));
                    }
                    catch
                    {
                        tempCPU.Speed = null;
                    }
                    tempCPU.MaxRAM = null;
                    tempCPU.Fan = null;
                    tempCPU.Cores = null;
                    tempCPU.Cache = null;
                    tempCPU.Chipset = GetChipset(ParseToArray(part["feature_bullets"]));
                    tempCPU.Socket = GetSocketType(ParseToArray(part["feature_bullets"]));

                    ORM.CPUs.Add(tempCPU);
                    ORM.SaveChanges();
                }
            }
        }

        public static void SaveMotherBoardsToDB()
        {
            List<JObject> searchedparts = new List<JObject>();
            searchedparts = GetPartData(GetParts("Motherboard"));
            Entities ORM = new Entities();

            foreach (JObject chosenpart in searchedparts)
            {
                string y = chosenpart["product_id"].ToString();
                Motherboard tempObj = new Motherboard(chosenpart["title"].ToString());

                List<Motherboard> z = new List<Motherboard>();
                z = ORM.Motherboards.Where(x => x.ProductID == y).ToList();

                if (z.Count < 1)
                {
                    tempObj.ProductID = chosenpart["product_id"].ToString();
                    tempObj.Description = "x"; //chosenpart["product_description"].ToString();
                    tempObj.Brand = chosenpart["brand"].ToString();
                    try
                    {
                        tempObj.Price = int.Parse(chosenpart["price"].ToString());
                    }
                    catch
                    {
                        tempObj.Price = null;
                    }
                    tempObj.Stars = float.Parse(chosenpart["stars"].ToString());
                    tempObj.ImageLink = chosenpart["main_image"].ToString();
                    tempObj.Manufacturer = "x";
                    tempObj.Wattage = null;
                    tempObj.Socket = GetSocketType(ParseToArray(chosenpart["feature_bullets"]));
                    tempObj.SLILimit = null;
                    try
                    {
                        tempObj.SATASlots = GetSATA_Slots(ParseToArray(chosenpart["feature_bullets"]));
                    }
                    catch
                    {
                        tempObj.SATASlots = null;
                    }
                    tempObj.RAMType = GetRAMType(ParseToArray(chosenpart["feature_bullets"]));
                    tempObj.RAMSlots = GetRAMSlots(ParseToArray(chosenpart["feature_bullets"]));
                    tempObj.PCISlots = null; // GetPCI_Slots(ParseToArray(chosenpart["feature_bullets"]));
                    tempObj.FormFactor = GetFormFactor(ParseToArray(chosenpart["feature_bullets"]));
                    tempObj.CrossfireLimit = null;  //Crossfire_Limit(ParseToArray(chosenpart["feature_bullets"])); ;
                    tempObj.Chipset = GetChipset(ParseToArray(chosenpart["feature_bullets"]));

                    ORM.Motherboards.Add(tempObj);
                    ORM.SaveChanges();
                }
            }
        }

        public static void SaveMotherboardToDB(string partid)
        {
            JObject chosenpart = GetPartData(partid);
            Entities ORM = new Entities();

            Motherboard tempObj = new Motherboard(chosenpart["title"].ToString());

            List<Motherboard> z = new List<Motherboard>();
            z = ORM.Motherboards.Where(x => x.ProductID == partid).ToList();

            if (z.Count < 1)
            {
                tempObj.ProductID = chosenpart["product_id"].ToString();
                tempObj.Description = "x"; //chosenpart["product_description"].ToString();
                tempObj.Brand = chosenpart["brand"].ToString();
                tempObj.Price = int.Parse(chosenpart["price"].ToString());
                tempObj.Stars = float.Parse(chosenpart["stars"].ToString());
                tempObj.ImageLink = chosenpart["main_image"].ToString();
                tempObj.Manufacturer = "x";
                tempObj.Wattage = null;
                tempObj.Socket = GetSocketType(ParseToArray(chosenpart["feature_bullets"]));
                tempObj.SLILimit = null; // (ParseToArray(chosenpart["feature_bullets"]));
                try
                {
                    tempObj.SATASlots = GetSATA_Slots(ParseToArray(chosenpart["feature_bullets"]));
                }
                catch
                {
                    tempObj.SATASlots = null;
                }
                tempObj.RAMType = GetRAMType(ParseToArray(chosenpart["feature_bullets"]));
                tempObj.RAMSlots = GetRAMSlots(ParseToArray(chosenpart["feature_bullets"]));
                tempObj.PCISlots = GetPCI_Slots(ParseToArray(chosenpart["feature_bullets"]));
                tempObj.FormFactor = GetFormFactor(ParseToArray(chosenpart["feature_bullets"]));
                tempObj.CrossfireLimit = null; // (ParseToArray(chosenpart["feature_bullets"]));
                tempObj.Chipset = GetChipset(ParseToArray(chosenpart["feature_bullets"]));

                ORM.Motherboards.Add(tempObj);
                ORM.SaveChanges();
            }
        }      

        public static void SavePSUsToDB()
        {
            List<JObject> searchedparts = new List<JObject>();
            searchedparts = GetPartData(GetParts("PSU"));
            Entities ORM = new Entities();

            foreach (JObject part in searchedparts)
            {
                string y = part["product_id"].ToString();
                PSU tempPSU = new PSU(part["title"].ToString());

                List<PSU> z = new List<PSU>();
                z = ORM.PSUs.Where(x => x.ProductID == y).ToList();

                if (z.Count < 1)
                {
                    tempPSU.ProductID = part["product_id"].ToString();
                    tempPSU.Description = "x";//part["product_description"].ToString();
                    tempPSU.Brand = part["brand"].ToString();
                    tempPSU.Price = int.Parse(part["price"].ToString());
                    tempPSU.Stars = float.Parse(part["stars"].ToString());
                    tempPSU.ImageLink = part["main_image"].ToString();
                    tempPSU.Manufacturer = "x";
                    tempPSU.Width = null;
                    tempPSU.Wattage = null;
                    tempPSU.Length = null;
                    tempPSU.Height = null;
                    tempPSU.FormFactor = GetFormFactor(ParseToArray(part["feature_bullets"]));

                    ORM.PSUs.Add(tempPSU);
                    ORM.SaveChanges();
                }
            }
        }

        public static void SavePSUToDB(string partid)
        {
            JObject chosenpart = GetPartData(partid);
            Entities ORM = new Entities();

            PSU tempObj = new PSU(chosenpart["title"].ToString());

            List<PSU> z = new List<PSU>();
            z = ORM.PSUs.Where(x => x.ProductID == partid).ToList();

            if (z.Count < 1)
            {
                tempObj.ProductID = chosenpart["product_id"].ToString();
                tempObj.Description = "x";
                tempObj.Brand = chosenpart["brand"].ToString();
                tempObj.Price = int.Parse(chosenpart["price"].ToString());
                tempObj.Stars = float.Parse(chosenpart["stars"].ToString());
                tempObj.ImageLink = chosenpart["main_image"].ToString();
                tempObj.Manufacturer = "x";
                tempObj.Width = null;
                tempObj.Wattage = null;
                tempObj.Length = null;
                tempObj.Height = null;
                tempObj.FormFactor = GetFormFactor(ParseToArray(chosenpart["feature_bullets"]));

                ORM.PSUs.Add(tempObj);
                ORM.SaveChanges();
            }
        }

        public static void SavePCCasesToDB()
        {
            List<JObject> searchedparts = new List<JObject>();
            searchedparts = GetPartData(GetParts("Computer+Case"));
            Entities ORM = new Entities();

            foreach (JObject part in searchedparts)
            {
                string y = part["product_id"].ToString();
                PCCase tempPCCase = new PCCase(part["title"].ToString());

                List<PCCase> z = new List<PCCase>();
                z = ORM.PCCases.Where(x => x.ProductID == y).ToList();

                if (z.Count < 1)
                {
                    tempPCCase.ProductID = part["product_id"].ToString();
                    tempPCCase.Description = "x";//part["product_description"].ToString();
                    tempPCCase.Brand = part["brand"].ToString();
                    tempPCCase.Price = int.Parse(part["price"].ToString());
                    try { tempPCCase.Stars = float.Parse(part["stars"].ToString()); }
                    catch { tempPCCase.Stars = null; }
                    tempPCCase.ImageLink = part["main_image"].ToString();
                    tempPCCase.Manufacturer = "x";
                    tempPCCase.Width = null;
                    tempPCCase.TwoSlots = null;
                    tempPCCase.ThreeSlots = null;
                    tempPCCase.Style = null;
                    tempPCCase.Length = null;
                    tempPCCase.Height = null;
                    tempPCCase.ExpansionSlots = null;

                    ORM.PCCases.Add(tempPCCase);
                    ORM.SaveChanges();
                }
            }
        }

        public static void SavePCCaseToDB(string partid)
        {
            JObject chosenpart = GetPartData(partid);
            Entities ORM = new Entities();

            PCCase tempObj = new PCCase(chosenpart["title"].ToString());

            List<PCCase> z = new List<PCCase>();
            z = ORM.PCCases.Where(x => x.ProductID == partid).ToList();

            if (z.Count < 1)
            {
                tempObj.ProductID = chosenpart["product_id"].ToString();
                tempObj.Description = "x";
                tempObj.Brand = chosenpart["brand"].ToString();
                tempObj.Price = int.Parse(chosenpart["price"].ToString());
                try { tempObj.Stars = float.Parse(chosenpart["stars"].ToString()); }
                catch { tempObj.Stars = null;  }
                tempObj.Stars = float.Parse(chosenpart["stars"].ToString());
                tempObj.ImageLink = chosenpart["main_image"].ToString();
                tempObj.Manufacturer = "x";
                tempObj.Width = null;
                tempObj.TwoSlots = null;
                tempObj.ThreeSlots = null;
                tempObj.Style = null;
                tempObj.Length = null;
                tempObj.Height = null;
                tempObj.ExpansionSlots = null;

                ORM.PCCases.Add(tempObj);
                ORM.SaveChanges();
            }
        }

        public static void SaveRAMsToDB()
        {
            List<JObject> searchedparts = new List<JObject>();
            searchedparts = GetPartData(GetParts("RAM"));
            Entities ORM = new Entities();

            foreach (JObject part in searchedparts)
            {
                string y = part["product_id"].ToString();
                RAM tempRAM = new RAM(part["title"].ToString());

                List<RAM> z = new List<RAM>();
                z = ORM.RAMs.Where(x => x.ProductID == y).ToList();

                if (z.Count < 1)
                {
                    tempRAM.ProductID = part["product_id"].ToString();
                    tempRAM.Description = "x";//part["product_description"].ToString();
                    tempRAM.Brand = part["brand"].ToString();
                    tempRAM.Price = int.Parse(part["price"].ToString());
                    tempRAM.Stars = float.Parse(part["stars"].ToString());
                    tempRAM.ImageLink = part["main_image"].ToString();
                    tempRAM.Manufacturer = "x";
                    tempRAM.BusSpeed = null;
                    tempRAM.Quantity = null;
                    try { tempRAM.RAMType = GetRAMType(ParseToArray(part["feature_bullets"])); }
                    catch{ tempRAM.RAMType = null; }
                    tempRAM.TotalCapacity = null;
                    tempRAM.Voltage = null;

                    ORM.RAMs.Add(tempRAM);
                    ORM.SaveChanges();
                }
            }
        }
        public static void SaveRAMToDB(string partid)
        {
            JObject chosenpart = GetPartData(partid);
            Entities ORM = new Entities();

            RAM tempObj = new RAM(chosenpart["title"].ToString());

            List<RAM> z = new List<RAM>();
            z = ORM.RAMs.Where(x => x.ProductID == partid).ToList();

            if (z.Count < 1)
            {
                tempObj.ProductID = chosenpart["product_id"].ToString();
                tempObj.Description = "x";
                tempObj.Brand = chosenpart["brand"].ToString();
                tempObj.Price = int.Parse(chosenpart["price"].ToString());
                tempObj.Stars = float.Parse(chosenpart["stars"].ToString());
                tempObj.ImageLink = chosenpart["main_image"].ToString();
                tempObj.Manufacturer = "x";
                tempObj.BusSpeed = null;
                tempObj.Quantity = null;
                try { tempObj.RAMType = GetRAMType(ParseToArray(chosenpart["feature_bullets"])); }
                catch { tempObj.RAMType = null; }
                tempObj.TotalCapacity = null;
                tempObj.Voltage = null;

                ORM.RAMs.Add(tempObj);
                ORM.SaveChanges();
            }
        }

        public static void SaveMonitorsToDB()
        {
            List<JObject> searchedparts = new List<JObject>();
            searchedparts = GetPartData(GetParts("CPU+Monitor"));
            Entities ORM = new Entities();

            foreach (JObject part in searchedparts)
            {
                string y = part["product_id"].ToString();
                Monitor tempObj = new Monitor(part["title"].ToString());

                List<Monitor> z = new List<Monitor>();
                z = ORM.Monitors.Where(x => x.ProductID == y).ToList();
                if (z.Count < 1)
                {
                    tempObj.ProductID = part["product_id"].ToString();
                    tempObj.Description = "x";//part["product_description"].ToString();
                    tempObj.Brand = part["brand"].ToString();
                    tempObj.Price = int.Parse(part["price"].ToString());
                    tempObj.Stars = float.Parse(part["stars"].ToString());
                    tempObj.ImageLink = part["main_image"].ToString();
                    tempObj.Manufacturer = "x";
                    tempObj.RefreshRate = null;
                    int[] res = GetMaxScreenResolution(ParseToArray(part["feature_bullets"]));
                    tempObj.ResX = res[0];
                    tempObj.ResY = res[1];
                    
                    ORM.Monitors.Add(tempObj);
                    ORM.SaveChanges();
                }
            }
        }
        public static void SaveMonitorToDB(string partid)
        {
            JObject chosenpart = GetPartData(partid);
            Entities ORM = new Entities();

            Monitor tempObj = new Monitor(chosenpart["title"].ToString());

            List<Monitor> z = new List<Monitor>();
            z = ORM.Monitors.Where(x => x.ProductID == partid).ToList();

            if (z.Count < 1)
            {
                tempObj.ProductID = chosenpart["product_id"].ToString();
                tempObj.Description = "x";
                tempObj.Brand = chosenpart["brand"].ToString();
                tempObj.Price = int.Parse(chosenpart["price"].ToString());
                tempObj.Stars = float.Parse(chosenpart["stars"].ToString());
                tempObj.ImageLink = chosenpart["main_image"].ToString();
                tempObj.Manufacturer = "x";
                tempObj.RefreshRate = null;
                int[] res = GetMaxScreenResolution(ParseToArray(chosenpart["feature_bullets"]));
                tempObj.ResX = res[0];
                tempObj.ResY = res[1];

                ORM.Monitors.Add(tempObj);
                ORM.SaveChanges();
            }
        }

        public static void SaveHardDrivesToDB()
        {
            List<JObject> searchedparts = new List<JObject>();
            searchedparts = GetPartData(GetParts("Internal+Hard+Drive"));
            Entities ORM = new Entities();

            foreach (JObject part in searchedparts)
            {
                string y = part["product_id"].ToString();
                HardDrive tempObj = new HardDrive(part["title"].ToString());

                List<HardDrive> z = new List<HardDrive>();
                z = ORM.HardDrives.Where(x => x.ProductID == y).ToList();
                if (z.Count < 1)
                {
                    //tempObj.HardDriveID = Guid.NewGuid().ToString("D");
                    tempObj.ProductID = part["product_id"].ToString();
                    tempObj.Description = "x";//part["product_description"].ToString();
                    tempObj.Brand = part["brand"].ToString();
                    tempObj.Price = int.Parse(part["price"].ToString());
                    tempObj.Stars = float.Parse(part["stars"].ToString());
                    tempObj.ImageLink = part["main_image"].ToString();
                    tempObj.Manufacturer = "x";
                    tempObj.BuildDisks = null;
                    tempObj.Capacity = null; //GetHardDrive_ReadSpeed(ParseToArray(part["feature_bullets"]));
                    tempObj.CapacityUnits = null;
                    tempObj.Interface = null;
                    tempObj.SlotSize = false;//null;


                    ORM.HardDrives.Add(tempObj);
                    ORM.SaveChanges();
                }
            }
        }
        public static void SaveHardDriveToDB(string partid)
        {
            JObject chosenpart = GetPartData(partid);
            Entities ORM = new Entities();

            HardDrive tempObj = new HardDrive(chosenpart["title"].ToString());

            List<HardDrive> z = new List<HardDrive>();
            z = ORM.HardDrives.Where(x => x.ProductID == partid).ToList();

            if (z.Count < 1)
            {
                tempObj.ProductID = chosenpart["product_id"].ToString();
                tempObj.Description = "x";
                tempObj.Brand = chosenpart["brand"].ToString();
                tempObj.Price = int.Parse(chosenpart["price"].ToString());
                tempObj.Stars = float.Parse(chosenpart["stars"].ToString());
                tempObj.ImageLink = chosenpart["main_image"].ToString();
                tempObj.Manufacturer = "x";
                tempObj.Capacity = GetHardDrive_ReadSpeed(ParseToArray(chosenpart["feature_bullets"]));
                tempObj.CapacityUnits = null;
                tempObj.Interface = null;
                tempObj.SlotSize = false;//null;

                ORM.HardDrives.Add(tempObj);
                ORM.SaveChanges();
            }
        }

        public static void SaveOpticalDriversToDB()
        {
            List<JObject> searchedparts = new List<JObject>();
            searchedparts = GetPartData(GetParts("Optical+Driver"));
            Entities ORM = new Entities();

            foreach (JObject part in searchedparts)
            {
                string y = part["product_id"].ToString();
                OpticalDriver tempObj = new OpticalDriver(part["title"].ToString());

                List<OpticalDriver> z = new List<OpticalDriver>();
                z = ORM.OpticalDrivers.Where(x => x.ProductID == y).ToList();
                if (z.Count < 1)
                {
                    tempObj.ProductID = part["product_id"].ToString();
                    tempObj.Description = "x";//part["product_description"].ToString();
                    tempObj.Brand = part["brand"].ToString();
                    tempObj.Price = int.Parse(part["price"].ToString());
                    tempObj.Stars = float.Parse(part["stars"].ToString());
                    tempObj.ImageLink = part["main_image"].ToString();
                    tempObj.Manufacturer = "x";
                    tempObj.WriteSpeed = null;  //GetOpticalDrive_WriteSpeed(ParseToArray(part["feature_bullets"]));// null;
                    tempObj.Wattage = 0;// null;
                    tempObj.Type = GetOpticalDrive_Types(ParseToArray(part["feature_bullets"]));
                    tempObj.Rewritable = false; //null;
                    tempObj.ReadSpeed = null;  //GetOpticalDrive_ReadSpead(ParseToArray(part["feature_bullets"]));
                    tempObj.Interface = null;


                    ORM.OpticalDrivers.Add(tempObj);
                    ORM.SaveChanges();
                }
            }
        }
        public static void SaveOpticalDriverToDB(string partid)
        {
            JObject chosenpart = GetPartData(partid);
            Entities ORM = new Entities();

            OpticalDriver tempObj = new OpticalDriver(chosenpart["title"].ToString());

            List<OpticalDriver> z = new List<OpticalDriver>();
            z = ORM.OpticalDrivers.Where(x => x.ProductID == partid).ToList();

            if (z.Count < 1)
            {
                tempObj.ProductID = chosenpart["product_id"].ToString();
                tempObj.Description = "x";
                tempObj.Brand = chosenpart["brand"].ToString();
                tempObj.Price = int.Parse(chosenpart["price"].ToString());
                tempObj.Stars = float.Parse(chosenpart["stars"].ToString());
                tempObj.ImageLink = chosenpart["main_image"].ToString();
                tempObj.Manufacturer = "x";
                tempObj.WriteSpeed = null; // GetOpticalDrive_WriteSpeed(ParseToArray(chosenpart["feature_bullets"]));// null;
                tempObj.Wattage = 0;// null;
                tempObj.Type = null; // GetOpticalDrive_Types(ParseToArray(chosenpart["feature_bullets"]));
                tempObj.Rewritable = false; //null;
                tempObj.ReadSpeed = GetOpticalDrive_ReadSpead(ParseToArray(chosenpart["feature_bullets"]));
                tempObj.Interface = null;

                ORM.OpticalDrivers.Add(tempObj);
                ORM.SaveChanges();
            }
        }

        public static void SavePCICardsToDB()
        {
            List<JObject> searchedparts = new List<JObject>();
            searchedparts = GetPartData(GetParts("PCI+Card"));
            Entities ORM = new Entities();

            foreach (JObject part in searchedparts)
            {
                string y = part["product_id"].ToString();
                PCICard tempObj = new PCICard(part["title"].ToString());

                List<PCICard> z = new List<PCICard>();
                z = ORM.PCICards.Where(x => x.ProductID == y).ToList();
                if (z.Count < 1)
                {
                    tempObj.ProductID = part["product_id"].ToString();
                    tempObj.Description = "x";//part["product_description"].ToString();
                    tempObj.Brand = part["brand"].ToString();
                    tempObj.Price = int.Parse(part["price"].ToString());
                    tempObj.Stars = float.Parse(part["stars"].ToString());
                    tempObj.ImageLink = part["main_image"].ToString();
                    tempObj.Manufacturer = "x";


                    ORM.PCICards.Add(tempObj);
                    ORM.SaveChanges();
                }
            }
        }
        public static void SavePCICardToDB(string partid)
        {
            JObject chosenpart = GetPartData(partid);
            Entities ORM = new Entities();

            PCICard tempObj = new PCICard(chosenpart["title"].ToString());

            List<PCICard> z = new List<PCICard>();
            z = ORM.PCICards.Where(x => x.ProductID == partid).ToList();

            if (z.Count < 1)
            {
                tempObj.ProductID = chosenpart["product_id"].ToString();
                tempObj.Description = "x";
                tempObj.Brand = chosenpart["brand"].ToString();
                tempObj.Price = int.Parse(chosenpart["price"].ToString());
                tempObj.Stars = float.Parse(chosenpart["stars"].ToString());
                tempObj.ImageLink = chosenpart["main_image"].ToString();
                tempObj.Manufacturer = "x";

                ORM.PCICards.Add(tempObj);
                ORM.SaveChanges();
            }
        }

        public static string[] ParseToArray(JToken specs)
        {
            int x = 0;
            foreach (JToken y in specs)
            {
                x = x + 1;
            }

            string[] specarray = new string[x];

            foreach (JToken z in specs)
            {
                x = x - 1;
                specarray[x] = z.ToString();
            }
            return specarray;
        }
    }
}

