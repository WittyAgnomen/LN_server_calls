/**
Search function/method for sreturning documents found number
 */
using System;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using WSKCons.wsapi;
using System.Configuration;
using System.Net;

namespace WSKCons
{
    class WSKSearch
    {
        
        public string search(string binarySecurityToken, string Search_Term, string SourceId)
        {
            string wskEndPoint = "";            // Endpoint server
            string wskID = "";                  // WSK Id
            string wskPassword = "";            // WSK Password
            string wskNamespace = "";           // Namespace, US or UK.

            System.Console.Out.WriteLine("\n********** Search (Boolean)");

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

            //string add = "";
            string enCodeQry = "";

            enCodeQry = srcQuery;

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
            sic.sourceIdList = new string[] { SourceId}; //Will need to loop through sources
            searchReq.sourceInformation = sic;

            // Set the date restriction
            DateRestriction dr = new DateRestriction();
            dr.endDateSpecified = false; //originally =false
            //dr.endDate = enddate; //new DateTime(2015, 9, 30); 2015-09-30
            dr.startDateSpecified = false; //originally =false
           // dr.startDate = startdate; //new DateTime(2015, 9, 1); 2015-09-01
            searchReq.searchOptions.dateRestriction = dr;

           
            // Set the retrieval options
            RetrievalOptions ro = new RetrievalOptions();
            ro.documentMarkup = DocumentMarkup.Display;
            ro.documentMarkupSpecified = true;
            ro.documentView = DocumentView.Cite;
            ro.documentViewSpecified = true;
            Range range = new Range();
            range.begin = "1";
            range.end = "10";
            ro.documentRange = range;
            searchReq.retrievalOptions = ro;

            System.Console.Out.WriteLine("SourceId: " + searchReq.sourceInformation.sourceIdList[0]);
            System.Console.Out.WriteLine("Query: " + searchReq.query + "\n");

            SearchSoapBinding searchBind = null;
            try
            {
                //Create the binding  
                searchBind = new SearchSoapBinding();

                // Adjust the endpoint URL to point to the correct environment
                searchBind.Url = fixEndPointURL(searchBind.Url, wskEndPoint);

                // Make the Web Service call
                searchResp = searchBind.Search(searchReq);

                // Use the response object
                System.Console.Out.WriteLine("Success");
                System.Console.Out.WriteLine("searchId = " + searchResp.searchId);
                string DocsFound = searchResp.documentsFound;
                System.Console.Out.WriteLine("Documents Found : " + DocsFound);

               //Console.WriteLine("Press enter to continue");
               // Console.ReadLine();

                return DocsFound;
            }


            finally
            {
                
                searchReq = null;
                searchBind = null;
                
            }

            
           
        }
        
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
