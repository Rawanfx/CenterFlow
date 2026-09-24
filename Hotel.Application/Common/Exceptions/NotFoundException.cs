using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace CenterFlow.Application.Common.Exceptions
{
    public class NotFoundException:Exception
    {
        public string Message { get; }
        public NotFoundException (string message):base(message)
        {
            this.Message = message;
        }
    }
}
