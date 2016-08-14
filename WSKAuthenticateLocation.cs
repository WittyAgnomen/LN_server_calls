/**
Authenticate Location
 */

using System;
using System.Web.Services.Protocols;
using System.Xml;
using WSKCons.wsapi;
using System.Configuration;
using System.Text;
using System.Net;

namespace WSKCons
{
    class WSKAuthenticateLocation
    {
       public void authenticateLocation()
       {
           string wskEndPoint = "";            // Endpoint server
           string wskID = "";                  // WSK Id
           string wskPassword = "";            // WSK Password
           string wskNamespace = "";           // Namespace, US or UK.
           string locationNamespace = "";      // Location Namespace

           System.Console.Out.WriteLine("\n********** AuthenticateLocation - IP based authentication");

           // Get the configured endpoint
           wskEndPoint = ConfigurationManager.AppSettings["wskEndPoint"];

           // Get Authentication values from configuration file
           wskID = ConfigurationManager.AppSettings["wskIDAdmin"];
           wskPassword = ConfigurationManager.AppSettings["wskPasswordAdmin"];
           wskNamespace = ConfigurationManager.AppSettings["wskNamespaceAdmin"];

           // Get location values from configuration file
           locationNamespace = ConfigurationManager.AppSettings["locationNamespace"];

           //************************ AuthenticateEndUser ************************
           System.Console.Out.WriteLine("***** AuthenticateLocation...");

           // Create the request and response objects
           AuthenticateLocation authLocReq = new AuthenticateLocation();
           AuthenticateLocationResponse authLocResp = null;

           // Populate the request with information
           if (locationNamespace != null && locationNamespace != "")
           {
               authLocReq.@namespace = locationNamespace;
           }
           
           System.Console.Out.WriteLine("locationNamespace: " + authLocReq.@namespace + "\n");

           AuthenticationSoapBinding authBind = null;
           try
           {
               // Create the binding
               authBind = new AuthenticationSoapBinding();

               // Adjust the endpoint URL to point to the correct environment
               authBind.Url = fixEndPointURL(authBind.Url, wskEndPoint);

               // Make the Web Service call
               authLocResp = authBind.AuthenticateLocation(authLocReq);

               // Use the response object
               if (authLocResp.binarySecurityToken != null)
               {
                   Console.Out.WriteLine("Success");
                   Console.Out.WriteLine("BinarySecurityToken: " + authLocResp.binarySecurityToken.ToString());
                   Console.Out.WriteLine("ExpiryDate: " + authLocResp.expiration.ToString());
                   Console.Out.WriteLine("locale: " + authLocResp.userInformation.locale.ToString());
               }
           }
           catch (WebException we)
           {
               // Possibly an HTTP Error?
               System.Console.Out.WriteLine("Web Exception: " + we.Message);
               return;
           }
           catch (SoapException se)
           {
               // WSK has returned a Fault message (i.e. an error has occurred)
               // Extract the WSK-specific error code

               // Add a mapping of the XML prefix "p:" to the XML Namespace of WSK
               XmlNamespaceManager nsmgr =
                   new XmlNamespaceManager(se.Detail.OwnerDocument.NameTable);
               nsmgr.AddNamespace("p", "http://services.v1.wsapi.lexisnexis.com");

               // Search the Fault message's detail for the  element
               XmlNode nd = se.Detail.SelectSingleNode("//p:errorCode", nsmgr);
               if (nd != null)
               {
                   Console.Out.WriteLine("WSK error code: " + nd.InnerText);
               }
               return;
           }
           catch (Exception exc)
           {
               // General Exception.
               Console.Out.WriteLine("General Exception: " + exc.Message);
               return;
           }
           finally
           {
               authLocResp = null;
               authBind = null;
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