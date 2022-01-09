using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
namespace Bot
{
    public record QA(string Question, Answer Answer);

    public record Answer(AnswerType Type)
    {
        public string? Text { get; init; }
        public string? DocumetUrl { get; init; }
        public Venue? VenueData { get; init; }
        public Location? LocationData { get; init; }
    };

    public enum AnswerType
    {
        Text,
        Photo,
        Venue,
        Location,
        Document
    }

    public record Venue(string Title, string Address, float Latitude, float Longtitude);

    public record Location(float Latitude, float Longtitude);
}
/*
 * new scheme
----answer scheme---
Document: 'document' field in json and in message
text formating: *bold* `info`

*/