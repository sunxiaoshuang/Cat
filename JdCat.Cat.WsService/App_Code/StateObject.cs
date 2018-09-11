using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.Cat.WsService.App_Code
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        public static Dictionary<int, StateObject> DicSocket = new Dictionary<int, StateObject>();
        public Socket workSocket = null;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
        public int Id { get; set; }
        public string Msg { get; set; }
    }
}
