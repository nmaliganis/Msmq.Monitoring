using System.Collections.Generic;

namespace cbs.common.infrastructure.BrokenRules;

public class BusinessResult<TModel> : BusinessResult
{
    public TModel Model { get; set; }
    public object ExtraProp { get; set; }

    protected BusinessResult() { }

    public BusinessResult(TModel model) => Model = model;

    public static implicit operator BusinessResult<TModel>(TModel model) => new BusinessResult<TModel>(model);

    public static implicit operator BusinessResult<TModel>(BusinessError businessRule) =>
        SingleErrorResult(businessRule);

    public static implicit operator BusinessResult<TModel>(List<BusinessError> businessRules) =>
        ErrorResult(businessRules);

    public new static BusinessResult<TModel> SingleErrorResult(string message) =>
        SingleErrorResult(BusinessError.CreateInstance(string.Empty, message));

    public new static BusinessResult<TModel> SingleErrorResult(string property, string message) =>
        SingleErrorResult(BusinessError.CreateInstance(property, message));

    public new static BusinessResult<TModel> SingleErrorResult(BusinessError businessRule)
    {
        var result = new BusinessResult<TModel>();
        result.AddBrokenRule(businessRule);

        return result;
    }

    public new static BusinessResult<TModel> ErrorResult(IEnumerable<BusinessError> businessRules)
    {
        var result = new BusinessResult<TModel>();
        result.AddBrokenRule(businessRules);

        return result;
    }
}