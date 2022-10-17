using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Helpers
{
    public class Envelope<T> where T : class
    {
        public List<T> Collection { get; set; }
        public ErrorLogger Logger { get; set; }
        public Envelope()
        {
            Collection = new List<T>();
            Logger = new ErrorLogger();
        }        
    }
    public class ErrorLogger
    {
        public List<Error> Errors { get; set; }
        public bool HasErrors
        {
            get
            {
                return (Errors.Count > 0);
            }
            private set { HasErrors = value; }
        }
        public ErrorLogger()
        {
            Errors = new List<Error>();
        }
        public void MergeErrors(ErrorLogger otherLogger)
        {
            foreach (var othererror in otherLogger.Errors)
            {
                Errors.Add(othererror);
            }
        }
        public void AddError(string message, string controller)
        {
            Error err = new Error()
            {
                Message = message,
                Controller = controller
            };
            Errors.Add(err);
        }
    }
    public class Error
    {
        public string Message { get; set; }
        public string? Controller { get; set; }
    }
}
