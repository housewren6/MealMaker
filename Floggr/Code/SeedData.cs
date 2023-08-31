using Floggr.Data;
using Newtonsoft.Json;
using RootFoundationFoods;
using Floggr.Models;

namespace Floggr.Code
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (
                var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<FloggrContext>();

                //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
                //look for foundation foods
                if (context.RootFoundationFoods.Any())
                {
                    return; //DB is seeded
                }
				var jsonString = File.ReadAllText("./Data/foundationDownload.json");
				if (jsonString != null)
                {
                    Root foundationFoods = JsonConvert.DeserializeObject<Root>(jsonString);

                    context.RootFoundationFoods.AddRange(foundationFoods);
                    context.SaveChanges();
                }

            }
        }
    }
}