using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Completions;
using OpenAI_API;

namespace Floggr.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OpenAIController : ControllerBase
	{
		public OpenAIController() { }
		[HttpGet]
		[Route("UseOpenAI")]
		public async Task<IActionResult> UseOpenAI(string query)
		{
			string outputResult = "";
			var openai = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY_MealMaker"));
			CompletionRequest completionRequest = new CompletionRequest();
			completionRequest.Prompt = query;
			completionRequest.Model = OpenAI_API.Models.Model.DavinciText;
			completionRequest.MaxTokens = 1024;
			completionRequest.Temperature = 0.8;

			var completions = await openai.Completions.CreateCompletionAsync(completionRequest);

			foreach (var completion in completions.Completions)
			{
				outputResult += completion.Text;
			}

			return Ok(outputResult);

		}
	}
}
