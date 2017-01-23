using System.Collections.Generic;

namespace CodeMovement.EbcdicCompare.Models.Result
{
    public class OperationResult <T>
    {
        public OperationResult()
        {
            Messages = new List<string>();
            Result = default(T);
        }

        public T Result { get; set; }
        public List<string> Messages { get; set; }

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        public bool Successful
        {
            get { return Result != null && Messages.Count == 0; }
        }

        public static OperationResult<T> CreateResult(T result, params string[] messages)
        {
            return new OperationResult<T>
            {
                Result = result,
                Messages = (messages == null || messages.Length == 0) 
                    ? new List<string>() 
                    : new List<string>(messages)
            };
        }

        public static OperationResult<T> CreateResult(T result, List<string> messages)
        {
            return new OperationResult<T>
            {
                Result = result,
                Messages = messages
            };
        }
    }
}
