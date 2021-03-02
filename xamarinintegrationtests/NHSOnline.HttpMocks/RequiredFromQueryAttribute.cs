using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace NHSOnline.HttpMocks
{
    internal sealed class RequiredFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
    {
        public void Apply(ParameterModel parameter)
        {
            if (parameter.Action.Selectors?.Any() == true)
            {
                var queryStringParameterName = parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName;
                var constraint = new RequiredFromQueryActionConstraint(queryStringParameterName);
                parameter.Action.Selectors[^1].ActionConstraints.Add(constraint);
            }
        }
    }
}