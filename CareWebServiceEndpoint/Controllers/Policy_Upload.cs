using CareWebServiceEndpoint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace CareWebServiceEndpoint.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class Policy_Upload : Controller
    {
        private readonly SEAWEBContext _seaWebContext;
        private readonly ARTALEARNContext _artaLearnContext;

        public Policy_Upload(SEAWEBContext seaWebContext, ARTALEARNContext artaLearnContext)
        {
            _seaWebContext = seaWebContext;
            _artaLearnContext = artaLearnContext;
        }

        [HttpPost("/UP00000001-Upload")]
        public async Task<string> UploadUP01Data([FromBody] List<UP00000001Model> UP01Data)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(150);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://172.20.12.55/CareWebServiceV5/WSEUploader.asmx?op=Upload_Excel");
                request.Content = new StringContent(ConvertJsonToXML("UP00000001", UP01Data).ToString(), System.Text.Encoding.UTF8, "application/soap+xml");

                var response = await client.SendAsync(request);

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("/BE00000005-Upload")]
        public async Task<string> UploadBE05Data([FromBody] List<BE00000005Model> BE05Data)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(150);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://172.20.12.55/CareWebServiceV5/WSEUploader.asmx?op=Upload_Excel");
                request.Content = new StringContent(ConvertJsonToXML("BE00000005", BE05Data).ToString(), System.Text.Encoding.UTF8, "application/soap+xml");

                var response = await client.SendAsync(request);

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("/Check-Policy-Upload")]
        public async Task<List<UPDataModel>> UploadCheck(string batchNo)
        {
            try
            {
                List<UPDataModel> upData = new();

                var uploadedData = await _seaWebContext.CatalogSysBatchOriginalUP.AsNoTracking().Where(x => x.BatchNo == batchNo.Trim()).ToListAsync();

                foreach (var item in uploadedData)
                {
                    upData.Add(new UPDataModel
                    {
                        BatchNo = item.BatchNo,
                        ErrMsg = item.ErrMsg,
                        Status = item.Status,
                        RefNO = item.RefNO
                    });
                }

                await _artaLearnContext.CatalogUPData.AddRangeAsync(upData);

                await _artaLearnContext.SaveChangesAsync();

                return await _artaLearnContext.CatalogUPData.AsNoTracking().Where(x => x.BatchNo == batchNo.Trim()).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected XElement ConvertJsonToXML<T>(string uploadOption, List<T> UploadedData)
        {
            string json = JsonConvert.SerializeObject(UploadedData);

            //Console.WriteLine(json);

            string wrappedDoc = string.Format("{{ Table: {0} }}", json);

            XmlDocument? node = JsonConvert.DeserializeXmlNode(wrappedDoc, "Dataset");

            XNamespace soap = "http://www.w3.org/2003/05/soap-envelope";
            XNamespace tem = "http://tempuri.org/";
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace diffgr = "urn:schemas-microsoft-com:xml-diffgram-v1";

            // Create the SOAP Header (empty in this case)
            XElement header = new(soap + "Header");

            // Create the SOAP Body with the Upload_Excel operation
            XElement body = new XElement(soap + "Body",
                new XElement(tem + "Upload_Excel",
                    new XElement(tem + "dbuser", "care"),
                    new XElement(tem + "dbpassword", ""),
                    new XElement(tem + "inparam",
                        new XElement(tem + "anyType",
                            new XAttribute(xsi + "type", "xs:string"),
                            uploadOption
                        ),
                        new XElement(tem + "anyType",
                            new XAttribute(xsi + "type", "xs:string"),
                            uploadOption.Substring(0, 2)
                        ),
                        new XElement(tem + "anyType",
                            new XAttribute(xsi + "type", "xs:string")
                        ),
                        new XElement(tem + "anyType",
                            new XAttribute(xsi + "type", "xs:boolean"),
                            "false"
                        ),
                        new XElement(tem + "anyType",
                            new XAttribute(xsi + "type", "xs:string")
                        ),
                        new XElement(tem + "anyType",
                            new XAttribute(xsi + "type", "xs:boolean"),
                            "false"
                        )
                    ),
                    new XElement(tem + "dataset",
                        new XElement(xs + "schema",
                            new XElement(xs + "element",
                                new XAttribute("name", "Dataset"),
                                new XElement(xs + "complexType",
                                    new XElement(xs + "sequence",
                                        new XElement(xs + "element",
                                            new XAttribute("name", "Table"),
                                            new XAttribute("maxOccurs", "unbounded"),
                                            new XElement(xs + "complexType",
                                                new XElement(xs + "sequence",
                                                    UploadedData.SelectMany(data => data.GetType().GetProperties()
                                                        .Where(type => type.PropertyType == typeof(string))
                                                        .Select(pName => pName.Name))
                                                        .Distinct()
                                                        .Select(uniqueName => new XElement(xs + "element",
                                                            new XAttribute("name", uniqueName),
                                                            new XAttribute("type", "xs:string"),
                                                            new XAttribute("minOccurs", "0")
                                                        ))
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        ),
                        new XElement(diffgr + "diffgram",
                            new XAttribute(XNamespace.Xmlns + "diffgr", "urn:schemas-microsoft-com:xml-diffgram-v1"),
                            new XAttribute(XNamespace.Xmlns + "msdata", "urn:schemas-microsoft-com:xml-msdata"),
                            new XElement(XElement.Parse(node.OuterXml))
                        )
                    ),
                    new XElement(tem + "outparam"),
                    new XElement(tem + "ErrMsg")
                )
            );

            // Create the SOAP Envelope
            XElement envelope = new XElement(soap + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soap", soap),
                new XAttribute(XNamespace.Xmlns + "tem", tem),
                new XAttribute(XNamespace.Xmlns + "xs", xs),
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                header,
                body
            );

            // Output the XML
            //Console.WriteLine(envelope);

            return envelope;

            //Console.WriteLine(node);

            //for (int i = 0; i < 100; i++)
            //{
            //    UP01Data.Add(new UP00000001Model
            //    {
            //        HOLDERID = "D01WI01368",
            //        INSUREDID = "D01WI01368",
            //        HOLDERNAME = "BAHTERA WAHANA TRITATA",
            //        INSUREDNAME = "wilson",
            //        ADDRESS1 = "01",
            //        ADDRESS2 = "",
            //        ADDRESS3 = "",
            //        CITY = "",
            //        BIRTHDATE = "",
            //        EMAIL = "",
            //        PHONE = "",
            //        MOBILE = "",
            //        ID_NO = "",
            //        TAXID = "",
            //        ZIPCODE = "",
            //        CORRESPONDENCE_ADDRESS = "",
            //        CORRESPONDENCE_EMAIL = "",
            //        CORRESPONDENCE_PHONE = "",
            //        BRANCH = "01",
            //        MO = "johana.e",
            //        SEGMENT = "BROKER",
            //        STYPE = "S",
            //        INSURANCE_TYPE = "I",
            //        TOC = "1216",
            //        TOPRO = "",
            //        TOPRO_CURRENCY = "",
            //        LONG_INSURED_NAME = "wilson",
            //        REFNO = "ShpBwt-23052024-0038757263",
            //        STARTDATE = "2024-05-23T12:00:00",
            //        ENDDATE = "2024-11-23T12:00:00",
            //        TOPRO_TSI = "",
            //        TOPRO_ATSI = "",
            //        AUTOCALCADDITIONF = "",
            //        MASTERF = "",
            //        AUTOINFORCE = "",
            //        POLICYNO = "",
            //        REGNO = "",
            //        CONJ_POLICYNO = "",
            //        CONJ_CERTIFICATENO = "",
            //        REMARKS = "",
            //        MEMO = "",
            //        PAYORID = "D01BW00076",
            //        GRACEPERIOD = "30",
            //        DISCOUNT = "",
            //        INSTALLMENT_DURATION = "",
            //        INSTALLMENT_FIRSTDUE = "",
            //        INSTALLMENT_FREQ = "",
            //        OBJ_INFO_01 = "1904733808574454792_1609966925784304640",
            //        OBJ_INFO_02 = "",
            //        OBJ_INFO_03 = "Mouse Logitech M331 Silent Plus OriginaL 100%",
            //        OBJ_INFO_04 = "Damage Protection+",
            //        OBJ_INFO_05 = "Anywhere within Republic of Indonesia",
            //        OBJ_INFO_06 = "Anywhere within Republic of Indonesia",
            //        OBJ_INFO_07 = "Segala resiko sebagaimana diatur dalam polis schedule",
            //        OBJ_INFO_08 = "",
            //        OBJ_INFO_09 = "",
            //        OBJ_INFO_10 = "",
            //        OBJ_INFO_11 = "",
            //        OBJ_INFO_12 = "",
            //        OBJ_INFO_13 = "",
            //        OBJ_INFO_14 = "",
            //        OBJ_INFO_15 = "",
            //        DESCRIPTION1 = "",
            //        DESCRIPTION2 = "",
            //        DESCRIPTION3 = "",
            //        DESCRIPTION4 = "",
            //        DESCRIPTION5 = "",
            //        DESCRIPTION6 = "",
            //        DESCRIPTION7 = "",
            //        DESCRIPTION8 = "",
            //        DESCRIPTION9 = "",
            //        DESCRIPTION10 = "",
            //        DESCRIPTION11 = "",
            //        DESCRIPTION12 = "",
            //        DESCRIPTION13 = "",
            //        DESCRIPTION14 = "",
            //        DESCRIPTION15 = "",
            //        COVERAGE1 = "MPAR-04",
            //        COVERAGE2 = "",
            //        COVERAGE3 = "",
            //        COVERAGE4 = "",
            //        COVERAGE5 = "",
            //        COVERAGE6 = "",
            //        COVERAGE7 = "",
            //        COVERAGE8 = "",
            //        COVERAGE9 = "",
            //        COVERAGE10 = "",
            //        COVERAGE11 = "",
            //        COVERAGE12 = "",
            //        COVERAGE13 = "",
            //        COVERAGE14 = "",
            //        COVERAGE15 = "",
            //        COVERAGEREMARK1 = "Damage Protection+",
            //        COVERAGEREMARK2 = "",
            //        COVERAGEREMARK3 = "",
            //        COVERAGEREMARK4 = "",
            //        COVERAGEREMARK5 = "",
            //        COVERAGEREMARK6 = "",
            //        COVERAGEREMARK7 = "",
            //        COVERAGEREMARK8 = "",
            //        COVERAGEREMARK9 = "",
            //        COVERAGEREMARK10 = "",
            //        COVERAGEREMARK11 = "",
            //        COVERAGEREMARK12 = "",
            //        COVERAGEREMARK13 = "",
            //        COVERAGEREMARK14 = "",
            //        COVERAGEREMARK15 = "",
            //        COVERAGERATE1 = "688",
            //        COVERAGERATE2 = "",
            //        COVERAGERATE3 = "",
            //        COVERAGERATE4 = "",
            //        COVERAGERATE5 = "",
            //        COVERAGERATE6 = "",
            //        COVERAGERATE7 = "",
            //        COVERAGERATE8 = "",
            //        COVERAGERATE9 = "",
            //        COVERAGERATE10 = "",
            //        COVERAGERATE11 = "",
            //        COVERAGERATE12 = "",
            //        COVERAGERATE13 = "",
            //        COVERAGERATE14 = "",
            //        COVERAGERATE15 = "",
            //        COVERAGEUNIT1 = "F",
            //        COVERAGEUNIT2 = "",
            //        COVERAGEUNIT3 = "",
            //        COVERAGEUNIT4 = "",
            //        COVERAGEUNIT5 = "",
            //        COVERAGEUNIT6 = "",
            //        COVERAGEUNIT7 = "",
            //        COVERAGEUNIT8 = "",
            //        COVERAGEUNIT9 = "",
            //        COVERAGEUNIT10 = "",
            //        COVERAGEUNIT11 = "",
            //        COVERAGEUNIT12 = "",
            //        COVERAGEUNIT13 = "",
            //        COVERAGEUNIT14 = "",
            //        COVERAGEUNIT15 = "",
            //        COVERAGESDATE1 = "",
            //        COVERAGESDATE2 = "",
            //        COVERAGESDATE3 = "",
            //        COVERAGESDATE4 = "",
            //        COVERAGESDATE5 = "",
            //        COVERAGESDATE6 = "",
            //        COVERAGESDATE7 = "",
            //        COVERAGESDATE8 = "",
            //        COVERAGESDATE9 = "",
            //        COVERAGESDATE10 = "",
            //        COVERAGESDATE11 = "",
            //        COVERAGESDATE12 = "",
            //        COVERAGESDATE13 = "",
            //        COVERAGESDATE14 = "",
            //        COVERAGESDATE15 = "",
            //        COVERAGEEDATE1 = "",
            //        COVERAGEEDATE2 = "",
            //        COVERAGEEDATE3 = "",
            //        COVERAGEEDATE4 = "",
            //        COVERAGEEDATE5 = "",
            //        COVERAGEEDATE6 = "",
            //        COVERAGEEDATE7 = "",
            //        COVERAGEEDATE8 = "",
            //        COVERAGEEDATE9 = "",
            //        COVERAGEEDATE10 = "",
            //        COVERAGEEDATE11 = "",
            //        COVERAGEEDATE12 = "",
            //        COVERAGEEDATE13 = "",
            //        COVERAGEEDATE14 = "",
            //        COVERAGEEDATE15 = "",
            //        COVERAGEDISCOUNT1 = "",
            //        COVERAGEDISCOUNT2 = "",
            //        COVERAGEDISCOUNT3 = "",
            //        COVERAGEDISCOUNT4 = "",
            //        COVERAGEDISCOUNT5 = "",
            //        COVERAGEDISCOUNT6 = "",
            //        COVERAGEDISCOUNT7 = "",
            //        COVERAGEDISCOUNT8 = "",
            //        COVERAGEDISCOUNT9 = "",
            //        COVERAGEDISCOUNT10 = "",
            //        COVERAGEDISCOUNT11 = "",
            //        COVERAGEDISCOUNT12 = "",
            //        COVERAGEDISCOUNT13 = "",
            //        COVERAGEDISCOUNT14 = "",
            //        COVERAGEDISCOUNT15 = "",
            //        COVERAGEPCALC1 = "F",
            //        COVERAGEPCALC2 = "",
            //        COVERAGEPCALC3 = "",
            //        COVERAGEPCALC4 = "",
            //        COVERAGEPCALC5 = "",
            //        COVERAGEPCALC6 = "",
            //        COVERAGEPCALC7 = "",
            //        COVERAGEPCALC8 = "",
            //        COVERAGEPCALC9 = "",
            //        COVERAGEPCALC10 = "",
            //        COVERAGEPCALC11 = "",
            //        COVERAGEPCALC12 = "",
            //        COVERAGEPCALC13 = "",
            //        COVERAGEPCALC14 = "",
            //        COVERAGEPCALC15 = "",
            //        COVERAGEPRORATA1 = "",
            //        COVERAGEPRORATA2 = "",
            //        COVERAGEPRORATA3 = "",
            //        COVERAGEPRORATA4 = "",
            //        COVERAGEPRORATA5 = "",
            //        COVERAGEPRORATA6 = "",
            //        COVERAGEPRORATA7 = "",
            //        COVERAGEPRORATA8 = "",
            //        COVERAGEPRORATA9 = "",
            //        COVERAGEPRORATA10 = "",
            //        COVERAGEPRORATA11 = "",
            //        COVERAGEPRORATA12 = "",
            //        COVERAGEPRORATA13 = "",
            //        COVERAGEPRORATA14 = "",
            //        COVERAGEPRORATA15 = "",
            //        COVERAGEINDEMNITY1 = "",
            //        COVERAGEINDEMNITY2 = "",
            //        COVERAGEINDEMNITY3 = "",
            //        COVERAGEINDEMNITY4 = "",
            //        COVERAGEINDEMNITY5 = "",
            //        COVERAGEINDEMNITY6 = "",
            //        COVERAGEINDEMNITY7 = "",
            //        COVERAGEINDEMNITY8 = "",
            //        COVERAGEINDEMNITY9 = "",
            //        COVERAGEINDEMNITY10 = "",
            //        COVERAGEINDEMNITY11 = "",
            //        COVERAGEINDEMNITY12 = "",
            //        COVERAGEINDEMNITY13 = "",
            //        COVERAGEINDEMNITY14 = "",
            //        COVERAGEINDEMNITY15 = "",
            //        COVERAGELOADING1 = "",
            //        COVERAGELOADING2 = "",
            //        COVERAGELOADING3 = "",
            //        COVERAGELOADING4 = "",
            //        COVERAGELOADING5 = "",
            //        COVERAGELOADING6 = "",
            //        COVERAGELOADING7 = "",
            //        COVERAGELOADING8 = "",
            //        COVERAGELOADING9 = "",
            //        COVERAGELOADING10 = "",
            //        COVERAGELOADING11 = "",
            //        COVERAGELOADING12 = "",
            //        COVERAGELOADING13 = "",
            //        COVERAGELOADING14 = "",
            //        COVERAGELOADING15 = "",
            //        INTERESTCODE1 = "M08",
            //        INTERESTCODE2 = "",
            //        INTERESTCODE3 = "",
            //        INTERESTCODE4 = "",
            //        INTERESTCODE5 = "",
            //        INTERESTCODE6 = "",
            //        INTERESTCODE7 = "",
            //        INTERESTCODE8 = "",
            //        INTERESTCODE9 = "",
            //        INTERESTCODE10 = "",
            //        INTERESTREMARK1 = "Movable Property All Risk",
            //        INTERESTREMARK2 = "",
            //        INTERESTREMARK3 = "",
            //        INTERESTREMARK4 = "",
            //        INTERESTREMARK5 = "",
            //        INTERESTREMARK6 = "",
            //        INTERESTREMARK7 = "",
            //        INTERESTREMARK8 = "",
            //        INTERESTREMARK9 = "",
            //        INTERESTREMARK10 = "",
            //        INTERESTCURRENCY1 = "IDR",
            //        INTERESTCURRENCY2 = "",
            //        INTERESTCURRENCY3 = "",
            //        INTERESTCURRENCY4 = "",
            //        INTERESTCURRENCY5 = "",
            //        INTERESTCURRENCY6 = "",
            //        INTERESTCURRENCY7 = "",
            //        INTERESTCURRENCY8 = "",
            //        INTERESTCURRENCY9 = "",
            //        INTERESTCURRENCY10 = "",
            //        INTERESTSUMINSURED1 = "215,000.00",
            //        INTERESTSUMINSURED2 = "",
            //        INTERESTSUMINSURED3 = "",
            //        INTERESTSUMINSURED4 = "",
            //        INTERESTSUMINSURED5 = "",
            //        INTERESTSUMINSURED6 = "",
            //        INTERESTSUMINSURED7 = "",
            //        INTERESTSUMINSURED8 = "",
            //        INTERESTSUMINSURED9 = "",
            //        INTERESTSUMINSURED10 = "",
            //        MAXLIABILITY1 = "",
            //        MAXLIABILITY2 = "",
            //        MAXLIABILITY3 = "",
            //        MAXLIABILITY4 = "",
            //        MAXLIABILITY5 = "",
            //        MAXLIABILITY6 = "",
            //        MAXLIABILITY7 = "",
            //        MAXLIABILITY8 = "",
            //        MAXLIABILITY9 = "",
            //        MAXLIABILITY10 = "",
            //        DCODE1 = "",
            //        DCODE2 = "",
            //        DCODE3 = "",
            //        DCODE4 = "",
            //        DCODE5 = "",
            //        DCODE6 = "",
            //        REMARKS1 = "",
            //        REMARKS2 = "",
            //        REMARKS3 = "",
            //        REMARKS4 = "",
            //        REMARKS5 = "",
            //        REMARKS6 = "",
            //        PCTCL1 = "",
            //        PCTCL2 = "",
            //        PCTCL3 = "",
            //        PCTCL4 = "",
            //        PCTCL5 = "",
            //        PCTCL6 = "",
            //        PCTTSI1 = "",
            //        PCTTSI2 = "",
            //        PCTTSI3 = "",
            //        PCTTSI4 = "",
            //        PCTTSI5 = "",
            //        PCTTSI6 = "",
            //        CURRENCY1 = "",
            //        CURRENCY2 = "",
            //        CURRENCY3 = "",
            //        CURRENCY4 = "",
            //        CURRENCY5 = "",
            //        CURRENCY6 = "",
            //        FIXEDMAX1 = "",
            //        FIXEDMAX2 = "",
            //        FIXEDMAX3 = "",
            //        FIXEDMAX4 = "",
            //        FIXEDMAX5 = "",
            //        FIXEDMAX6 = "",
            //        FIXEDMIN1 = "",
            //        FIXEDMIN2 = "",
            //        FIXEDMIN3 = "",
            //        FIXEDMIN4 = "",
            //        FIXEDMIN5 = "",
            //        FIXEDMIN6 = "",
            //        FEE_ID = "",
            //        FEE_REMARK = "",
            //        FEE_CURRENCY = "",
            //        FEE_AMOUNT = "",
            //        DUTY_ID = "",
            //        DUTY_REMARK = "",
            //        DUTY_CURRENCY = "",
            //        DUTY_AMOUNT = "",
            //        BSID1 = "M01BW00004",
            //        BSID2 = "",
            //        BSID3 = "",
            //        BSTYPE1 = "B",
            //        BSTYPE2 = "",
            //        BSTYPE3 = "",
            //        BSFEE1 = "35",
            //        BSFEE2 = "",
            //        BSFEE3 = "",
            //        CLAUSE1 = "",
            //        CLAUSE2 = "",
            //        CLAUSE3 = "",
            //        CLAUSE4 = "",
            //        CLAUSE5 = "",
            //        CLAUSE6 = "",
            //        CLAUSE7 = "",
            //        CLAUSE8 = "",
            //        CLAUSE9 = "",
            //        CLAUSE10 = "",
            //        CLAUSE11 = "",
            //        CLAUSE12 = "",
            //        CLAUSE13 = "",
            //        CLAUSE14 = "",
            //        CLAUSE15 = "",
            //        LEADERID = "",
            //        LEADERSHARE = "",
            //        LEADERFEE = "",
            //        MEMBERID1 = "",
            //        MEMBERID2 = "",
            //        MEMBERID3 = "",
            //        MEMBERSHARE1 = "",
            //        MEMBERSHARE2 = "",
            //        MEMBERSHARE3 = "",
            //        MEMBERCOMM1 = "",
            //        MEMBERCOMM2 = "",
            //        MEMBERCOMM3 = "",
            //        MEMBERHF1 = "",
            //        MEMBERHF2 = "",
            //        MEMBERHF3 = "",
            //        MEMBERPCOLLECTIONF1 = "",
            //        MEMBERPCOLLECTIONF2 = "",
            //        MEMBERPCOLLECTIONF3 = "",
            //        MEMBERCCOLLECTIONF1 = "",
            //        MEMBERCCOLLECTIONF2 = "",
            //        MEMBERCCOLLECTIONF3 = "",
            //        EMETHOD = "",
            //        XPID = "",
            //        XPSHARE = "",
            //        CID = "",
            //        CSHARE = "",
            //        QID = "",
            //        QSHARE = "",
            //        SP1ID = "",
            //        SP1SHARE = "",
            //        SP2ID = "",
            //        SP2SHARE = "",
            //        SP3ID = "",
            //        SP3SHARE = "",
            //        SHORTFALL = "",
            //        AFID = "",
            //        AFSHARE = "",
            //        FACOUT1ID = "",
            //        FACOUT1RATE = "",
            //        FACOUT1SHARE = "",
            //        FACOUT1COMM = "",
            //        FACOUT1WPC = "",
            //        FACOUT2ID = "",
            //        FACOUT2RATE = "",
            //        FACOUT2SHARE = "",
            //        FACOUT2COMM = "",
            //        FACOUT2WPC = "",
            //        FACOUT3ID = "",
            //        FACOUT3RATE = "",
            //        FACOUT3SHARE = "",
            //        FACOUT3COMM = "",
            //        FACOUT3WPC = "",
            //        VOYAGEFROM = "",
            //        VOYAGETO = "",
            //        TRANSHIPMENT = "",
            //        ATANDFROM = "",
            //        TRANSTO = "",
            //        CONSIGNEE = "",
            //        CONSIGNEE_ADDRESS = ""
            //    });
            //}
        }
    }
}