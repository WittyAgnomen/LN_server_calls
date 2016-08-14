/**
Authenticate End User
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
    class WSKAuthenticateEndUser
    {
        public void authenticateEndUser()
        {
            string wskEndPoint = "";            // Endpoint server
            string wskID = "";                  // WSK Id
            string wskPassword = "";            // WSK Password
            string wskNamespace = "";           // Namespace, US or UK.
            string endUserID = "";              // End User Product Id
            string endUserPassword = "";        // End User Product Password
            string endUserProduct = "";         // End User Product

            System.Console.Out.WriteLine("\n********** AuthenticateEndUser - Authenticate as the user of another lexisNexis product");

            // Get the configured endpoint
            wskEndPoint = ConfigurationManager.AppSettings["wskEndPoint"];

            // Get Authentication values from configuration file
            wskID = ConfigurationManager.AppSettings["wskIDAdmin"];
            wskPassword = ConfigurationManager.AppSettings["wskPasswordAdmin"];
            wskNamespace = ConfigurationManager.AppSettings["wskNamespaceAdmin"];

            // Get End User product Authentication values from configuration file
            endUserID = ConfigurationManager.AppSettings["endUserID"];
            endUserPassword = ConfigurationManager.AppSettings["endUserPassword"];
            endUserProduct = ConfigurationManager.AppSettings["endUserProduct"];

            //************************ AuthenticateEndUser ************************
            System.Console.Out.WriteLine("***** AuthenticateEndUser...");

            // Create the request and response objects
            AuthenticateEndUser authEndUserReq = new AuthenticateEndUser();
            AuthenticateEndUserResponse authEndUserResp = null;

            // Set the WSK authentication details
            WskCredentials wskCreds = new WskCredentials();
            wskCreds.authId = wskID;
            wskCreds.password = wskPassword;
            if (wskNamespace != null && wskNamespace != "")
            {
                wskCreds.@namespace = wskNamespace;
            }

            // Set the Nexis.com Authentication details
            EndUserCredentials endUserCreds = new EndUserCredentials();
            endUserCreds.authId = endUserID;
            endUserCreds.password = endUserPassword;

            // Populate the request with information
            authEndUserReq.product = Product.NexisCom; // Default Nexis
            if (endUserProduct != null)
            {    
                if (endUserProduct.Equals("Lexis"))
                {
                    authEndUserReq.product = Product.LexisCom;
                }
                else if (endUserProduct.Equals("Dossier"))
                {
                    authEndUserReq.product = Product.Dossier;
                }
            }
            authEndUserReq.wskCredentials = wskCreds;
            authEndUserReq.endUserCredentials = endUserCreds;

            System.Console.Out.WriteLine("wskID: " + authEndUserReq.wskCredentials.authId);
            System.Console.Out.WriteLine("wskPassword: " + authEndUserReq.wskCredentials.password);
            System.Console.Out.WriteLine("wskNamespace: " + authEndUserReq.wskCredentials.@namespace + "\n");

            System.Console.Out.WriteLine("Product: " + authEndUserReq.product.ToString());
            System.Console.Out.WriteLine("endUserID: " + authEndUserReq.endUserCredentials.authId);
            System.Console.Out.WriteLine("endUserPassword: " + authEndUserReq.endUserCredentials.password + "\n");

            AuthenticationSoapBinding authBind = null;
            try
            {
                // Create the binding
                authBind = new AuthenticationSoapBinding();

                // Adjust the endpoint URL to point to the correct environment
                authBind.Url = fixEndPointURL(authBind.Url, wskEndPoint);

                // Make the Web Service call
                authEndUserResp = authBind.AuthenticateEndUser(authEndUserReq);
                
                // Use the response object
                if (authEndUserResp.binarySecurityToken != null)
                {
                    Console.Out.WriteLine("Success");
                    Console.Out.WriteLine("BinarySecurityToken: " + authEndUserResp.binarySecurityToken.ToString());
                    Console.Out.WriteLine("ExpiryDate: " + authEndUserResp.expiration.ToString());
                    Console.Out.WriteLine("locale: " + authEndUserResp.userInformation.locale.ToString());
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
                authEndUserReq = null;
                wskCreds=null;
                endUserCreds=null;
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
