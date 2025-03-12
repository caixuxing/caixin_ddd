using CaiXin.Domain.Comm;
using SqlSugar;
using System.Security.Cryptography;
using System.Text;

namespace CaiXin.Domain.SysUser.Entity;


/// <summary>
/// 系统用户
/// </summary>

[SugarTable("sys_user", "系统用户")]
public partial record UserDo : AggRoot.EntityBase
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public UserDo() { }

    /// <summary>
    /// 用户名称
    /// </summary>
    [SugarColumn(ColumnDescription = "用户名称", Length = 10)]
    public string Name { get; private set; } = default!;

    /// <summary>
    /// 账号
    /// </summary>
    [SugarColumn(ColumnDescription = "账号", Length = 50)]
    public string Account { get; private set; } = default!;

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(ColumnDescription = "账号", Length = 100)]
    public string Password { get; private set; } = default!;

    /// <summary>
    /// 邮箱地址
    /// </summary>
    [SugarColumn(ColumnDescription = "账号", Length = 50)]
    public string Email { get; private set; } = default!;


    /// <summary>
    /// 人信息
    /// </summary>
    [SugarColumn(ColumnDescription = "人信息", IsNullable =true, ColumnDataType = "text")]
    public string PersonInfo { get; set; }

}


/// <summary>
/// 系统用户功能
/// </summary>
public partial record UserDo
{
    private UserDo(string name, string password, string email)
    {
        Name = name;
        Password = password;
        Email = email;
    }


    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="passWord">密码</param>
    /// <param name="email">邮箱</param>
    /// <returns>用户对象实体</returns>
    public static UserDo CreateUser(string name, string password, string email)
    {
        return new(name, password, email);
    }

    /// <summary>
    /// 设置名称
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public UserDo SetName(string name)
    {
        this.Name = name;
        return this;
    }

    /// <summary>
    /// 设置邮箱
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public UserDo SetEmail(string email)
    {
        this.Email = email;
        return this;
    }

    /// <summary>
    /// 设置密码
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public UserDo SetPassword(string password)
    {
        this.Password = CalculateMD5Hash(password);
        return this;
    }


    public UserDo SetAccount(string account)
    {
        this.Account = account;
        return this;
    }

    public UserDo SetPersonInfo(string personInfo)
    {
        this.PersonInfo = personInfo;
        return this;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string CalculateMD5Hash(string input)
    {
        // 创建一个 MD5 实例
        using (MD5 md5 = MD5.Create())
        {
            // 将输入字符串转换为字节数组
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            // 计算输入字节数组的 MD5 哈希值
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            // 将字节数组转换为十六进制字符串
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                // 每个字节转换为两位十六进制字符串
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}