// See https://aka.ms/new-console-template for more information
using System.Net;

//Collection of projects and explorer urls
Dictionary<string, string> baseUrls = new Dictionary<string, string>();

//All types of output data
List<string> UniqueAddressOutput = new List<string>();
List<string> DailyTransactions = new List<string>();
List<string> VerifiedContracts = new List<string>();
List<string> ActiveUsers = new List<string>();

//Complete list of URLs identified
string url1 = "https://etherscan.io/chart";
string url2 = "https://www.bscscan.com/chart";
string url3 = "https://snowtrace.io/chart";
string url4 = "https://polygonscan.com/chart";
string url5 = "https://cronoscan.com/chart";
string url6 = "https://ftmscan.com/chart";
string url7 = "https://arbiscan.io/chart";
string url8 = "https://optimistic.etherscan.io/chart";
string url9 = "https://aurorascan.dev/chart";

//Pairing of the blockchain and one available tracker
baseUrls.Add("ETH", url1);
baseUrls.Add("BSC", url2);
baseUrls.Add("AVAX", url3);
baseUrls.Add("MATIC", url4);
baseUrls.Add("CRO", url5);
baseUrls.Add("FANTOM", url6);
baseUrls.Add("ARBITRUM", url7);
baseUrls.Add("OPTIMISM", url8);
baseUrls.Add("AURORA", url9);


//Specific URLs to get data from
string uniqueAddressUrl = "/address?output=csv";
string dailyTransactionsUrl = "/tx?output=csv";
string verifiedContractsUrl = "/verified-contracts?output=csv";
string activeAddressUrl = "/active-address?output=csv";



//Combine the data and prefix the blockchain name
void CombineFiles(string url, string pattern, List<string> output)
{
    if (url.Contains(pattern))
    {
        string[] tmp = File.ReadAllLines(url);
        for (int i = 1; i < tmp.Length; i++)
        {
            string[] fileParts = url.Split("\\");
            string last = fileParts.LastOrDefault().Split("-").FirstOrDefault();
            output.Add((last + "," + tmp[i]).Replace("\"", ""));
        }
    }
}


Console.WriteLine("Data will be collected and combined");
using (var client = new WebClient())
{
    foreach (KeyValuePair<string, string> pair in baseUrls)
    {
        //Get each file for each chain
        client.DownloadFile(pair.Value + uniqueAddressUrl, pair.Key + "-addresses.csv");
        client.DownloadFile(pair.Value + dailyTransactionsUrl, pair.Key + "-dailytx.csv");
        client.DownloadFile(pair.Value + verifiedContractsUrl, pair.Key + "-verifiedcontracts.csv");
        client.DownloadFile(pair.Value + activeAddressUrl, pair.Key + "-dailyActive.csv");

    }
}

//Add a header to each file
DailyTransactions.Add("Network, Date(UTC),UnixTimeStamp,Value");
UniqueAddressOutput.Add("Network, Date(UTC),UnixTimeStamp,Value");
VerifiedContracts.Add("Network, Date(UTC),UnixTimeStamp,Value");
ActiveUsers.Add("Network, Date(UTC),UnixTimeStamp,Value");

//Combine data sets for all networks
foreach (string url in Directory.GetFiles(Environment.CurrentDirectory))
{
    CombineFiles(url, "-addresses.csv", UniqueAddressOutput);
    CombineFiles(url, "-dailytx.csv", DailyTransactions);
    CombineFiles(url, "-addresses.csv", UniqueAddressOutput);
    CombineFiles(url, "-dailyActive.csv", ActiveUsers);
}


//Write the combined files to disk
File.WriteAllLines("DailyActive-Combined.csv", ActiveUsers);
File.WriteAllLines("Addresses-Combined.csv", UniqueAddressOutput);
File.WriteAllLines("VerifiedContracts-Combined.csv", VerifiedContracts);
File.WriteAllLines("DailyTX-Combined.csv    ", DailyTransactions);

//Inform complete
Console.WriteLine("Process has completed.");


