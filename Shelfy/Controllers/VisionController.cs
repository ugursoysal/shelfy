using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Grpc.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Shelfy.Models;

namespace Shelfy.Controllers
{
    public class VisionController : Controller
    {
        readonly string customSearchApiID = "015847366241823599244:pam31f8whz8";
        readonly string customSearchApiKey = "AIzaSyCLpJcSsIdRGJuga5pVvSH1KitTfXvEdac";
        //https://www.googleapis.com/customsearch/v1?q={querystring}&cx=015847366241823599244:pam31f8whz8&key=AIzaSyCLpJcSsIdRGJuga5pVvSH1KitTfXvEdac&fields=spelling/correctedQuery,searchInformation/totalResults,items/pagemap/book
        public int Puanla(Vision.Kitaplar kitapmi)
        {
            var kList = kitapmi.Aranan.Split(" ");
            foreach (var k in kList)
            {
                if (kitapmi.Kitap.ToLower()
                    .Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace("ç", "c").Replace("ö", "o").Replace("ı", "i").Contains(k.ToLower()
                    .Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace("ç", "c").Replace("ö", "o").Replace("ı", "i"))) return 1;
            }
            return 0;
        }
        public Vision.Kitaplar Duzelt(string duzel) // ?lr=lang_tr
        {
            try
            {
                if (duzel == null || duzel.Length < 6) return null;
                string encode = System.Web.HttpUtility.UrlEncode(duzel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/customsearch/v1?lr=lang_tr&q=" + encode + "&cx=" + customSearchApiID + "&key=" + customSearchApiKey + "&fields=spelling/correctedQuery,searchInformation/totalResults,items/pagemap/book");//&fields=spelling/correctedQuery,searchInformation/totalResults,items/pagemap/book
                request.Method = "GET";
                request.KeepAlive = true;
                request.ContentType = "application/json; charset=utf-8";

                HttpWebResponse desponse = (HttpWebResponse)request.GetResponse();

                string myResponse = "";
                //myResponse = desponse.ToString();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(desponse.GetResponseStream()))
                {
                    myResponse = sr.ReadToEnd();
                }
                JObject o = JObject.Parse(myResponse);
                //string dogru = o["searchInformation"]["totalResults"].ToString();
                if (o["searchInformation"]["totalResults"].ToString() == "0") // probably wrong input
                {/*
                    try
                    {
                        if (o["spelling"]["correctedQuery"] != null)
                        {
                            return duzel + " => " + o["spelling"]["correctedQuery"].ToString();
                        }
                    }
                    catch
                    {
                        return null;
                    }*/
                    return null;
                }
                else
                {
                    try
                    {
                        if (o["spelling"]["correctedQuery"] != null)
                        {
                            // return duzel + " => " + o["spelling"]["correctedQuery"].ToString() + " => results:" + o["searchInformation"]["totalResults"].ToString();
                            return Duzelt(o["spelling"]["correctedQuery"].ToString());
                        }
                    }
                    catch
                    { }
                    Vision.Kitaplar yeni = new Vision.Kitaplar
                    {
                        Aranan = duzel
                    };
                    try { yeni.Kitap = o["items"][0]["pagemap"]["book"][0]["name"].ToString(); } catch { yeni.Kitap = null; }
                    try
                    {
                        yeni.Yazar = o["items"][0]["pagemap"]["book"][0]["author"].ToString();
                        if (yeni.Yazar
                            .ToLower()
                            .Replace(" ", "").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace("ç", "c").Replace("ö", "o").Replace("ı", "i").Replace("[^a-z]", "")

                            .Contains(duzel.ToLower()
                            .Replace(" ", "").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace("ç", "c").Replace("ö", "o").Replace("ı", "i")
                            .Replace("prof", "")//,StringComparison.CurrentCultureIgnoreCase)
                            .Replace("psikolog", "")//, StringComparison.CurrentCultureIgnoreCase)
                            .Replace("[^a-z]", "")))/*, StringComparison.CurrentCultureIgnoreCase) > -1*/ { return null; } // probably wrong results if author name is given only (yeni.Yazar.Length == duzel.Length)
                    }
                    catch { yeni.Yazar = null; if (yeni.Kitap == null) return null; } // sonuç yok
                    try { yeni.Resim = o["items"][0]["pagemap"]["book"][0]["image"].ToString(); } catch { yeni.Resim = null; }
                    try
                    {
                        yeni.Yayinci = o["items"][0]["pagemap"]["book"][0]["publisher"].ToString();
                        if (yeni.Yayinci
                            .ToLower()
                            .Replace(" ", "").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace("ç", "c").Replace("ö", "o").Replace("ı", "i").Replace("[^a-z]", "")

                            .Contains(duzel.ToLower()
                            .Replace(" ", "").Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s").Replace("ç", "c").Replace("ö", "o").Replace("ı", "i")
                            .Replace("prof", "")//,StringComparison.CurrentCultureIgnoreCase)
                            .Replace("psikolog", "")//, StringComparison.CurrentCultureIgnoreCase)
                            .Replace("[^a-z]", ""))) { return null; } // probably wrong results if publisher is given only (yeni.Yayinci.Length == duzel.Length)
                    }
                    catch { yeni.Yayinci = null; }
                    try { yeni.ISBN = o["items"][0]["pagemap"]["book"][1]["isbn"].ToString(); } catch { yeni.ISBN = null; }
                    //yeni.Dogruluk = dogru;
                    return yeni;
                }
            }
            catch { return null; }

        }
        // POST api/values
        //https://www.googleapis.com/customsearch/v1?q={query}&cx=apiID&key=apiKey &?lr=lang_tr
        [Route("api/[controller]")]
        [HttpPost]
        public ActionResult Sonuc([FromBody]Vision.Gelen file)
        {
            try
            {
                using (WebClient cli = new WebClient())
                {
                    cli.DownloadFile(file.URL, "Resimler/" + file.Dosya);
                }

                var credential = GoogleCredential.FromFile("C:\\Shelff-4e74dc12eb0d.json")
                    .CreateScoped(ImageAnnotatorClient.DefaultScopes);

                var channel = new Grpc.Core.Channel(
                        ImageAnnotatorClient.DefaultEndpoint.ToString(),
                        credential.ToChannelCredentials());

                var client = ImageAnnotatorClient.Create(channel);
                // var image = Google.Cloud.Vision.V1.Image.FromUri(file.URL);
                string path = Path.Combine("Resimler", file.Dosya); // :(
                var image = Google.Cloud.Vision.V1.Image.FromFile(path);
                var kesponse = client.DetectDocumentText(image);
                string wt;
                var sonuc = kesponse.Text.Split("\n");
                Vision.Kitaplar pp = null;
                List<Vision.Kitaplar> que = new List<Vision.Kitaplar>();
                Regex rgx = new Regex("[^a-zA-Z0-9İÜĞŞÇÖıüğşçö ]");
                //Regex re = new Regex(@"\s*\b\w{1,3}\b\s*");
                foreach (var p in sonuc)
                {
                    wt = rgx.Replace(p, "");
                    wt = String.Join(" ", wt.Split(' ').Where(x => x.Length > 2).ToArray());
                    pp = Duzelt(wt);
                    if (pp != null && Puanla(pp) != 0 && que.Where(s => s.ISBN == pp.ISBN).Count() == 0) que.Add(pp);
                }
                return Json(que);

            }
            catch (Exception ex) // düzenlenecek
            {
                return BadRequest(ex);
            }
        }

        // POST api/values
        [Route("api/[controller]/json")]
        [HttpPost]
        public ActionResult JSonuc([FromBody]byte[] file)
        {
            try
            {/*
                    file.
                    string path = Path.Combine(Server.MapPath("~/App_Data/Images"),
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);*/
                var credential = GoogleCredential.FromFile("C:\\Shelff-4e74dc12eb0d.json")
                .CreateScoped(ImageAnnotatorClient.DefaultScopes);
                var channel = new Grpc.Core.Channel(
                    ImageAnnotatorClient.DefaultEndpoint.ToString(),
                    credential.ToChannelCredentials());
                var client = ImageAnnotatorClient.Create(channel);
                var image = Google.Cloud.Vision.V1.Image.FromBytes(file);

                Vision.Sonuclar sonuc = new Vision.Sonuclar { LabelAnnotations = client.DetectLabels(image), TextAnnotations = client.DetectText(image), SafeSearchAnnotation = client.DetectSafeSearch(image), ImagePropertiesAnnotation = client.DetectImageProperties(image), CropHintsAnnotation = client.DetectCropHints(image), FullTextAnnotation = client.DetectText(image), WebDetection = client.DetectWebInformation(image) };
                return Json(sonuc);

            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }
    }
}
