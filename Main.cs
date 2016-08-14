/**
Main; where the search is loop are called
 * 
 */

using System;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using WSKCons.wsapi;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;


namespace WSKCons
{
    class MainClass
    {
        static void Main(string[] args)
        {
            //making arrays to search on------------------------------------------------------------------------
            //pick a case 1 is article search, 2 is art dl
            int caseSwitch = 2;

            //pick risk term
            //norm
            //string rt1 = "AND DATE ";
            //risk term 1
            //string rt1 = "AND (((currency OR FX OR exchange rate) w/1 (conversion OR inconvertibility OR regulation)) OR exchange inconvertibility OR (government AND (foreign OR multinational OR transnational) AND (inconvertibility OR export tax OR discriminatory regulation OR breach of contract OR (Reneging AND (contract OR agreement)) OR (Renege AND (contract OR agreement)) OR (abrogat! AND (agreement OR contract)) OR renegotiating terms  OR (tax payments AND dispute) OR (coer*ion AND (contract OR agreement)) OR (License  AND (cancellation OR revocation)) OR (permit AND (cancellation OR revocation))   OR  impairment OR BIT OR Bilateral Investment Treat!  OR arbitration OR nationali*ation  OR expropriat! OR (restrict! AND repatriation) OR limits on remittances OR discriminatory taxation OR violence  OR interference with operations  OR confiscat! OR diversion  OR active blockage OR commandeer OR seizure OR passive blockage))) AND DATE ";
            //risk term 2
            string rt1 = "AND ((foreign OR multinational OR transnational) AND (kidnap! OR hijack! OR (arrest! AND executive) OR imprison! executive OR boycot! OR sabotage OR damage to property OR damage to operations OR property destruction OR loss of property OR property loss OR bomb OR attack OR corruption)) AND DATE ";
            //risk term 3
            //string rt1 = "AND (mass labor strikes OR mass labour strikes OR general strike OR  social unrest OR ethnic conflict OR ethnic violence OR civil strife OR ethnic strife OR ethnic cleansing OR ethnic eradication OR genocide OR riot OR tear gas OR water canon OR mass protest OR (War AND civil) OR (War AND military) OR military conflict OR coup d’etat OR palace revolution OR military conflict OR military coup OR military overthrow OR military plot OR military combat OR military barrage OR military takeover OR military raid OR (Bombard AND civilians) OR IED OR Improvised explosive device OR (Bloodshed AND civilians) OR (Massacre AND civilians) OR (Slaughter AND civilians) OR Junta OR Dictator OR Dictatorship OR putsch OR regime change OR (overthrow  AND  government) OR weapons of mass destruction OR WMD OR terrorism OR terrorist OR Rebellion OR Revolution OR Revolt OR Occupation forces OR Hijack OR Guerrilla OR Jihadist OR suicide bomber OR suicide vest OR roadside bomb OR roadside explosive OR Molotov cocktail) AND DATE ";

            //years
            string[] dates = { "01/01/1980", "02/01/1980", "03/01/1980", "04/01/1980", "05/01/1980", "06/01/1980", "07/01/1980", "08/01/1980", "09/01/1980", "10/01/1980", "11/01/1980", "12/01/1980", "01/01/1981", "02/01/1981", "03/01/1981", "04/01/1981", "05/01/1981", "06/01/1981", "07/01/1981", "08/01/1981", "09/01/1981", "10/01/1981", "11/01/1981", "12/01/1981", "01/01/1982", "02/01/1982", "03/01/1982", "04/01/1982", "05/01/1982", "06/01/1982", "07/01/1982", "08/01/1982", "09/01/1982", "10/01/1982", "11/01/1982", "12/01/1982", "01/01/1983", "02/01/1983", "03/01/1983", "04/01/1983", "05/01/1983", "06/01/1983", "07/01/1983", "08/01/1983", "09/01/1983", "10/01/1983", "11/01/1983", "12/01/1983", "01/01/1984", "02/01/1984", "03/01/1984", "04/01/1984", "05/01/1984", "06/01/1984", "07/01/1984", "08/01/1984", "09/01/1984", "10/01/1984", "11/01/1984", "12/01/1984", "01/01/1985", "02/01/1985", "03/01/1985", "04/01/1985", "05/01/1985", "06/01/1985", "07/01/1985", "08/01/1985", "09/01/1985", "10/01/1985", "11/01/1985", "12/01/1985", "01/01/1986", "02/01/1986", "03/01/1986", "04/01/1986", "05/01/1986", "06/01/1986", "07/01/1986", "08/01/1986", "09/01/1986", "10/01/1986", "11/01/1986", "12/01/1986", "01/01/1987", "02/01/1987", "03/01/1987", "04/01/1987", "05/01/1987", "06/01/1987", "07/01/1987", "08/01/1987", "09/01/1987", "10/01/1987", "11/01/1987", "12/01/1987", "01/01/1988", "02/01/1988", "03/01/1988", "04/01/1988", "05/01/1988", "06/01/1988", "07/01/1988", "08/01/1988", "09/01/1988", "10/01/1988", "11/01/1988", "12/01/1988", "01/01/1989", "02/01/1989", "03/01/1989", "04/01/1989", "05/01/1989", "06/01/1989", "07/01/1989", "08/01/1989", "09/01/1989", "10/01/1989", "11/01/1989", "12/01/1989", "01/01/1990", "02/01/1990", "03/01/1990", "04/01/1990", "05/01/1990", "06/01/1990", "07/01/1990", "08/01/1990", "09/01/1990", "10/01/1990", "11/01/1990", "12/01/1990", "01/01/1991", "02/01/1991", "03/01/1991", "04/01/1991", "05/01/1991", "06/01/1991", "07/01/1991", "08/01/1991", "09/01/1991", "10/01/1991", "11/01/1991", "12/01/1991", "01/01/1992", "02/01/1992", "03/01/1992", "04/01/1992", "05/01/1992", "06/01/1992", "07/01/1992", "08/01/1992", "09/01/1992", "10/01/1992", "11/01/1992", "12/01/1992", "01/01/1993", "02/01/1993", "03/01/1993", "04/01/1993", "05/01/1993", "06/01/1993", "07/01/1993", "08/01/1993", "09/01/1993", "10/01/1993", "11/01/1993", "12/01/1993", "01/01/1994", "02/01/1994", "03/01/1994", "04/01/1994", "05/01/1994", "06/01/1994", "07/01/1994", "08/01/1994", "09/01/1994", "10/01/1994", "11/01/1994", "12/01/1994", "01/01/1995", "02/01/1995", "03/01/1995", "04/01/1995", "05/01/1995", "06/01/1995", "07/01/1995", "08/01/1995", "09/01/1995", "10/01/1995", "11/01/1995", "12/01/1995", "01/01/1996", "02/01/1996", "03/01/1996", "04/01/1996", "05/01/1996", "06/01/1996", "07/01/1996", "08/01/1996", "09/01/1996", "10/01/1996", "11/01/1996", "12/01/1996", "01/01/1997", "02/01/1997", "03/01/1997", "04/01/1997", "05/01/1997", "06/01/1997", "07/01/1997", "08/01/1997", "09/01/1997", "10/01/1997", "11/01/1997", "12/01/1997", "01/01/1998", "02/01/1998", "03/01/1998", "04/01/1998", "05/01/1998", "06/01/1998", "07/01/1998", "08/01/1998", "09/01/1998", "10/01/1998", "11/01/1998", "12/01/1998", "01/01/1999", "02/01/1999", "03/01/1999", "04/01/1999", "05/01/1999", "06/01/1999", "07/01/1999", "08/01/1999", "09/01/1999", "10/01/1999", "11/01/1999", "12/01/1999", "01/01/2000", "02/01/2000", "03/01/2000", "04/01/2000", "05/01/2000", "06/01/2000", "07/01/2000", "08/01/2000", "09/01/2000", "10/01/2000", "11/01/2000", "12/01/2000", "01/01/2001", "02/01/2001", "03/01/2001", "04/01/2001", "05/01/2001", "06/01/2001", "07/01/2001", "08/01/2001", "09/01/2001", "10/01/2001", "11/01/2001", "12/01/2001", "01/01/2002", "02/01/2002", "03/01/2002", "04/01/2002", "05/01/2002", "06/01/2002", "07/01/2002", "08/01/2002", "09/01/2002", "10/01/2002", "11/01/2002", "12/01/2002", "01/01/2003", "02/01/2003", "03/01/2003", "04/01/2003", "05/01/2003", "06/01/2003", "07/01/2003", "08/01/2003", "09/01/2003", "10/01/2003", "11/01/2003", "12/01/2003", "01/01/2004", "02/01/2004", "03/01/2004", "04/01/2004", "05/01/2004", "06/01/2004", "07/01/2004", "08/01/2004", "09/01/2004", "10/01/2004", "11/01/2004", "12/01/2004", "01/01/2005", "02/01/2005", "03/01/2005", "04/01/2005", "05/01/2005", "06/01/2005", "07/01/2005", "08/01/2005", "09/01/2005", "10/01/2005", "11/01/2005", "12/01/2005", "01/01/2006", "02/01/2006", "03/01/2006", "04/01/2006", "05/01/2006", "06/01/2006", "07/01/2006", "08/01/2006", "09/01/2006", "10/01/2006", "11/01/2006", "12/01/2006", "01/01/2007", "02/01/2007", "03/01/2007", "04/01/2007", "05/01/2007", "06/01/2007", "07/01/2007", "08/01/2007", "09/01/2007", "10/01/2007", "11/01/2007", "12/01/2007", "01/01/2008", "02/01/2008", "03/01/2008", "04/01/2008", "05/01/2008", "06/01/2008", "07/01/2008", "08/01/2008", "09/01/2008", "10/01/2008", "11/01/2008", "12/01/2008", "01/01/2009", "02/01/2009", "03/01/2009", "04/01/2009", "05/01/2009", "06/01/2009", "07/01/2009", "08/01/2009", "09/01/2009", "10/01/2009", "11/01/2009", "12/01/2009", "01/01/2010", "02/01/2010", "03/01/2010", "04/01/2010", "05/01/2010", "06/01/2010", "07/01/2010", "08/01/2010", "09/01/2010", "10/01/2010", "11/01/2010", "12/01/2010", "01/01/2011", "02/01/2011", "03/01/2011", "04/01/2011", "05/01/2011", "06/01/2011", "07/01/2011", "08/01/2011", "09/01/2011", "10/01/2011", "11/01/2011", "12/01/2011", "01/01/2012", "02/01/2012", "03/01/2012", "04/01/2012", "05/01/2012", "06/01/2012", "07/01/2012", "08/01/2012", "09/01/2012", "10/01/2012", "11/01/2012", "12/01/2012", "12/01/2012", "01/01/2013", "02/01/2013", "03/01/2013", "04/01/2013", "05/01/2013", "06/01/2013", "07/01/2013", "08/01/2013", "09/01/2013", "10/01/2013", "11/01/2013", "12/01/2013", "01/01/2014", "02/01/2014", "03/01/2014", "04/01/2014", "05/01/2014", "06/01/2014", "07/01/2014", "08/01/2014", "09/01/2014", "10/01/2014", "11/01/2014", "12/01/2014", "01/01/2015", "02/01/2015", "03/01/2015", "04/01/2015", "05/01/2015", "06/01/2015", "07/01/2015", "08/01/2015", "09/01/2015", "10/01/2015", "11/01/2015", "12/01/2015", "01/01/2016" };

            //country
            //string[] country = { "Chin! ", "Colombia* ", "Ivorian OR \"Ivory Coast\" ", "Croatia* ", "Denmark OR Danish ", "Dominican Republic ", "Ecuador! ", "Egypt! ", "El Salvador OR Salvador! ", "Finland OR Finnish " };
            string[] country = { "Argentin! " };

            //create searh space

            List<string> sea_space = new List<string>();
            for (int y = 0; y < (country.Length); y += 1)
            {
                for (int x = 0; x < (dates.Length) - 1; x += 1)
                {
                    sea_space.Add((country[y] + rt1 + ">= " + dates[x] + " AND <" + dates[x + 1]).Replace(Convert.ToChar(160).ToString(), ""));
                }
            }

            //convert list to array
            string[] searchTerms = sea_space.ToArray();

            /*
            //to test whats in search array
             for (int i=0; i < searchTerms.Length; i++)
                {
                    Console.WriteLine(searchTerms[i]);
                }
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
            */

            //source array 1 Array for souces set1 (regular set) 8010 del  
            string[] sourceTerms = {   "244786", "8357", "8006", "5774", "7945", "8109", "138620", "8200", "142626", "6742", "379740", "247189","8075", "8213", "409005",
            "158262", "265544", "138211", "244784", "144760", "10882", "8002",   "11314", "7911", "244788", "138794", "138528", "303830", "145202", "8286"};

            //------------------------------------------start searching--------------------------------------------------
            //Authenticate against the WSK  need tis to return token
            WSKAuthentication auth = new WSKAuthentication();
            string token = auth.authenticate();

            switch (caseSwitch)
            {

                case 1:
                    {
                        Console.WriteLine("Case 1 chosen, article count");
                        //start of code for using just article obtainment
                        WSKSearch so = new WSKSearch();

                        //counter for timer cant exceed 3000 queueries per hour
                        int ques = 0;
                        int total = 0;

                        //first time stamp
                        var starttime = DateTime.Now;

                        //write output to csv
                        using (CsvFileWriter writer = new CsvFileWriter(filename: @"S1R2_8091_us.csv"))
                        {
                            for (int i = 0; i < (searchTerms.Length); i += 1)
                            {
                                for (int j = 0; j < (sourceTerms.Length); j += 1)
                                {

                                    string hey = null; //declare hey outside try/ catch

                                    try
                                    {
                                        hey = so.search(token, searchTerms[i], sourceTerms[j]); //get a result for one queuery
                                    }

                                    catch (Exception ex)
                                    {
                                        Console.Out.WriteLine("Error: " + ex.Message);
                                        hey = "Error: " + ex.Message;
                                    }

                                    ques += 1; //update number of times method is called

                                    total += 1;
                                    Console.Out.WriteLine("number of searches in this hour: " + ques.ToString());
                                    Console.Out.WriteLine("number of searches in run: " + total.ToString());

                                    //Console.ReadLine();

                                    //write to csv
                                    CsvRow row = new CsvRow();
                                    row.Add(searchTerms[i]);
                                    row.Add(sourceTerms[j]);
                                    row.Add(hey);
                                    writer.WriteRow(row);

                                    //to control 3000 or less dls in an hour and 3000 or less queueries in an hour
                                    if (ques < 3000)
                                    {
                                        //Thread.Sleep(5000); //wait five seconds
                                        //Console.ReadLine();
                                    }
                                    else
                                    {
                                        var curTime = DateTime.Now;

                                        //for sleeper
                                        int wait = Math.Max(60 - ((curTime - starttime).Minutes), 0);

                                        for (int w = 1; w <= wait; w += 1)
                                        {
                                            int waiter = 60000;
                                            Thread.Sleep(waiter); //sleep wait time
                                            Console.Out.WriteLine(w);
                                        }

                                        starttime = DateTime.Now; //reset starttime
                                        ques = 0; //restart srch counter
                                    }

                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("Case 2 chosen, article dl");
                        //-------------------------------------------------------------------------------------------
                        //start of code for dl articles
                        WSKGetDocumentsByRange so = new WSKGetDocumentsByRange();

                        //counter for timer cant exceed 3000 dls per hour
                        int dls = 0;

                        //counter for number of times queueried
                        int srch = 0;
                        int total = 0;

                        //first time stamp
                        var starttime = DateTime.Now;

                        using (CsvFileWriter writer = new CsvFileWriter(filename: @"S1R2_Arg.csv"))
                        {
                            for (int i = 0; i < (searchTerms.Length); i += 1)
                            {
                                for (int j = 0; j < (sourceTerms.Length); j += 1)
                                {

                                    List<string> hey = new List<string>(); //declare hey outside try/ catch

                                    try
                                    {
                                        hey = so.getDocumentsByRange(token, searchTerms[i], sourceTerms[j]); //get a result for one queuery
                                    }

                                    catch (Exception ex)
                                    {
                                        Console.Out.WriteLine("Error: " + ex.Message);
                                        hey.Add("0");
                                        hey.Add("Error: " + ex.Message);
                                    }

                                    srch += 1; //update number of times method is called

                                    total += 1;
                                    Console.Out.WriteLine("number of searches in this hour: " + srch.ToString());
                                    Console.Out.WriteLine("number of searches in run: " + total.ToString());
                                    Console.Out.WriteLine("number of dls in run: " + dls.ToString());

                                    //Console.ReadLine();

                                    //write output to csv
                                    int counter = hey.Count;

                                    for (int iii = 1; iii < counter; iii++)
                                    {
                                        CsvRow row = new CsvRow();
                                        row.Add(searchTerms[i]);
                                        row.Add(sourceTerms[j]);
                                        row.Add(hey[0]);
                                        row.Add(hey[iii]);
                                        writer.WriteRow(row);
                                    }

                                    try
                                    {
                                        dls += Convert.ToInt32(hey[0]); //update dls count
                                    }

                                    catch (Exception ex)
                                    {
                                        Console.Out.WriteLine("Error: " + ex.Message);
                                        dls += 0; //update dls count
                                    }


                                    //to control 3000 or less dls in an hour and 3000 or less queueries in an hour
                                    if (srch < 3000)
                                    {
                                        //Thread.Sleep(5000); //wait five seconds
                                        //Console.ReadLine();
                                    }
                                    else
                                    {
                                        var curTime = DateTime.Now;

                                        //for sleeper
                                        int wait = Math.Max(60 - ((curTime - starttime).Minutes), 0);

                                        for (int w = 1; w <= wait; w += 1)
                                        {
                                            int waiter = 60000;
                                            Thread.Sleep(waiter); //sleep wait time
                                            Console.Out.WriteLine(w);
                                        }

                                        starttime = DateTime.Now; //reset starttime
                                        srch = 0; //restart srch counter
                                    }
                                }
                            }
                        }
                        break;
                    }
                default:
                    Console.WriteLine("Default case Nothng happens");
                    break;
            }
        }

    }
}
