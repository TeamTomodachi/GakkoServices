using System;

namespace GakkoServices.Core.Messages
{
    public class ResultMessage
    {
        public enum Status
        {
            Ok,
            Error,
        }

        public Status status;

        public object data;
    }
}
