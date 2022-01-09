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
        public string? DocumentUrl { get; init; }
        public Venue? VenueData { get; init; }
        public Location? LocationData { get; init; }
        public Contact? ContactData { get; init; }
    };

    public enum AnswerType
    {
        Text,
        Venue,
        Location,
        Document,
        Contact
    }

    public record Venue(string Title, string Address, float Latitude, float Longitude);

    public record Location(float Latitude, float Longitude);

    public record Contact(string FirstName, string LastName, string PhoneNumber);
}