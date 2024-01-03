using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DummyDataInsertion.Models;

const string URI_REQUEST = "https://dummyjson.com/products";
const string DUMMY_DATA = "dummydata.json";

HttpClient client = GetClient();
string apiData = await GetAllProductsFromDummyJsonApi(client);
WriteDataInJsonFile(apiData, DUMMY_DATA);
PrintAllProductsDescription(DUMMY_DATA);

HttpClient GetClient()
{
    HttpClient client = new HttpClient
    {
        BaseAddress = new Uri(URI_REQUEST)
    };
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));

    return client;
}

async Task<string> GetAllProductsFromDummyJsonApi(HttpClient client)
{
    try
    {
        using HttpResponseMessage response = await client.GetAsync(URI_REQUEST);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();
        return data;

    }
    catch (Exception e)
    {
        throw new Exception(e.Message);
    }
}


void WriteDataInJsonFile(string data,
                        string path)
{
    try
    {
        byte[] productBytes = Encoding.UTF8.GetBytes(data);

        using FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        fs.Write(productBytes, 0, productBytes.Length);
        Console.WriteLine("Os dados foram salvos");
    }
    catch (Exception e)
    {
        throw new Exception(e.Message);
    }

}


void PrintAllProductsDescription(string jsonFile)
{
    try
    {
        if (File.Exists(jsonFile))
        {
            var json = File.ReadAllText(jsonFile);
            using JsonDocument document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.TryGetProperty("products", out var productsFile))
            {
                var products = productsFile.Deserialize<Product[]>();
                if (products != null)
                {
                    foreach (var product in products)
                    {
                        Console.WriteLine(product.Description);
                    }
                }

            }

        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }

}
