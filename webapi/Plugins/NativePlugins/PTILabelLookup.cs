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
using DocumentFormat.OpenXml.Wordprocessing;

namespace CopilotChat.WebApi.Plugins.NativePlugins;

public class PTILabelLookup
{
  [SKFunction, Description("Validate PTI Image Compliance and return the PTI barcode if detected")]
  public async Task<string> ValidatePTIImage(string filename)
  {
    string responseContent = string.Empty;
    string filePath = ("/Users/d0g0dj6/Documents/Projects/AI Assisted Claims/Hackathon/python/" +filename);
    Console.WriteLine("filenname:" + filePath);
    string url = "http://localhost:7071/api/barcodeCompliances?image=" +filePath;

    try
    {
      using (var client = new HttpClient())
      {
        using (var content = new MultipartFormDataContent())
        {
          // byte[] fileBytes = File.ReadAllBytes(filePath);
          // Console.WriteLine("bytes:" + fileBytes);
          // content.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "file", filename);
          Console.WriteLine("Before PostAsync:");

          var response = await client.GetAsync(url);
  
          Console.WriteLine("After PostAsync:" + response);

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
      Console.WriteLine(ex.Message);
      throw new Exception("An error occurred: " + ex.Message);     
    }
    return responseContent;
  }
  

  [SKFunction, Description("Get Product Details using the given PTI Number above")]
  public async Task<string> GetProductDetails(string ptinumber)
  {
    //string connectionString = "Server=tcp:your_server.database.windows.net,1433;Initial Catalog=your_database;Persist Security Info=False;User ID=your_username;Password=your_password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

    string connectionString = Environment.GetEnvironmentVariable("PRODUCT_DETAILS_DB");
    Console.WriteLine("DB Connection String : " + connectionString);
    connectionString="";
    
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
      connection.Open();
      string dbResponse = "";

      string sqlQuery = "SELECT PTI_NUM,ITEM_NBR,PRODUCT_NM, SUPPLIER_NAME,ROUTE_NO,TRAILER_NO,SEAL_NO,QUALITY_TIPS," +  
                        "ACCEPTABLE_IMAGE, UNACCEPTABLE_IMAGE_1, UNACCEPTABLE_IMAGE_2,UNACCEPTABLE_IMAGE_3, " +
                        "QUALITY_ISSUE_1, QUALITY_ISSUE_2, QUALITY_ISSUE_3 " +
                        " FROM PRODUCT_DETAILS WHERE PTI_NUM = '" + ptinumber + "'";

      using (SqlCommand command = new SqlCommand(sqlQuery, connection))
      {
        using (SqlDataReader reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
              dbResponse = dbResponse + "    " + String.Format("PTI Number: {0}, Item number: {1}, Product name: {2}, Supplier Name: {3}, Route Number: {4}, Trailer Number: {5}, Seal Number: {6}, Quality Tips: {7}", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6], reader[7]); 
          }
          Console.WriteLine(dbResponse);
        }
      }
      return "The details for the given PTI number are:"+dbResponse;
    }
  }

  static string EncodeImageToBase64(string imagePath)
  {
    byte[] imageBytes = File.ReadAllBytes(imagePath);
    return Convert.ToBase64String(imageBytes);
  }
}
