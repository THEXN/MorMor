﻿using Newtonsoft.Json;

namespace MomoAPI.Entities.Segment.DataModel;

public record File : BaseMessage
{
    [JsonProperty("file")]
    public string Data { get; set; }

    [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("file_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string FileId { get; set; }
}
