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
        string region = RegionEntry.Text;
        string profile = ProfileEntry.Text;
        string path = PathEntry.Text;

        var awsCredentials = new StoredProfileAWSCredentials(profile);
        var ssmClient = new AmazonSimpleSystemsManagementClient(awsCredentials, RegionEndpoint.GetBySystemName(region));

        JObject resultJson = await GetParametersByPathAsync(ssmClient, path);
        JsonEditor.Text = resultJson.ToString(Newtonsoft.Json.Formatting.Indented);
    }

    static async Task<JObject> GetParametersByPathAsync(AmazonSimpleSystemsManagementClient ssmClient, string path)
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

    static void AddParameterToJObject(JObject jsonObject, string path, JToken value)
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

