﻿using FluentValidation;
namespace CaiXin.Application.Contracts.Validate;


/// <summary>
/// 
/// </summary>
public static class ValidateExten
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="validator"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task ValidateAndThrowAsync<T>(this IValidator<T> validator, T dto)
    {
        // 调用验证器的验证方法
        var validateResult = await validator.ValidateAsync(dto);
        if (!validateResult.IsValid)
        {
            // 提取验证错误信息
            var errorList = validateResult.Errors.Select(_ => new { key = _.PropertyName, msg = _.ErrorMessage }).ToList();
            // 抛出异常，使用第一个错误信息
            throw new InvalidOperationException($"{errorList.FirstOrDefault()?.msg ?? string.Empty}");
        }
    }
}
