using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GmailDelete
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials (token.json).
        static string[] Scopes = { GmailService.Scope.GmailModify };
        static string ApplicationName = "Gmail API Trash Example";

        static void Main(string[] args)
        {
            var service = GetGmailService();

            // Replace this with the sender you want to target
            string senderEmail = "no-reply@twitch.tv";
            
            TrashAllFromSender(service, senderEmail);
        }

        static GmailService GetGmailService()
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Gmail API service
            var service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }

        private static void TrashAllFromSender(GmailService service, string senderEmail)
        {
            // 1. Build search query
            string query = $"from:{senderEmail}";
            
            var allMessageIds = new List<string>();
            string pageToken = null;

            // 2. Find all message IDs
            do
            {
                var request = service.Users.Messages.List("me");
                request.Q = query;
                request.PageToken = pageToken;
                request.MaxResults = 100; // up to 500

                var response = request.Execute();
                if (response.Messages != null && response.Messages.Count > 0)
                {
                    allMessageIds.AddRange(response.Messages.Select(m => m.Id));
                }

                pageToken = response.NextPageToken;
            }
            while (!string.IsNullOrEmpty(pageToken));

            // 3. Send them to Trash
            if (allMessageIds.Count == 0)
            {
                Console.WriteLine($"No messages found from {senderEmail}");
                return;
            }

            Console.WriteLine($"Found {allMessageIds.Count} messages from {senderEmail} — moving them to Trash...");

            // Use BatchModify with the TRASH label
            const int batchSize = 1000;
            for (int i = 0; i < allMessageIds.Count; i += batchSize)
            {
                var chunk = allMessageIds.Skip(i).Take(batchSize).ToList();

                var batchModifyRequest = new BatchModifyMessagesRequest
                {
                    Ids = chunk,
                    AddLabelIds = new List<string> { "TRASH" }
                };

                service.Users.Messages.BatchModify(batchModifyRequest, "me").Execute();
            }

            Console.WriteLine($"Successfully moved {allMessageIds.Count} messages to Trash.");
        }
    }
}
