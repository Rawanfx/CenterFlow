using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace CenterFlow.Application.Common.Exceptions
{
    public class NotFoundException:AppException
    {
        public NotFoundException (string message):base(message,404)
        {
        }
    }
}
