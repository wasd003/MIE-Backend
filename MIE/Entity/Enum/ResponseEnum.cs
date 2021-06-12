﻿using System;
namespace MIE.Entity.Enum
{
    public class ResponseEnum
    {
        public int Code { get; set; }

        public string Msg { get; set; }

        public ResponseEnum(int code, string msg)
        {
            Code = code;
            Msg = msg;
        }

        /**
         * 身份认证模块相关异常
         * 返回码以1开头
         */
        public static ResponseEnum WrongUsernameOrPwd() 
            => new ResponseEnum(10000, "用户名或密码错误");

        public static ResponseEnum ShortPwdOrEmptyUsername()
            => new ResponseEnum(10001, "密码太短或者用户名为空");

        public static ResponseEnum DuplicateUser()
            => new ResponseEnum(10002, "用户已存在");

        public static ResponseEnum FailToRegister()
            => new ResponseEnum(10003, "注册失败，后台数据库相关错误");

        /**
         * 预约模块相关异常
         * 返回码以2开头
         */
        public static ResponseEnum NoAvailableTime()
            => new ResponseEnum(20000, "预约时间不存在");

        public static ResponseEnum WrongDateformat()
            => new ResponseEnum(20001, "日期格式不正确，正确格式为MM/dd/yyyy");
    }
}