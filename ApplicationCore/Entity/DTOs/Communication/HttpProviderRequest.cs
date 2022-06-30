using System.Collections.Generic;

namespace Farsica.Template.Entity.DTOs.Communication
{
    public class HttpProviderRequest<T>
    {
        public string BaseAddress { get; set; }
        public string Uri { get; set; }
        public IReadOnlyList<(string Key, string Value)> HeaderParameters { get; set; }
        public T Body { get; set; }
    }
}
