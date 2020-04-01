using Google.Cloud.Vision.V1;
using System.Collections.Generic;

namespace Shelfy.Models
{
    public class Vision
    {
        public class Gelen
        {
            public string Dosya { get; set; }
            public string URL { get; set; }
        }
        public class Sonuclar
        {
            public IReadOnlyList<EntityAnnotation> LabelAnnotations { get; set; }
            public IReadOnlyList<EntityAnnotation> TextAnnotations { get; set; }
            public SafeSearchAnnotation SafeSearchAnnotation { get; set; }
            public ImageProperties ImagePropertiesAnnotation { get; set; }
            public CropHintsAnnotation CropHintsAnnotation { get; set; }
            public IReadOnlyList<EntityAnnotation> FullTextAnnotation { get; set; }
            public WebDetection WebDetection { get; set; }
        }
        public class Kitaplar
        {
            public string Aranan { get; set; }
            /*public string Dene1 { get; set; }
            public string Dene2 { get; set; }*/
            public string Kitap { get; set; }
            public string Yazar { get; set; }
            public string Resim { get; set; }
            public string ISBN { get; set; }
            public string Yayinci { get; set; }
            //public string Dogruluk { get; set; }
        }
    }
}