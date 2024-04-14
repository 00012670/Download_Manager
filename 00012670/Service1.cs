using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace _00012670
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public List<string> DownloadFiles(List<string> urls)
        {
            List<string> downloadedFiles = new List<string>();

            foreach (var url in urls)
            {
                // Download the file and add the local file path to the list
                string localFilePath = DownloadFile(url);
                downloadedFiles.Add(localFilePath);
            }

            return downloadedFiles;
        }

        private string DownloadFile(string url)
        {
            // Implement your file download logic here
            // For example, you might use WebClient.DownloadFile method
            // Return the local file path where the file has been downloaded
        }
    }

}
