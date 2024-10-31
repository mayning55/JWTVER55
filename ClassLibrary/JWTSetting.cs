using System;

namespace ClassLibrary;

public class JWTSetting
{
    public string SigningKey{ get; set; }
    public int ExpireSeconds{get; set; }
    public string Issuer{ get; set; }
    public string  Audience{ get; set; }

}
