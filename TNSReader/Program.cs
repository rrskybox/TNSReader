/*
* TennisNet is a Transient Name Server client for assembling supernova data
* 
* Author:           Rick McAlister
* Date:             12/21/18
* Current Version:  0.1
* Developed in:     Visual Studio 2017
* Coded in:         C 7.0
* App Envioronment: Windows 10 Pro (V1809)
* 
* Change Log:
* 
* 12/22/18 Rev 1.0  Release
* 
*/

using System;
using System.Collections.Specialized;
using System.Deployment.Application;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TNSReader
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string version;
            try
            { version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(); }
            catch
            {
                //probably in debug mode
                version = "in Debug";
            }
            Console.WriteLine("TNSReader V " + version);
            TNStoClipboard();
        }

        static void TNStoClipboard()
        {
            // url of TNS and TNS-sandbox api                                     
            const string url_tns_search = "http://wis-tns.weizmann.ac.il/search?";
            const string tsxHead = "SN      Host Galaxy      Date         R.A.    Decl.    Offset   Mag.   Disc. Ref.            SN Position         Posn. Ref.       Type  SN      Discoverer(s)";

            string weburl = MakeSearchQuery();

            WebClient client = new WebClient();
            string contents = client.DownloadString(url_tns_search + MakeSearchQuery());

            //Clean up the column headers so they can be used as XML item names
            string[] lines = contents.Split('\n');
            lines[0] = lines[0].Replace(" ", "_");
            lines[0] = lines[0].Replace("/", "");
            lines[0] = lines[0].Replace("(", "");
            lines[0] = lines[0].Replace(")", "");
            lines[0] = lines[0].Replace(".", "");
            lines[0] = lines[0].Replace("\"", "");

            //Split into rows and load the header line
            char[] csvSplit = new char[] { '\t' }; ;
            string[] headers = lines[0].Split(csvSplit, System.StringSplitOptions.None).Select(x => x.Trim('\"')).ToArray();

            //Duplicate the TSX photo input format -- i.e. make it the same as the Harvard IUA display format
            //  as it is copied into the clipboard
            //Create a text string to be filled in for the clipboard: Column headings and two newlines.
            string cbText = tsxHead + "\n\n";

            //create an xml working database
            XElement xml = new XElement("SuperNovaList");
            for (int line = 1; line < lines.Length; line++)
            {
                lines[line] = lines[line].Replace("\"", "");
                string[] entries = lines[line].Split(csvSplit, System.StringSplitOptions.None);
                XElement xmlItem = new XElement("SNEntry");
                for (int i = 0; i < headers.Length; i++)
                {
                    xmlItem.Add(new XElement(headers[i], entries[i]));
                }
                //fill in clipboard fields
                //Name
                cbText += xmlItem.Element("Name").Value.Replace("SN ", "").PadRight(8);
                //Name of the Host Galaxy, if any
                cbText += FitFormat(xmlItem.Element("Host_Name").Value, 17);
                //Discovery Date
                cbText += FitFormat(xmlItem.Element("Discovery_Date_UT").Value, 12);
                //Truncated RA and Dec for locale
                cbText += FitFormat(xmlItem.Element("RA").Value, 8);
                cbText += FitFormat(xmlItem.Element("DEC").Value, 12);
                //Offsets?
                cbText += "       ";  //offsets
                //Magnitude
                //cbText += xmlItem.Element("Discovery_Mag").Value.Substring(0, 4).PadRight(8); ;
                cbText += FitFormat(xmlItem.Element("Discovery_Mag").Value, 8);
                //Catelogs, truncated at 15 chars, if any
                cbText += FitFormat(xmlItem.Element("Ext_catalogs").Value, 15);
                //Actual RA/Dec location
                cbText += FitFormat(xmlItem.Element("RA").Value, 12);
                cbText += FitFormat(xmlItem.Element("DEC").Value, 14);
                //filler for Position Reference
                cbText += "                 ";
                //Supernova Type
                cbText += FitFormat(xmlItem.Element("Obj_Type").Value.Replace("SN ", ""), 6);
                //Supernova Name (as derived from entry name
                cbText += FitFormat(xmlItem.Element("Name").Value.Replace("SN ", ""), 8);
                //Discoverer
                cbText += FitFormat(xmlItem.Element("Discovering_Groups").Value, 12);
                //New Line
                cbText += "\n";
                xml.Add(xmlItem);
            }
            System.Windows.Forms.Clipboard.SetText(cbText, TextDataFormat.UnicodeText);
        }

        public static string MakeSearchQuery()
        {
            //Returns a url string for querying the TNS website

            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString["format"] = "tsv";
            queryString["name"] = "";
            queryString["date_start[date]"] = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            queryString["date_end[date]"] = DateTime.Now.ToString("yyyy-MM-dd");
            queryString["classified_sne"] = "1";
            queryString["public"] = "all";
            queryString["unclassified_at"] = "0";
            queryString["name_like"] = "0";
            queryString["isTNS_AT"] = "all";
            queryString["public"] = "all";
            queryString["unclassified_at"] = "0";
            queryString["classified_sne"] = "1";
            queryString["ra"] = "";
            queryString["decl"] = "";
            queryString["radius"] = "";
            queryString["coords_unit"] = "arcsec";
            queryString["groupid[]"] = "null";
            queryString["classifier_groupid[]"] = "null";
            queryString["type[]"] = "null";
            queryString["discovery_mag_min"] = "";
            queryString["discovery_mag_max"] = "";
            queryString["internal_name"] = "";
            queryString["redshift_min"] = "";
            queryString["redshift_max"] = "";
            queryString["spectra_count"] = "";
            queryString["discoverer"] = "";
            queryString["classifier"] = "";
            queryString["discovery_instrument[]"] = "";
            queryString["classification_instrument[]"] = "";
            queryString["hostname=&associated_groups[]"] = "null";
            queryString["& ext_catid"] = "";
            queryString["num_page"] = "50";
            queryString["display[redshift]"] = "1";
            queryString["display[hostname]"] = "1";
            queryString["display[host_redshift]"] = "1";
            queryString["display[source_group_name]"] = "1";
            queryString["display[classifying_source_group_name]"] = "1";
            queryString["display[discovering_instrument_name]"] = "0";
            queryString["display[classifing_instrument_name]"] = "0";
            queryString["programs_name]"] = "0";
            queryString["internal_name]"] = "1";
            queryString["display[isTNS_AT]"] = "0";
            queryString["display[public]"] = "1";
            queryString["displa[end_pop_period]"] = "0";
            queryString["display[pectra_count]"] = "1";
            queryString["display[discoverymag]"] = "1";
            queryString["display[Bdiscmagfilter]"] = "1";
            queryString["display[discoverydate]"] = "1";
            queryString["display[discoverer ]"] = "1";
            queryString["display[sources]"] = "0";
            queryString["display[bibcode]"] = "0";
            queryString["display[ext_catalogs]"] = "0";

            return queryString.ToString(); // Returns "key1=value1&key2=value2", all URL-encoded
        }

        public static string FitFormat(string entry, int slotSize)
        {
            //Returns a string which is the entry truncated to the slot Size, if necessary
            if (entry.Length > slotSize)
                return entry.Substring(0, slotSize - 1).PadRight(slotSize);
            else
                return entry.PadRight(slotSize);
        }

    }

}

