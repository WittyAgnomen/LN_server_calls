/**
Authentication
 * 
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
    class WSKAuthentication
    {
        public string authenticate()
        {
            string wskEndPoint = "";            // Endpoint server
            string wskID = "";                  // WSK Id
            string wskPassword = "";            // WSK Password
            string wskNamespace = "";           // Namespace, US or UK.

            System.Console.Out.WriteLine("\n********** Authenticate WSK ID and Password");

            // Get the configured endpoint
            wskEndPoint = ConfigurationManager.AppSettings["wskEndPoint"];

            // Get Authentication values from configuration file
            wskID = ConfigurationManager.AppSettings["wskID"];
            wskPassword = ConfigurationManager.AppSettings["wskPassword"];
            wskNamespace = ConfigurationManager.AppSettings["wskNamespace"];

            //************************ Authenticate ************************
            System.Console.Out.WriteLine("***** Authenticate...");

            // Create the request and response objects
            Authenticate authReq = new Authenticate();
            AuthenticateResponse authResp = null;

            // Populate the request with information
            authReq.authId = wskID;
            authReq.password = wskPassword;
            if (wskNamespace != null && wskNamespace != "")
            {
                authReq.@namespace = wskNamespace;
            }

            System.Console.Out.WriteLine("wskID: " + authReq.authId);
            System.Console.Out.WriteLine("wskPassword: " + authReq.password);
            System.Console.Out.WriteLine("wskNamespace: " + authReq.@namespace + "\n");

            AuthenticationSoapBinding authBind = null;
            try
            {
                // Create the binding
                authBind = new AuthenticationSoapBinding();

                // Adjust the endpoint URL to point to the correct environment
                authBind.Url = fixEndPointURL(authBind.Url, wskEndPoint);

                // Make the Web Service call
                authResp = authBind.Authenticate(authReq);

                // Use the response object
                if (authResp.binarySecurityToken != null)
                {
                    Console.Out.WriteLine("Success");
                    Console.Out.WriteLine("BinarySecurityToken: " + authResp.binarySecurityToken.ToString());
                    Console.Out.WriteLine("ExpiryDate: " + authResp.expiration.ToString());
                    Console.Out.WriteLine("locale: " + authResp.userInformation.locale.ToString());
                }

                return authResp.binarySecurityToken.ToString();
            }
            //catch (WebException we)
            //{
            //    // Possibly an HTTP Error?
            //    System.Console.Out.WriteLine("Web Exception: " + we.Message);
            //    return;
            //}
            //catch (SoapException se)
            //{
            //    // WSK has returned a Fault message (i.e. an error has occurred)
            //    // Extract the WSK-specific error code

            //    // Add a mapping of the XML prefix "p:" to the XML Namespace of WSK
            //    XmlNamespaceManager nsmgr =
            //        new XmlNamespaceManager(se.Detail.OwnerDocument.NameTable);
            //    nsmgr.AddNamespace("p", "http://services.v1.wsapi.lexisnexis.com");

            //    // Search the Fault message's detail for the  element
            //    XmlNode nd = se.Detail.SelectSingleNode("//p:errorCode", nsmgr);
            //    if (nd != null)
            //    {
            //        Console.Out.WriteLine("WSK error code: " + nd.InnerText);
            //    }
            //    return;
            //}
            //catch (Exception exc)
            //{
            //    // General Exception.
            //    Console.Out.WriteLine("General Exception: " + exc.Message);
            //    return;
            //}
            finally
            {
                authBind = null;
            }
        }

        // Set the correct environment in the end point URL
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
