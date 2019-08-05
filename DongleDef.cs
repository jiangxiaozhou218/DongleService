using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Dongle
{
    class DongleDef
    {

        public const uint CONST_PID = 0xFFFFFFFF;   //出厂时默认的PID

        public const string CONST_USERPIN = "12345678";  //出厂时默认的USERPIN
        public const string CONST_ADMINPIN = "FFFFFFFFFFFFFFFF"; //出厂时默认的ADMINPIN

        //加密锁协议类型
        public const uint PROTOCOL_HID = 0; //hid协议
        public const uint PROTOCOL_CCID = 1; //ccid协议

        //文件类型
        public const uint FILE_DATA = 1; //普通数据文件
        public const uint FILE_PRIKEY_RSA = 2; //RSA私钥文件
        public const uint FILE_PRIKEY_ECCSM2 = 3; //ECC或者SM2私钥文件(SM2私钥文件时ECC私钥文件的子集)
        public const uint FILE_KEY = 4; //SM4和3DES密钥文件
        public const uint FILE_EXE = 5; //可执行文件

        //LED灯状态定义
        public const uint LED_OFF = 0; //灯灭
        public const uint LED_ON = 1; //灯亮
        public const uint LED_BLINK = 2;//灯闪


        //PIN码类型
        public const uint FLAG_USERPIN = 0;//用户PIN
        public const uint FLAG_ADMINPIN = 1;//开发商PIN

        //加解密标志
        public const uint FLAG_ENCODE = 0;//加密
        public const uint FLAG_DECODE = 1; //解密

        //HASH算法类型
        public const uint FLAG_HASH_MD5 = 0; //MD5     运算结果16字节
        public const uint FLAG_HASH_SHA1 = 1;//SHA1    运算结果20字节
        public const uint FLAG_HASH_SM3 = 2; //SM3     运算结果32字节


        //远程升级的功能号
        public const uint UPDATE_FUNC_CreateFile = 1; //创建文件
        public const uint UPDATE_FUNC_WriteFile = 2; //写文件
        public const uint UPDATE_FUNC_DeleteFile = 3; //删除文件
        public const uint UPDATE_FUNC_FileLic = 4; //设置文件授权
        public const uint UPDATE_FUNC_SeedCount = 5; //设置种子码可运算次数
        public const uint UPDATE_FUNC_DownloadExe = 6; //升级可执行文件
        public const uint UPDATE_FUNC_UnlockUserPin = 7; //解锁用户PIN
        public const uint UPDATE_FUNC_Deadline = 8;//时钟锁升级使用期限


        //错误码
        public const uint DONGLE_SUCCESS = 0x00000000;            // 操作成功
        public const uint DONGLE_NOT_FOUND = 0xF0000001;          // 未找到指定的设备
        public const uint DONGLE_INVALID_HANDLE = 0xF0000002;     // 无效的句柄
        public const uint DONGLE_INVALID_PARAMETER = 0xF0000003;  // 参数错误
        public const uint DONGLE_COMM_ERROR = 0xF0000004;		   // 通讯错误
        public const uint DONGLE_INSUFFICIENT_BUFFER = 0xF0000005;// 缓冲区空间不足
        public const uint DONGLE_NOT_INITIALIZED = 0xF0000006;	   // 产品尚未初始化 (即没设置PID)
        public const uint DONGLE_ALREADY_INITIALIZED = 0xF0000007;// 产品已经初始化 (即已设置PID)
        public const uint DONGLE_ADMINPIN_NOT_CHECK = 0xF0000008; // 开发商密码没有验证
        public const uint DONGLE_USERPIN_NOT_CHECK = 0xF0000009;  // 用户密码没有验证
        public const uint DONGLE_INCORRECT_PIN = 0xF000FF00;	   // 密码不正确 (后2位指示剩余次数)
        public const uint DONGLE_PIN_BLOCKED = 0xF000000A;		   // PIN码已锁死
        public const uint DONGLE_ACCESS_DENIED = 0xF000000B;	   // 访问被拒绝 
        public const uint DONGLE_FILE_EXIST = 0xF000000E;		   // 文件已存在
        public const uint DONGLE_FILE_NOT_FOUND = 0xF000000F;	   // 未找到指定的文件
        public const uint DONGLE_READ_ERROR = 0xF0000010;         // 读取数据错误
        public const uint DONGLE_WRITE_ERROR = 0xF0000011;        // 写入数据错误
        public const uint DONGLE_FILE_CREATE_ERROR = 0xF0000012;  // 创建文件或文件夹错误
        public const uint DONGLE_FILE_READ_ERROR = 0xF0000013;    // 读取文件错误
        public const uint DONGLE_FILE_WRITE_ERROR = 0xF0000014;   // 写入文件错误
        public const uint DONGLE_FILE_DEL_ERROR = 0xF0000015;     // 删除文件或文件夹错误
        public const uint DONGLE_FAILED = 0xF0000016;             // 操作失败
        public const uint DONGLE_CLOCK_EXPIRE = 0xF0000017;       // 加密锁时钟到期
        public const uint DONGLE_ERROR_UNKNOWN = 0xFFFFFFFF;      // 未知的错误

    }
}
