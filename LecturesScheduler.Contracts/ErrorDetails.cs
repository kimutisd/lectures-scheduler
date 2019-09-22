using Newtonsoft.Json;

namespace LecturesScheduler.Contracts
{
    public class ErrorDetails
    {
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
