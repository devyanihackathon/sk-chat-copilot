// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Microsoft.Data.SqlClient;


namespace CopilotChat.WebApi.Plugins.NativePlugins;

public class PTILabelLookup
{

  [SKFunction, Description("Validate PTI Image")]
  public async Task<string> ValidatePTIImage(string filename)
  {
    string responseContent = string.Empty;
    string filePath = "C:\\Images\\" + filename;
    string url = "";

    try
    {
      using (var client = new HttpClient())
      {
        using (var content = new MultipartFormDataContent())
        {
          byte[] fileBytes = File.ReadAllBytes(filePath);
          content.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "file", "filename");

          var response = await client.PostAsync(url, content);

          if (response.IsSuccessStatusCode)
          {
            responseContent = await response.Content.ReadAsStringAsync();
          }
          else
          {
            throw new Exception("Error occurred. Status code: " + response.StatusCode);
          }
        }
      }
    }
    catch (Exception ex)
    {
      throw new Exception("An error occurred: " + ex.Message);
    }
    return responseContent;
  }


  [SKFunction, Description("Get Product Details using PTI Number")]
  public async Task<string> GetProductDetails(string ptinumber)
  {
    string connectionString = "Server=tcp:your_server.database.windows.net,1433;Initial Catalog=your_database;Persist Security Info=False;User ID=your_username;Password=your_password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
      connection.Open();

      using (SqlCommand command = new SqlCommand("SELECT * FROM your_table", connection))
      {
        using (SqlDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            Console.WriteLine(String.Format("{0}, {1}", reader[0], reader[1]));
          }
        }
      }
      return "";
    }
  }

}

