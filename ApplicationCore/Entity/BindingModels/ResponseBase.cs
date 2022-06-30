using System.Collections.Generic;

namespace Farsica.Template.Entity.BindingModels
{
    public class ResponseBase
    {
        public List<Error> Errors { get; set; }

        public struct Error
        {
            public string Message { get; set; }
            public string Code { get; set; }
            public string Reference { get; set; }
            public string Info { get; set; }
            public string Value { get; set; }
        }
    }
}
