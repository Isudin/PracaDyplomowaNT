using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PracaDyplomowaNT.Shipx.Model
{
    public class Parcel
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("template")] public string Template { get; set; }
        [JsonProperty("dimensions")] public Dimensions Dimensions { get; set; }
        [JsonProperty("weight")] public Weight Weight { get; set; }
        [JsonProperty("tracking_number")] public string TrackingNumber { get; set; }
        [JsonProperty("is_non_standard")] public bool IsNonStandard { get; set; }
    }

    public class Dimensions
    {
        [JsonProperty("length")] public string Length { get; set; }
        [JsonProperty("width")] public string Width { get; set; }
        [JsonProperty("height")] public string Height { get; set; }
        [JsonProperty("unit")] public string Unit { get; set; }
    }

    public class Weight
    {
        [JsonProperty("amount")] public string Amount { get; set; }
        [JsonProperty("unit")] public string Unit { get; set; }
    }
}
