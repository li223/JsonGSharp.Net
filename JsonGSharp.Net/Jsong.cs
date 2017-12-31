using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace JsonGSharp.Net
{
    public class JsongImageObject
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this).ToString();
        }

        public static JsongImageObject JsonToJsongObject(string json)
        {
            return JsonConvert.DeserializeObject<JsongImageObject>(json);
        }

        public static JsongImageObject ImageToJsongObject(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(":foxbot::mademe::dothis:");
            var img = Image.FromFile(path) as Bitmap;
            var layer = new JsongLayer()
            {
                DefaultColor = new JsongColor()
                {
                    Red = 0,
                    Green = 0,
                    Blue = 0,
                    Alpha = 255
                }
            };
            var pixels = new List<JsongPixel>();
            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    var pixel = img.GetPixel(x, y);
                    pixels.Add(new JsongPixel()
                    {
                        Color = new JsongColor()
                        {
                            Red = pixel.R,
                            Green = pixel.G,
                            Blue = pixel.B,
                            Alpha = pixel.A
                        },
                        Position = new JsongPixelPosition()
                        {
                            X = x,
                            Y = y
                        },
                        Comment = "?whatdo?"
                    });
                }
            }
            layer.Pixels = pixels;
            var obj = new JsongImageObject()
            {
                Layers = new List<JsongLayer>(1) { layer },
                Size = new JsongImageSize()
                {
                    Height = img.Height,
                    Width = img.Width
                },
                Transparency = pixels.Any(x => x.Color.Alpha == 0),
                Comment = "?whatdo?",
                Version = "1.0"
            };
            return obj;
        }

        public static Bitmap JsongObjectToBitmap(JsongImageObject obj)
        {
            var map = new Bitmap(obj.Size.Width, obj.Size.Height);
            foreach(var layer in obj.Layers)
            {
                for(var y = 0; y < obj.Size.Height; y++)
                {
                    for (var x = 0; x < obj.Size.Width; x++)
                    {
                        var pixel = layer.Pixels.First(a => a.Position.X == x && a.Position.Y == y);
                        var color = Color.FromArgb(pixel.Color.Alpha, pixel.Color.Red, pixel.Color.Green, pixel.Color.Blue);
                        map.SetPixel(x, y, color);
                    }
                }
            }
            return map;
        }

        [JsonProperty("version")]
        public string Version { get; internal set; }

        [JsonProperty("comment")]
        public string Comment { get; internal set; }

        [JsonProperty("transparency")]
        public bool Transparency { get; internal set; }

        [JsonProperty("size")]
        public JsongImageSize Size { get; internal set; }

        [JsonProperty("layers")]
        public List<JsongLayer> Layers { get; internal set; }
    }

    public class JsongLayer
    {
        [JsonProperty("default_color")]
        public JsongColor DefaultColor { get; internal set; }
        [JsonProperty("pixels")]
        public List<JsongPixel> Pixels { get; internal set; }
    }

    public class JsongPixel
    {
        [JsonProperty("position")]
        public JsongPixelPosition Position { get; internal set; }
        [JsonProperty("color")]
        public JsongColor Color { get; internal set; }
        [JsonProperty("comment")]
        public string Comment { get; internal set; }
    }

    public class JsongColor
    {
        [JsonProperty("red")]
        public int Red { get; internal set; }
        [JsonProperty("green")]
        public int Green { get; internal set; }
        [JsonProperty("blue")]
        public int Blue { get; internal set; }
        [JsonProperty("alpha")]
        public int Alpha { get; internal set; }
    }

    public class JsongPixelPosition
    {
        [JsonProperty("x")]
        public int X { get; internal set; }
        [JsonProperty("y")]
        public int Y { get; internal set; }
    }

    public class JsongImageSize
    {
        [JsonProperty("height")]
        public int Height { get; internal set; }
        [JsonProperty("width")]
        public int Width { get; internal set; }
    }
}
