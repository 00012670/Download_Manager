using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace _00012670
{
    // Define a service contract for the download service.
    [ServiceContract]
    public interface IDownloadService
    {
        // Define an operation contract for downloading a file.
        [OperationContract]
        Task<bool> DownloadFile(string downloadUrl, string targetPath, int downloadId);

        // Define an operation contract for getting the progress of a download.
        [OperationContract]
        int GetDownloadProgress(int downloadId);

        // Define an operation contract for canceling a download.
        [OperationContract]
        void CancelDownload(int downloadId);

        // Define an operation contract for getting the remaining time of a download.
        [OperationContract]
        string GetRemainingTime(int downloadId);

        // Define an operation contract for pausing a download.
        [OperationContract]
        void PauseDownload(int downloadId);

        // Define an operation contract for resuming a download.
        [OperationContract]
        void ResumeDownload(int downloadId);
    }

    // Define a data contract for the progress of a download.
    [DataContract]
    public class DownloadProgress
    {
        // Define a data member for the URL of the download.
        [DataMember]
        public string Url { get; set; }

        // Define a data member for the percentage of the download.
        [DataMember]
        public int Percentage { get; set; }

        // Define a data member for the status of the download.
        [DataMember]
        public string Status { get; set; }

        // Define a data member for the error message of the download.
        [DataMember]
        public string ErrorMessage { get; set; }
    }

    // Define a data contract for a download request.
    [DataContract]
    public class DownloadRequest
    {
        // Define a data member for the URL of the download.
        [DataMember]
        public string Url { get; set; }

        // Define a data member for the target path of the download.
        [DataMember]
        public string TargetPath { get; set; }

        // Define a data member for the priority of the download.
        [DataMember]
        public DownloadPriority Priority { get; set; }

        // Define a data member for the error message of the download.
        [DataMember]
        public string ErrorMessage { get; set; }

        // Constructor for the download request.
        public DownloadRequest(string url, string targetPath, DownloadPriority priority)
        {
            Url = url;
            TargetPath = targetPath;
            Priority = priority;
        }

        // Define an enumeration for the download priority.
        public enum DownloadPriority
        {
            Low,
            Normal,
            High
        }
    }
}
