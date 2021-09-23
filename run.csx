#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

public class SiteMap
{
    public string Name {get; set;}
    public string Url {get;set;}
}
public class MySites 
{
    public IList<SiteMap> Sites {get; set;} 
}


public static async Task<IActionResult> Run(HttpRequest req, string userpath, ILogger log, ExecutionContext exc)
{
    log.LogInformation("C# HTTP trigger function processed a request.");    
    string path = exc.FunctionDirectory;
    log.LogInformation($"Current working directory is {path}.");
    string sitesFile = $"{path}\\Sites.json";
    string text;
    try
    {
        using (var sr = new StreamReader(sitesFile))
        {
            text = await sr.ReadToEndAsync();
        }
    }
    catch (FileNotFoundException ex)
    {
        log.LogError($"{ex.Message}");
        return new BadRequestObjectResult(new { message = "400 Bad Request", currentDate = DateTime.Now });
    }
    
    MySites mySites = JsonConvert.DeserializeObject<MySites>(text);
    var result = mySites.Sites?.Where(site => site.Name.ToLower() == userpath.ToLower()).FirstOrDefault();

    if(result == null)
    {
        log.LogInformation($"Result was null.");
        return new RedirectResult("https://www.fandango.com", false);
    }
    else
    {
        log.LogInformation($"Result {result.Name}.");
        return new RedirectResult(result.Url, false);
    }
}
