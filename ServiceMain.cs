using Dongle;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HongshiConsoleApp
{
    class ServiceMain
    {
        HttpListener httpobj;
        object bDongleInfo = null;
        uint uiCount = 0;//Dongle数量
        uint uiError = 0;//错误码
        uint uiHandle = 0;//加密锁句柄
        int reOpenCount = 0;//重新读锁次数
        byte[] bKey = null;
        Dongle.DongleClass ryArmDongle = new Dongle.DongleClass();
        StreamWriter sw;
        FileStream fs;
        DateTime dt;
        Hashtable ht = new Hashtable();
        public void start()
        {
            initLogfile();
            initDongle();
            initHttpListener();
        }
        public void stop()
        {
            httpobj.Close();
        }
        private void initLogfile()
        {
            string sCurrentDir = System.Threading.Thread.GetDomain().BaseDirectory;
            string strFilePath = sCurrentDir + "log/log_error.txt";
            if (!Directory.Exists(sCurrentDir + "log"))
            {
                Directory.CreateDirectory(sCurrentDir + "log");
            }
            if (!File.Exists(strFilePath))
            {
                FileStream createfs = File.Create(strFilePath);
                createfs.Close();
            }
            fs = new FileStream(strFilePath, FileMode.Append);
            sw = new StreamWriter(fs, Encoding.UTF8);
        }
        private void initDongle()
        {

            ryArmDongle.Enum(ref bDongleInfo, ref uiCount, ref uiError);
            
            uint uiRemainCount = 0;

            uint uiIndex = 0;//序号


            //打开加密锁
            ryArmDongle.Open(ref uiHandle, uiIndex, ref uiError);
            if (uiError != DongleDef.DONGLE_SUCCESS)
            {
                Console.WriteLine(string.Format("打开锁失败。 错误码：{0:X8}\r\n", uiError));
                sw.WriteLine(DateTime.Now.ToString() + string.Format("：打开锁失败。 错误码：{0:X8}", uiError));
                sw.Flush();
                return;
            }

            //验证开发商密码
            ryArmDongle.VerifyPIN(uiHandle, DongleDef.FLAG_ADMINPIN, DongleDef.CONST_ADMINPIN, ref uiRemainCount, ref uiError);
            if (uiError != DongleDef.DONGLE_SUCCESS)
            {
                Console.WriteLine(string.Format("校验开发商PIN失败。 错误码：{0:X8}\r\n", uiError));
                sw.WriteLine(DateTime.Now.ToString() + string.Format("：校验开发商PIN失败。 错误码：{0:X8}", uiError));
                sw.Flush();
                return;
            }
            readKey();

        }
        private void initHttpListener()
        {
            //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
            httpobj = new HttpListener();
            //定义url及端口号，通常设置为配置文件
            httpobj.Prefixes.Add("http://127.0.0.1:2965/");
            //启动监听器
            httpobj.Start();
            //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
            //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
            httpobj.BeginGetContext(Result, null);
            Console.WriteLine($"服务已启动，时间：{ DateTime.Now.ToString()}\r\n");
            sw.WriteLine($"服务已启动，时间：{ DateTime.Now.ToString()}");
            sw.Flush();
            Console.Read();
        }
        private void Result(IAsyncResult ar)
        {
            reOpenCount = 0;
            //当接收到请求后程序流会走到这里
            //Console.Clear();
            //继续异步监听
            httpobj.BeginGetContext(Result, null);
            Console.ForegroundColor = ConsoleColor.White;
            //获得context对象
            var context = httpobj.EndGetContext(ar);
            var request = context.Request;
            var response = context.Response;
            Console.WriteLine($"接到新的请求:{request.RawUrl},时间：{DateTime.Now.ToString()}");
            //如果是js的ajax请求，还可以设置跨域的ip地址与参数
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");//后台跨域请求，通常设置为配置文件
            context.Response.AppendHeader("Access-Control-Allow-Headers", "Content-Type, Content-Length, Authorization, Accept, X-Requested-With , yourHeaderFeild");//后台跨域参数设置，通常设置为配置文件
            context.Response.AppendHeader("Access-Control-Allow-Method", "PUT,POST,GET,DELETE,OPTIONS");//后台跨域请求设置，通常设置为配置文件
            context.Response.ContentType = "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
            context.Response.AddHeader("Content-type", "application/json");//添加响应头信息
            context.Response.ContentEncoding = Encoding.UTF8;
            DataInfo returnObj = null;//定义返回客户端的信息
            if (request.HttpMethod == "POST" && request.InputStream != null)
            {
                TimeSpan d3 = DateTime.Now.Subtract(dt);
                if (d3.TotalMinutes >= 1)
                {
                    bKey = null;
                    readKey();
                }
                //处理客户端发送的请求并返回处理信息
                returnObj = HandleRequest(request, response);
            }
            else
            {
                returnObj = null;// $"不是post请求或者传过来的数据为空";
            }
            var returnByteArr = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(returnObj));//设置客户端返回信息的编码
            try
            {
                using (var stream = response.OutputStream)
                {
                    //把处理信息返回到客户端
                    stream.Write(returnByteArr, 0, returnByteArr.Length);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"网络蹦了：{ex.ToString()}");
                sw.WriteLine($"{ DateTime.Now.ToString()}：网络蹦了：{ex.ToString()}");
                sw.Flush();
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"请求处理完成：{request.RawUrl},时间：{ DateTime.Now.ToString()}\r\n");
        }

        private DataInfo HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string data = null;
            try
            {
                var byteList = new List<byte>();
                var byteArr = new byte[2048];
                int readLen = 0;
                int len = 0;
                //接收客户端传过来的数据并转成字符串类型
                do
                {
                    readLen = request.InputStream.Read(byteArr, 0, byteArr.Length);
                    len += readLen;
                    byteList.AddRange(byteArr);
                } while (readLen != 0);
                data = Encoding.UTF8.GetString(byteList.ToArray(), 0, len);

                //获取得到数据data可以进行其他操作
            }
            catch (Exception ex)
            {
                response.StatusDescription = "404";
                response.StatusCode = 404;
                Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"在接收数据时发生错误:{ex.ToString()}");
                return null;//把服务端错误信息直接返回可能会导致信息不安全，此处仅供参考
            }
            response.StatusDescription = "200";//获取或设置返回给客户端的 HTTP 状态代码的文本说明。
            response.StatusCode = 200;// 获取或设置返回给客户端的 HTTP 状态代码。
            Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine($"接收数据完成:{data.Trim()},时间：{DateTime.Now.ToString()}");
            if (request.RawUrl == "/getDogInfo")
            {
                return getDogInfo();
            }
            else if (request.RawUrl == "/encryp")
            {
                ParamsList param = JsonConvert.DeserializeObject<ParamsList>(data);
                return encryp(param);
            }
            else if (request.RawUrl == "/decrypt")
            {
                DecryptParamsList param = JsonConvert.DeserializeObject<DecryptParamsList>(data);
                return decrypt(param);
            }
            else
            {
                DataInfo datainfo = new DataInfo();
                datainfo.Code = 0;
                datainfo.Data = "";
                return datainfo;
            }
        }
        private bool reInit()
        {
            if (reOpenCount == 0)
            {
                initDongle();
                reOpenCount++;
                return true;                    
            }
            return false;
        }
        private DataInfo getDogInfo()
        {
            uint uiVer = 0; //版本号
            uint uiAgent = 0; //代理编号
            uint uiPID = 0;  //产品ID
            uint uiUID = 0;  //用户ID
            uint uiDevType = 0;//设备类型
            uint uiIsMother = 0; //母锁标识
            uint uiType = 0; //产品类型
            object vHID = null; //HID，硬件序列号
            object vBirthday = null; //生产日期

            ryArmDongle.GetDongleInfo(bDongleInfo, 0, ref uiVer, ref vBirthday,
                                          ref uiAgent, ref uiPID, ref uiUID, ref vHID,
                                          ref uiDevType, ref uiIsMother, ref uiType);
            byte[] hid = (byte[])vHID;
            string shid = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}", hid[0], hid[1], hid[2], hid[3]
                    , hid[4], hid[5], hid[6], hid[7]);

            DataInfo datainfo = new DataInfo();
            datainfo.Code = 0;
            DogInfo dogInfo = new DogInfo();
            dogInfo.HId = shid;
            //读用户信息文件
            ushort usFileID = 0x1112;
            ushort usReadFileID = 0;
            object vFileListBuffer = null;
            uint uiFileListBufferLen = 0;
            ryArmDongle.ListFile(uiHandle, DongleDef.FILE_DATA, ref vFileListBuffer, ref uiFileListBufferLen, ref uiError);
            if (uiError != DongleDef.DONGLE_SUCCESS)
            {
                if (reInit())
                {
                    getDogInfo();
                }
                else
                {
                    return null;
                }
            }
            int i = 0;
            uint uiSize = 1024;//文件大小
            ushort usRead = 0;//读权限
            ushort usWrite = 1;//写权限
            while (true)
            {
                object vFileAttr = null;
                usReadFileID = 0;
                ryArmDongle.Convert_Buffer_To_FILE_LIST(vFileListBuffer, uiFileListBufferLen, (uint)i, DongleDef.FILE_DATA, ref usReadFileID, ref vFileAttr, ref uiError);
                if (uiError == DongleDef.DONGLE_INVALID_PARAMETER)
                {
                    break;
                }
                if (usReadFileID == usFileID)
                {
                    ryArmDongle.Convert_Buffer_To_DATA_FILE_ATTR(vFileAttr, ref uiSize, ref usRead, ref usWrite, ref uiError);
                    break;
                }
                i++;
            }

            object vData = null;
            ryArmDongle.ReadFile(uiHandle, usFileID, 0, ref vData, uiSize, ref uiError);
            string sInfo = RUtils.ByteToString((byte[])vData);
            string[] aInfo = sInfo.Split(';');
            if (aInfo.Length == 2)
            {
                dogInfo.Code = aInfo[0];
                dogInfo.Name = aInfo[1];
            }

            datainfo.Data = dogInfo;
            return datainfo;
        }
        //加密
        private DataInfo encryp(ParamsList param)
        {
            EncrypList encrypList = new EncrypList();
            encrypList.Encryp = new List<string>();
            foreach (string str in param.paramsString)
            {
                if (str != "" && str != null)
                {
                    string s = RUtils.Encrypt(bKey, str);
                    encrypList.Encryp.Add(s);
                } else
                {
                    encrypList.Encryp.Add("");
                }
            }
            DataInfo datainfo = new DataInfo();
            datainfo.Code = 0;
            datainfo.Data = encrypList;
            return datainfo;

        }
        //解密
        private DataInfo decrypt(DecryptParamsList param)
        {
            DecryptList decryptInfo = new DecryptList();
            decryptInfo.Decrypt = param.paramsString;
            Parallel.For(0, decryptInfo.Decrypt.Count, (i) =>
            {
                List<string> strs = decryptInfo.Decrypt[(int)i];
                Parallel.For(0, strs.Count, (j) =>
                {
                    string str = strs[(int)j];
                    if (str != "" && str != null)
                    {
                        if (ht[str] == null)
                        {
                            string dStr = RUtils.Decrypt(bKey, str);
                            decryptInfo.Decrypt[(int)i][(int)j] = dStr;
                            ht[str] = dStr;
                        }
                        else
                        {
                            decryptInfo.Decrypt[(int)i][(int)j] = (string)ht[str];
                        }
                    }
                    else
                    {
                        decryptInfo.Decrypt[(int)i][(int)j] = "";
                    }
                });

            });
            DataInfo datainfo = new DataInfo();
            datainfo.Code = 0;
            datainfo.Data = decryptInfo;
            return datainfo;
        }
        private void readKey()
        {
            DataInfo datainfo = new DataInfo();
            ushort usFileID = 0x1111;
            object vData = null;
            ryArmDongle.ReadFile(uiHandle, usFileID, 0, ref vData, 8, ref uiError);
            if (uiError != DongleDef.DONGLE_SUCCESS)
            {
                dt = new DateTime();
                if (reInit())
                {
                    readKey();
                }
                return;
            }
            bKey = (byte[])vData;
            dt = DateTime.Now;
        }
    }
}
