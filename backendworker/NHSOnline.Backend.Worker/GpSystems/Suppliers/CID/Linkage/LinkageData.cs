using System.Collections.Generic;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.CID.Linkage
{
    public class CreateLinkageData
    {
        public static Linkage ValidPatient
        {
            get {
                return new Linkage() {
                    NhsNumber = "3336669990",
                    LinkageResponse = new LinkageResponse
                    {
                        OdsCode = "A29928",
                        AccountId = "675234",
                        LinkageKey = "vVGO567gV6fvPb"
                    }  
                };
            }
        }
        
        public static Linkage NotFoundPatient
        {
            get {
                return new Linkage {
                    NhsNumber = "4447770001",
                    LinkageResponse = new LinkageResponse
                    {
                        OdsCode = "A29928"
                    }
                };
            }
        }
        
        public static Linkage ConflictPatient 
        {
            get {
                return new Linkage
                {
                    NhsNumber = "5558881112",
                    LinkageResponse = new LinkageResponse
                    {
                        OdsCode = "A29928"
                    }
                };
            }
        }
        
        public static Linkage BadGatewayPatient 
        {
            get {
                return new Linkage
                {
                    NhsNumber = "5634234345",
                    LinkageResponse = new LinkageResponse
                    {
                        OdsCode = "A29928"
                    }
                };
            }
        }
        
        public static Linkage TimeoutPatient 
        {
            get {
                return new Linkage
                {
                    NhsNumber = "5634200045",
                    LinkageResponse = new LinkageResponse
                    {
                        OdsCode = "A29928",
                        AccountId = "542343",
                        LinkageKey = "tTALtBP3rLR16"
                    }
                };
            }
        }
    }

    public class GetLinkageData
    {
        public static readonly List<Linkage> ExistingPatientLinkage = new List<Linkage>
        {
            new Linkage { NhsNumber = "3434234345",
                LinkageResponse = new LinkageResponse
                    { AccountId = "542343", LinkageKey = "tTALtBP3rLR16", OdsCode = "A29928" }},
            
            new Linkage { NhsNumber = "5454253356",
                LinkageResponse = new LinkageResponse
                    { AccountId = "897348", LinkageKey = "vVGO8bgV6fvPb", OdsCode = "A29928" }}            
        };
        
        public static readonly List<Linkage> ExpiredPatientLinkage = new List<Linkage>
        {
            new Linkage { NhsNumber = "4642234432",
                LinkageResponse = new LinkageResponse
                    { AccountId = "643243", LinkageKey = "Bmij89KnhY8Jp", OdsCode = "A29928" }},
            
            new Linkage { NhsNumber = "6423432552",
                LinkageResponse = new LinkageResponse
                    { AccountId = "343555", LinkageKey = "NAw3hSsw87hu2", OdsCode = "A29928" }}            
        };

        public static readonly Linkage NotFoundPatient = new Linkage
        {
            NhsNumber = "3434994345",
            LinkageResponse = new LinkageResponse
            {
                OdsCode = "A29928"
            }
        };
        
        public static readonly Linkage BadGatewayPatient = new Linkage
        {
            NhsNumber = "5634234345",
            LinkageResponse = new LinkageResponse
            {
                OdsCode = "A29928"
            }
        };
        
        public static readonly Linkage TimeOutPatient = new Linkage
        {
            NhsNumber = "5634200045",
            LinkageResponse = new LinkageResponse
            {
                OdsCode = "A29928",
                AccountId = "542343",
                LinkageKey = "tTALtBP3rLR16"
            }
        };
    }
    
    public class Linkage
    {
        public string NhsNumber { get; set; }
        public LinkageResponse LinkageResponse { get; set; }    
    }
}