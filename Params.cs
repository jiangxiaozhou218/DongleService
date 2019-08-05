using System;
using System.Collections.Generic;
using System.Text;

namespace HongshiConsoleApp
{
    class Params
    {
        public string paramsString { get; set; }
    }
    class ParamsList
    {
        public List<string> paramsString { get; set; }
    }
    class DecryptParamsList
    {
        public List<List<string>> paramsString { get; set; }
    }
    class UserParams
    {
        public string code { get; set; }
        public string name { get; set; }
    }
    class DataInfo
    {
        public int Code { get; set; }
        public object Data { get; set; }
    }
    class DogInfo
    {
        public string HId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    class DecryptInfo
    {
        public string Decrypt { get; set; }
    }
    class DecryptList
    {
        public List<List<string>> Decrypt { get; set; }
    }
    class EncrypInfo
    {
        public string Encryp { get; set; }
    }
    class EncrypList
    {
        public List<string> Encryp { get; set; }
    }
}
