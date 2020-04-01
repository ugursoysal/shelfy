using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using IBM.WatsonDeveloperCloud.PersonalityInsights.v3;
using IBM.WatsonDeveloperCloud.PersonalityInsights.v3.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Shelfy.Models;

namespace Shelfy.Controllers
{
    public class WatsonController : Controller
    {
        [Route("api/[controller]")]
        [HttpPost]
        public ActionResult Sonuc([FromBody]WatsonPI.Gelen value)
        {
            try
            {
                if (value.Body != null && value.Body.Length > 0)

                {
                    var credential = GoogleCredential.FromFile("C:\\Shelff-4e74dc12eb0d.json");
                    TranslationClient client = TranslationClient.Create(credential);
                    var response = client.TranslateText(value.Body, "en");

                    string contentToProfile = response.TranslatedText;

                    PersonalityInsightsService _personalityInsights = new PersonalityInsightsService("6480d872-d50e-46bc-84e9-c24da9ea920b", "wWUDvxn3NdGw", "2017-10-13");

                    Content content = new Content()
                    {
                        ContentItems = new List<ContentItem>()
                        {
                            new ContentItem()
                            {
                                Contenttype = ContentItem.ContenttypeEnum.TEXT_PLAIN,
                                Language = ContentItem.LanguageEnum.EN,
                                Content = contentToProfile
                            }
                        }
                    };

                    var result = _personalityInsights.Profile(content, "text/plain", acceptLanguage: "application/json", rawScores: true, consumptionPreferences: true, csvHeaders: true);
                    WatsonPI.Sonuclar sonuc = new WatsonPI.Sonuclar { Personality = result.Personality, Needs = result.Needs, Values = result.Values };
                    return Json(sonuc);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [Route("api/[controller]/json")]
        [HttpPost]
        public ActionResult JSonuc([FromBody]WatsonPI.Gelen value)
        {
            if (value.Body != null && value.Body.Length > 0)
                try
                {
                    var credential = GoogleCredential.FromFile("C:\\Shelff-4e74dc12eb0d.json");
                    TranslationClient client = TranslationClient.Create(credential);
                    var response = client.TranslateText(value.Body, "en");

                    string contentToProfile = response.TranslatedText;
                    PersonalityInsightsService _personalityInsights = new PersonalityInsightsService("6480d872-d50e-46bc-84e9-c24da9ea920b", "wWUDvxn3NdGw", "2017-10-13");

                    Content content = new Content()
                    {
                        ContentItems = new List<ContentItem>()
                    {
                        new ContentItem()
                        {
                            Contenttype = ContentItem.ContenttypeEnum.TEXT_PLAIN,
                            Language = ContentItem.LanguageEnum.EN,
                            Content = contentToProfile
                        }
                    }
                    };

                    var result = _personalityInsights.Profile(content, "text/plain", acceptLanguage: "application/json", rawScores: true, consumptionPreferences: true, csvHeaders: true);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return NotFound(ex);
                }
            else
            {
                return BadRequest();
            }
        }
    }
}
