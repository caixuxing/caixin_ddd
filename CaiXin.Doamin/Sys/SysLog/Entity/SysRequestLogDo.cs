using CaiXin.Domain.Comm;
using CaiXin.Domain.SysUser.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Domain.Sys.SysLog.Entity;

/// <summary>
/// 系统请求日志
/// </summary>
[SugarTable("sys_request_log", "系统请求日志")]
public partial record SysRequestLogDo : AggRoot.EntityBase
{
  

    /// <summary>
    /// 
    /// </summary>
    [SugarColumn(ColumnDescription = "UrlPath", Length = 200)]
    public string UrlPath { get; private set; } = default!;

    [SugarColumn(ColumnDescription = "UrlPath", Length = 200,IsNullable =true)]
    public string? ParameterValues { get; private set; }
    /// <summary>
    /// 控制器
    /// </summary>
    [SugarColumn(ColumnDescription = "控制器", Length = 50)]
    public string ControllerName { get; private set; } = default!;
    /// <summary>
    /// 方法名
    /// </summary>
    [SugarColumn(ColumnDescription = "方法名", Length = 50)]
    public string ActionName { get; private set; } = default!;
    /// <summary>
    /// 请求类型
    /// </summary>
    [SugarColumn(ColumnDescription = "请求类型", Length = 10)]
    public string RequestMethod { get; private set; } = default!;
    /// <summary>
    /// 服务器环境
    /// </summary>
    [SugarColumn(ColumnDescription = "服务器环境", Length = 50)]
    public string EnvironmentName { get; private set; } = default!;
    /// <summary>
    /// 完成情况
    /// </summary>
    [SugarColumn(ColumnDescription = "完成情况")]
    public bool IsSuccess { get; private set; } = default!;
    /// <summary>
    /// 执行耗时
    /// </summary>
    [SugarColumn(ColumnDescription = "执行耗时")]
    public long ElapsedTime { get; private set; } = default!;
    /// <summary>
    /// 客户端IP
    /// </summary>
    [SugarColumn(ColumnDescription = "客户端IP", Length = 20)]
    public string ClientIp { get; private set; } = default!;

    /// <summary>
    /// 异常信息
    /// </summary>
    [SugarColumn(ColumnDescription = "异常信息", Length = 200,IsNullable =true)]
    public string? ExceptionMsg { get; private set; }
    /// <summary>
    /// 响应结果
    /// </summary>

    [SugarColumn(ColumnDescription = "响应结果", Length = 200,IsNullable =true)]
    public string? ResponseData { get; private set; }
}

public partial record SysRequestLogDo
{
    /// <summary>
    /// 无参构造
    /// </summary>
    public SysRequestLogDo() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="urlPath"></param>
    /// <param name="parameterValues"></param>
    /// <param name="controllerName"></param>
    /// <param name="actionName"></param>
    /// <param name="requestMethod"></param>
    /// <param name="environmentName"></param>
    /// <param name="isSuccess"></param>
    /// <param name="elapsedTime"></param>
    /// <param name="clientIp"></param>
    /// <param name="exceptionMsg"></param>
    /// <param name="responseData"></param>
    private SysRequestLogDo(string urlPath, string parameterValues, string controllerName, string actionName, string requestMethod, string environmentName, bool isSuccess, long elapsedTime, string clientIp, string exceptionMsg, string responseData)
    {
        UrlPath = urlPath;
        ParameterValues = parameterValues;
        ControllerName = controllerName;
        ActionName = actionName;
        RequestMethod = requestMethod;
        EnvironmentName = environmentName;
        IsSuccess = isSuccess;
        ElapsedTime = elapsedTime;
        ClientIp = clientIp;
        ExceptionMsg = exceptionMsg;
        ResponseData = responseData;
    }


    public static SysRequestLogDo Create(string urlPath, string parameterValues, string controllerName,
        string actionName, string requestMethod, string environmentName, bool isSuccess,
        long elapsedTime, string clientIp, string exceptionMsg, string responseData)
    => new(urlPath, parameterValues, controllerName, actionName, requestMethod, environmentName,
            isSuccess, elapsedTime, clientIp, exceptionMsg, responseData);


    public SysRequestLogDo SetUrlPath(string urlPath) {

        this.UrlPath = urlPath;
        return this;
    }
    public SysRequestLogDo SetParameterValues(string parameterValues) => CurrentObj<SysRequestLogDo>.SetValue(x => ParameterValues = x, parameterValues);
    public SysRequestLogDo SetControllerName(string controllerName) => CurrentObj<SysRequestLogDo>.SetValue(x => ControllerName = x, controllerName);
    public SysRequestLogDo SetActionName(string v) => CurrentObj<SysRequestLogDo>.SetValue(x => ActionName = x, v);
    public SysRequestLogDo SetRequestMethod(string v) => CurrentObj<SysRequestLogDo>.SetValue(x => RequestMethod = x, v);
    public SysRequestLogDo SetEnvironmentName(string v) => CurrentObj<SysRequestLogDo>.SetValue(x => EnvironmentName = x, v);
    public SysRequestLogDo SetIsSuccess(bool v) => CurrentObj<SysRequestLogDo>.SetValue(x => IsSuccess = x, v);
    public SysRequestLogDo SetElapsedTime(long v) => CurrentObj<SysRequestLogDo>.SetValue(x => ElapsedTime = x, v);
    public SysRequestLogDo SetClientIp(string v) => CurrentObj<SysRequestLogDo>.SetValue(x => ClientIp = x, v);
    public SysRequestLogDo SetExceptionMsg(string v) => CurrentObj<SysRequestLogDo>.SetValue(x => ExceptionMsg = x, v);
    public SysRequestLogDo SetResponseData(string v) => CurrentObj<SysRequestLogDo>.SetValue(x => ResponseData = x, v);

}
