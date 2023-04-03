# Iguana AWS Parameter Store

Iguana AWS Parameter Store is a cross-platform application for Windows that helps you interact with AWS Simple Systems Management (SSM) Parameter Store. With this application, you can retrieve parameters based on a specified path and generate a JSON file from the parameters.

## Prerequisites

Before using Iguana AWS Parameter Store, ensure you have the following installed:

- [.NET 7.0 Runtime](https://dotnet.microsoft.com/download/dotnet/7.0/runtime) or later
- [AWS CLI](https://aws.amazon.com/cli/)

## AWS Configuration

To use Iguana AWS Parameter Store, you need to have valid AWS credentials configured on your system. You can either use the AWS CLI or manually create the required configuration files.

### Using AWS CLI

1. Install the [AWS CLI](https://aws.amazon.com/cli/) for your operating system.
2. Open a command prompt and run the following command:

```aws configure```

3. Follow the prompts to enter your AWS Access Key ID, AWS Secret Access Key, default region, and default output format.

### Manual Configuration
Locate your AWS credentials file. The default location is:

Windows: %USERPROFILE%\.aws\credentials
If the file doesn't exist, create it, and add the following content:

```
[default]
aws_access_key_id = YOUR_AWS_ACCESS_KEY_ID
aws_secret_access_key = YOUR_AWS_SECRET_ACCESS_KEY
```

Replace `YOUR_AWS_ACCESS_KEY_ID` and `YOUR_AWS_SECRET_ACCESS_KEY` with your actual AWS Access Key ID and Secret Access Key.

### Retrieving Parameters and Generating JSON
Iguana AWS Parameter Store allows you to retrieve parameters based on a specified path and generate a JSON file. To do this, follow these steps:

1. Open the Iguana AWS Parameter Store application.
2. Enter the aws profile name to use for the AWS
3. Enter the desired path to fetch the parameters from AWS SSM.
4. Click on "Load Parameters" to fetch the parameters from the specified path.
5. The application will generate a JSON file containing the fetched parameters.

### Installation
To install the Iguana AWS Parameter Store application, follow these steps:

1. Download the latest release of the application from the `Releases` folder.
2. Windows: 
   - double-click on Iguana.AwsParameterStoreApp_VERSION_x86.msix to install the application. The application is at the moment in preview and signed by self signed certificate. Follow the instructions https://learn.microsoft.com/en-us/dotnet/maui/windows/deployment/publish-cli?view=net-maui-7.0#installing-the-app to install the certificate.

Now you can start using Iguana AWS Parameter Store to retrieve parameters and generate JSON files from the AWS SSM Parameter Store.

### Support
If you encounter any issues or have questions, feel free to open an issue on the project's GitHub page. We'll be happy to help you.