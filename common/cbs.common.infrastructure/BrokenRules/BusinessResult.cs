using System.Collections.Generic;

namespace cbs.common.infrastructure.BrokenRules;

public class BusinessResult
{
    public List<BusinessError> BrokenRules { get; } = new List<BusinessError>();
    public BusinessResultStatus Status => BrokenRules.Count > 0 ? BusinessResultStatus.Fail : BusinessResultStatus.Success;

    public virtual bool IsSuccess() => Status == BusinessResultStatus.Success;

    public void AddBrokenRule(BusinessError brokenRule) => BrokenRules.Add(brokenRule);

    public void AddBrokenRule(IEnumerable<BusinessError> brokenRules) => BrokenRules.AddRange(brokenRules);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="businessRule"></param>
    public static implicit operator BusinessResult(BusinessError businessRule) => SingleErrorResult(businessRule);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="businessRules"></param>
    public static implicit operator BusinessResult(List<BusinessError> businessRules) => ErrorResult(businessRules);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static BusinessResult SingleErrorResult(string message) => SingleErrorResult(BusinessError.CreateInstance(string.Empty, message));

    public static BusinessResult SingleErrorResult(string property, string message) => SingleErrorResult(BusinessError.CreateInstance(property, message));

    public static BusinessResult SingleErrorResult(BusinessError businessRule)
    {
        var result = new BusinessResult();
        result.AddBrokenRule(businessRule);

        return result;
    }
    public static BusinessResult ErrorResult(IEnumerable<BusinessError> businessRules)
    {
        var result = new BusinessResult();
        result.AddBrokenRule(businessRules);

        return result;
    }
}