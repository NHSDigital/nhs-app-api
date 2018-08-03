using System.Collections.Generic;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.CID.Linkage
{
    public static class CreateLinkageData
    {
        public static LinkageDataItem ValidPatient
        {
            get {
                return new LinkageDataItem() {
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
        
        public static LinkageDataItem NotFoundPatient
        {
            get {
                return new LinkageDataItem {
                    NhsNumber = "4447770001",
                    LinkageResponse = new LinkageResponse
                    {
                        OdsCode = "A29928"
                    }
                };
            }
        }
        
        public static LinkageDataItem ConflictPatient 
        {
            get {
                return new LinkageDataItem
                {
                    NhsNumber = "5558881112",
                    LinkageResponse = new LinkageResponse
                    {
                        OdsCode = "A29928"
                    }
                };
            }
        }
        
        public static LinkageDataItem BadGatewayPatient 
        {
            get {
                return new LinkageDataItem
                {
                    NhsNumber = "5634234345",
                    LinkageResponse = new LinkageResponse
                    {
                        OdsCode = "A29928"
                    }
                };
            }
        }
        
        public static LinkageDataItem TimeoutPatient 
        {
            get {
                return new LinkageDataItem
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

    public static class GetLinkageData
    {
        public static readonly List<LinkageDataItem> ExistingPatientLinkage = new List<LinkageDataItem>
        {
            new LinkageDataItem { NhsNumber = "3434234345",
                LinkageResponse = new LinkageResponse
                    { AccountId = "542343", LinkageKey = "tTALtBP3rLR16", OdsCode = "A29928" }},
            
            new LinkageDataItem { NhsNumber = "5454253356",
                LinkageResponse = new LinkageResponse
                    { AccountId = "897348", LinkageKey = "vVGO8bgV6fvPb", OdsCode = "A29928" }}            
        };
        
        public static readonly List<LinkageDataItem> ExpiredPatientLinkage = new List<LinkageDataItem>
        {
            new LinkageDataItem { NhsNumber = "4642234432",
                LinkageResponse = new LinkageResponse
                    { AccountId = "643243", LinkageKey = "Bmij89KnhY8Jp", OdsCode = "A29928" }},
            
            new LinkageDataItem { NhsNumber = "6423432552",
                LinkageResponse = new LinkageResponse
                    { AccountId = "343555", LinkageKey = "NAw3hSsw87hu2", OdsCode = "A29928" }}            
        };

        public static readonly LinkageDataItem NotFoundPatient = new LinkageDataItem
        {
            NhsNumber = "3434994345",
            LinkageResponse = new LinkageResponse
            {
                OdsCode = "A29928"
            }
        };
        
        public static readonly LinkageDataItem BadGatewayPatient = new LinkageDataItem
        {
            NhsNumber = "5634234345",
            LinkageResponse = new LinkageResponse
            {
                OdsCode = "A29928"
            }
        };
        
        public static readonly LinkageDataItem TimeOutPatient = new LinkageDataItem
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
    
    public class LinkageDataItem
    {
        public string NhsNumber { get; set; }
        public LinkageResponse LinkageResponse { get; set; }    
    }
}