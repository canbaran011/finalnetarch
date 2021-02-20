using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Results
{
    public class Result : IResult
    {
        //private bool v1;
        //private string v2;

        public Result(bool success, string message) : this(success )
        {
            //Success = success; // Alttaki ctorda yapilir.
            Message = message;
        }
        public Result(bool success) // OVERLOADING
        {
            Success = success;
        }
        public bool Success { get; }

        public string Message { get; }
    }
}
