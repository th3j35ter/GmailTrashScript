## Gmail Trash Script
This script uses the Gmail API to move emails from a specific sender to the Trash.

### Prerequisites
- .NET 8.0 or higher
- Google Cloud credentials (`credentials.json`)

### How to Set Up
1. Clone the repository.
2. Go to the [Google Cloud Console](https://console.cloud.google.com/) and create a project.
3. Enable the Gmail API and generate OAuth 2.0 credentials.
4. Go to your Google Cloud Project, and go to Audience, and add the email you want to Test Users.
5. Place the `credentials.json` file in the root directory.
6. Build and run the project.
7. Accept the warning screen and enter the email address of the account you wish to move all the emails from to Trash.
8. Wait until the message in the console indicates that it has completed. It will indicate how many emails were moved.
