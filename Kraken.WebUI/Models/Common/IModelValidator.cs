using System.Collections.Generic;

namespace Kraken.WebUI.Models.Common
{
    public interface IModelValidator<T>
    {
        IEnumerable<string> Validate(T model);
    }
}
