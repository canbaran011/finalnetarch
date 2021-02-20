using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Results
{
   public class SuccessResult : Result
    {
        public SuccessResult(string message):base(true , message) // Result a yani base e bunlari yolluyor
        {

        }
        public SuccessResult():base(true)
        {

        }
    }
}
