using System;

namespace NJsonApi.Serialization.Representations
{
    public class Error
    {
        public Error()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public int Status { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Source { get; set; }
    }
}