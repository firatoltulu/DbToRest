using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DbToRest.Core.Exception
{
    public class DictionaryConvertException : System.Exception
    {
        public DictionaryConvertException(ICollection<ConvertProblem> problems)
            : this(CreateMessage(problems), problems)
        {
        }

        public DictionaryConvertException(string message, ICollection<ConvertProblem> problems)
            : base(message)
        {
            Problems = problems;
        }

        public DictionaryConvertException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ICollection<ConvertProblem> Problems { get; private set; }

        private static string CreateMessage(ICollection<ConvertProblem> problems)
        {
            var counter = 0;
            var builder = new StringBuilder();
            builder.Append("Could not convert all input values into their expected types:");
            builder.Append(Environment.NewLine);
            foreach (var prob in problems)
            {
                builder.AppendFormat("-----Problem[{0}]---------------------", counter++);
                builder.Append(Environment.NewLine);
                builder.Append(prob);
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
    }
}