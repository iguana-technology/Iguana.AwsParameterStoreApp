using Amazon;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Newtonsoft.Json.Linq;

namespace Iguana.AwsParameterStoreApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void LoadParametersButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            string path = PathEntry.Text;
            if(string.IsNullOrEmpty(path))
            {
                await DisplayAlert("Error", "Please provide path.", "OK");
                return;
            }

            var ssmClient = await CreateClientAsync();
            if(ssmClient == null)
            {
                return;
            }

            JObject resultJson = await GetParametersByPathAsync(ssmClient, path);
            JsonEditor.Text = resultJson.ToString(Newtonsoft.Json.Formatting.Indented);
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error", $"Error getting parameters", "OK");
        }
    }

    private async void OnUpdateOrCreateParameterButton_Clicked(object sender, EventArgs e)
    {
        string parameterName = ParameterNameEntry.Text;
        string parameterValue = ParameterValueEntry.Text;
        bool isSecure = IsSecureCheckBox.IsChecked;

        try
        {
            if(string.IsNullOrEmpty(parameterName) || string.IsNullOrEmpty(parameterValue))
            {
                await DisplayAlert("Error", "Please provide both the parameter name and value.", "OK");
                return;
            }

            var ssmClient = await CreateClientAsync();
            if(ssmClient == null)
            {
                return;
            }

            await UpdateOrCreateParameterAsync(ssmClient, parameterName, parameterValue, isSecure);
            await DisplayAlert("Success", $"Parameter '{parameterName}' has been updated/created.", "OK");
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error", $"Error updating/creating parameter '{parameterName}': {ex.Message}", "OK");
        }
    }

    private async void OnDeleteParameterButton_Clicked(object sender, EventArgs e)
    {
        var parameterName = ParameterNameEntry.Text;

        if(string.IsNullOrEmpty(parameterName))
        {
            await DisplayAlert("Error", "Please enter a parameter name.", "OK");
            return;
        }

        var confirm = await DisplayAlert("Delete Parameter", $"Are you sure you want to delete the parameter '{parameterName}'?", "Yes", "No");
        if(!confirm) return;

        try
        {
            var client = await CreateClientAsync();
            var request = new DeleteParameterRequest
            {
                Name = parameterName
            };
            await client.DeleteParameterAsync(request);

            await DisplayAlert("Success", $"The parameter '{parameterName}' has been deleted.", "OK");
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred while deleting the parameter: {ex.Message}", "OK");
        }
    }

    private async Task<AmazonSimpleSystemsManagementClient?> CreateClientAsync()
    {
        string region = RegionEntry.Text;
        string profile = ProfileEntry.Text;

        if(string.IsNullOrEmpty(region) || string.IsNullOrEmpty(profile))
        {
            await DisplayAlert("Error", "Please provide region and profile", "OK");
            return null;
        }

        var awsCredentials = new StoredProfileAWSCredentials(profile);
        return new AmazonSimpleSystemsManagementClient(awsCredentials, RegionEndpoint.GetBySystemName(region));
    }

    private async Task UpdateOrCreateParameterAsync(AmazonSimpleSystemsManagementClient ssmClient, string parameterName, string parameterValue, bool isSecure)
    {
        var request = new PutParameterRequest
        {
            Name = parameterName,
            Value = parameterValue,
            Type = isSecure ? ParameterType.SecureString : ParameterType.String,
            Overwrite = true
        };

        try
        {
            await ssmClient.PutParameterAsync(request);
            Console.WriteLine($"Parameter '{parameterName}' has been updated/created.");
        }
        catch(AmazonSimpleSystemsManagementException ex)
        {
            Console.WriteLine($"Error updating/creating parameter '{parameterName}': {ex.Message}");
        }
    }

    private static async Task<JObject> GetParametersByPathAsync(AmazonSimpleSystemsManagementClient ssmClient, string path)
    {
        var request = new GetParametersByPathRequest
        {
            Path = path,
            Recursive = true,
            WithDecryption = true
        };

        JObject resultJson = new JObject();

        do
        {
            var response = await ssmClient.GetParametersByPathAsync(request);
            foreach(var parameter in response.Parameters)
            {
                AddParameterToJObject(resultJson, parameter.Name, parameter.Value);
            }
            request.NextToken = response.NextToken;
        } while(!string.IsNullOrEmpty(request.NextToken));

        return resultJson;
    }

    private static void AddParameterToJObject(JObject jsonObject, string path, JToken value)
    {
        string[] parts = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        JObject currentObject = jsonObject;

        for(int i = 0; i < parts.Length - 1; i++)
        {
            if(!currentObject.ContainsKey(parts[i]))
            {
                currentObject[parts[i]] = new JObject();
            }
            currentObject = (JObject)currentObject[parts[i]];
        }

        currentObject[parts[^1]] = value;
    }
}

