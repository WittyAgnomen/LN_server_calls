/**
 Function for returning full text
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using WSKCons.wsapi;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

namespace WSKCons
{
    class WSKGetDocumentsByRange
    {

        public List<string> getDocumentsByRange(string binarySecurityToken, string Search_Term, string SourceId)
        {
            string wskEndPoint = "";            // Endpoint server
            string wskID = "";                  // WSK Id
            string wskPassword = "";            // WSK Password
            string wskNamespace = "";           // Namespace, US or UK.

            List<string> articles = new List<string>();

            System.Console.Out.WriteLine("\n********** GetDocumentsByRange - Get a range of documents from an existing result set");

            // Get the configured endpoint
            wskEndPoint = ConfigurationManager.AppSettings["wskEndPoint"];

            // Get Authentication values from configuration file
            wskID = ConfigurationManager.AppSettings["wskID"];
            wskPassword = ConfigurationManager.AppSettings["wskPassword"];
            wskNamespace = ConfigurationManager.AppSettings["wskNamespace"];


            //************************ Search ************************
            System.Console.Out.WriteLine("\n***** Search...");

            //Create the request and response objects
            Search searchReq = new Search();
            SearchResponse searchResp = new SearchResponse();
            string srcQuery = Search_Term;

            //string srcQuery = "Argentin! AND (mass labor strikes OR mass labour strikes OR general strike OR  social unrest OR ethnic conflict OR ethnic violence OR civil strife OR ethnic strife OR ethnic cleansing OR ethnic eradication OR genocide OR riot OR tear gas OR water canon OR mass protest OR (War AND civil) OR (War AND military) OR military conflict OR coup d’etat OR palace revolution OR military conflict OR military coup OR military overthrow OR military plot OR military combat OR military barrage OR military takeover OR military raid OR (Bombard AND civilians) OR IED OR Improvised explosive device OR (Bloodshed AND civilians) OR (Massacre AND civilians) OR (Slaughter AND civilians) OR Junta OR Dictator OR Dictatorship OR putsch OR regime change OR (overthrow  AND  government) OR weapons of mass destruction OR WMD OR terrorism OR terrorist OR Rebellion OR Revolution OR Revolt OR Occupation forces OR Hijack OR Guerrilla OR Jihadist OR suicide bomber OR suicide vest OR roadside bomb OR roadside explosive OR Molotov cocktail) AND DATE >= 07/01/2015 AND <08/01/2015";
            //string srcQuery = "running to fast";
            string add = "";
            string enCodeQry = "";

            StringBuilder sb = new StringBuilder();
            foreach (char c in srcQuery)
            {
                int a = Convert.ToInt32(c);
                byte[] b = Encoding.ASCII.GetBytes(c.ToString());
                int ascii = b[0];
                if (a != ascii)
                {
                    add = "&" + "#" + a + ";";
                }
                else
                {
                    add = c.ToString();
                }

                sb.Append(add);
            }
            enCodeQry = sb.ToString();

            //Populate the request with information
            searchReq.binarySecurityToken = binarySecurityToken;
            searchReq.locale = Locale.enUS;
            searchReq.projectId = "WSKSample";
            searchReq.query = enCodeQry;
            searchReq.useCSP = false;

            // Set the search options
            SearchOptions so = new SearchOptions();
            so.searchMethod = SearchMethod.Boolean;     // Boolean Search
            so.sortOrder = SortOrder.Implied;
            searchReq.searchOptions = so;

            // Set the source to search
            SourceInformationChoice sic = new SourceInformationChoice();
            sic.securedSourceId = null;
            sic.sourceIdList = new string[] { SourceId }; //Will need to loop through sources
            searchReq.sourceInformation = sic;

            // Set the date restriction
            DateRestriction dr = new DateRestriction();
            dr.endDateSpecified = false;
            dr.startDateSpecified = false;
            searchReq.searchOptions.dateRestriction = dr;

            // Set the retrieval options
            //RetrievalOptions ro = new RetrievalOptions();
            //ro.documentMarkup = DocumentMarkup.Display;
            //ro.documentMarkupSpecified = true;
            //ro.documentView = DocumentView.Cite;
            //ro.documentViewSpecified = true;
            //Range range = new Range();
            //range.begin = "1";
            //range.end = "10";
            //ro.documentRange = range;

            System.Console.Out.WriteLine("SourceId: " + searchReq.sourceInformation.sourceIdList[0]);
            System.Console.Out.WriteLine("Query: " + searchReq.query + "\n");

            searchReq.retrievalOptions = null;
            SearchSoapBinding searchBind;
            try
            {
                //Create the binding  
                searchBind = new SearchSoapBinding();

                // Change the endpoint according to your WSK environment
                searchBind.Url = fixEndPointURL(searchBind.Url, wskEndPoint);
                // Make the Web Service call
                searchResp = searchBind.Search(searchReq);

                // Use the response object
                System.Console.Out.WriteLine("Success");
                System.Console.Out.WriteLine("searchId = " + searchResp.searchId);
                string DocsFound = searchResp.documentsFound;
                System.Console.Out.WriteLine("Documents Found : " + searchResp.documentsFound);

                articles.Add(DocsFound);

                // Iterrate throuh the result set if documents have been returned.
                if (searchResp.documentContainerList != null)
                {
                    System.Console.Out.WriteLine("Documents Returned : " + searchResp.documentContainerList.Length);
                    for (int i = 0; i < searchResp.documentContainerList.Length; i++)
                    {
                        System.Console.Out.WriteLine("Doc[" + i + "] documentId: " + searchResp.documentContainerList[i].documentId);
                        // Convert the document byte stream to a string
                        String sDoc = Encoding.UTF8.GetString(searchResp.documentContainerList[i].document);
                        System.Console.Out.WriteLine("Doc[" + i + "]: " + sDoc);
                    }
                }

                // Iterrare through the disagnostic list if there is one.
                if (searchResp.diagnosticList != null)
                {
                    System.Console.Out.WriteLine("Diagnostics Returned : " + searchResp.diagnosticList.Length);
                    for (int i = 0; i <= searchResp.diagnosticList.Length; i++)
                    {
                        Diagnostic diag = searchResp.diagnosticList[i];

                        System.Console.Out.WriteLine("Diagnostics[" + i + "]:" + diag.code + diag.rationaleList[i].name + diag.rationaleList[i].message);
                    }
                }
            }
            catch (SoapException se)
            {

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(se.Detail.OwnerDocument.NameTable);
                nsmgr.AddNamespace("p", "http://services.v1.wsapi.lexisnexis.com");
                XmlNode nd = se.Detail.SelectSingleNode("//p:errorCode", nsmgr);
                if (nd != null)
                {
                    Console.Out.WriteLine("Detail: " + nd.InnerText);
                }
                else
                {
                    Console.Out.WriteLine("Error");
                }

            }
            catch (Exception exc)
            {
                Console.Out.WriteLine("General Exception: " + exc.Message);

            }
            finally
            {
                searchReq = null;
                searchBind = null;
            }

            //************************ GetDocumentByRange ************************
            //build a loop for the output; think there may be a better way and use and if/else

            System.Console.Out.WriteLine("\n***** GetDocumentsByRange...");




            //using (CsvFileWriter writer = new CsvFileWriter(filename: @"Results\" + Search_Term.Substring(0, 3) + SourceId + ".csv"))
            //{
            if (Convert.ToInt32(searchResp.documentsFound) == 0)
            {
                string noart = "No Articles";
                articles.Add(noart);
                return articles;
            }
            else
            {
                for (int looper = 0; looper < (Convert.ToInt32(searchResp.documentsFound)); looper += 10)
                {
                    // Convert.ToInt32(searchResp.documentsFound)
                    //Create the request and response objects
                    GetDocumentsByRange gdbrReq = new GetDocumentsByRange();
                    RetrievalResponse gdbrResp = new RetrievalResponse();

                    //Populate the request with information
                    gdbrReq.binarySecurityToken = binarySecurityToken;
                    gdbrReq.searchId = searchResp.searchId;

                    // Set the retrieval options
                    RetrievalOptions gdro = new RetrievalOptions();
                    gdro.documentMarkup = DocumentMarkup.Semantic;
                    gdro.documentMarkupSpecified = true;
                    gdro.documentView = DocumentView.FullText;  //ExpandedCite;
                    gdro.documentViewSpecified = true;
                    Range gdrange = new Range();

                    gdrange.begin = (looper + 1).ToString(); // looper + 1
                    gdrange.end = (looper + 10).ToString(); //looper + 10


                    gdro.documentRange = gdrange;
                    gdbrReq.retrievalOptions = gdro;

                    System.Console.Out.WriteLine("documentMarkup: " + gdbrReq.retrievalOptions.documentMarkup.ToString());
                    System.Console.Out.WriteLine("documentView: " + gdbrReq.retrievalOptions.documentView.ToString());
                    System.Console.Out.WriteLine("range: " + gdbrReq.retrievalOptions.documentRange.begin + " to " +
                        gdbrReq.retrievalOptions.documentRange.end + "\n");

                    //Console.WriteLine("Press enter to continue");
                    //Console.ReadLine();

                    RetrievalSoapBinding retrieveBind;
                    try
                    {
                        //Create the binding  
                        retrieveBind = new RetrievalSoapBinding();

                        // Change the endpoint according to your WSK environment
                        retrieveBind.Url = fixEndPointURL(retrieveBind.Url, wskEndPoint);
                        // Make the Web Service call
                        gdbrResp = retrieveBind.GetDocumentsByRange(gdbrReq);

                        // Use the response object
                        System.Console.Out.WriteLine("Success");

                        // Iterrate throuh the result set if documents have been returned.

                        if (gdbrResp.documentContainerList != null)
                        {
                            System.Console.Out.WriteLine("Documents Returned : " + gdbrResp.documentContainerList.Length);
                            for (int i = 0; i < gdbrResp.documentContainerList.Length; i++)
                            {

                                System.Console.Out.WriteLine("Doc[" + i + "] documentId: " + gdbrResp.documentContainerList[i].documentId);
                                // Convert the document byte stream to a string
                                String sDoc = Encoding.UTF8.GetString(gdbrResp.documentContainerList[i].document);
                                System.Console.Out.WriteLine("Doc[" + i + "]: " + sDoc);

                                articles.Add(sDoc);

                                //System.Console.ReadLine();

                            }

                        }
                    }

                    catch (SoapException se)
                    {

                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(se.Detail.OwnerDocument.NameTable);
                        nsmgr.AddNamespace("p", "http://services.v1.wsapi.lexisnexis.com");
                        XmlNode nd = se.Detail.SelectSingleNode("//p:errorCode", nsmgr);
                        if (nd != null)
                        {
                            Console.Out.WriteLine("Detail: " + nd.InnerText);
                        }
                        else
                        {
                            Console.Out.WriteLine("Error");
                        }

                    }
                    catch (Exception exc)
                    {
                        Console.Out.WriteLine("General Exception: " + exc.Message);

                    }
                    finally
                    {
                        gdbrReq = null;
                        retrieveBind = null;
                    }
                }

                return articles;

            }
        }
    
            //}
        

        private string fixEndPointURL(string url, string env)
        {
            // Strip the existing server name from the URL - remove all characters between
            // the "//" and the next "/"
            int startChr = url.IndexOf("//") + 2;
            int endChr = url.IndexOf("/", startChr);
            url = url.Remove(startChr, endChr - startChr);
            // Insert the correct server name.
            url = url.Insert(startChr, env);

            return url;
        }
    }
}
