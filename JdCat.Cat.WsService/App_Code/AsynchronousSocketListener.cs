using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JdCat.Cat.WsService.App_Code
{
    public class AsynchronousSocketListener
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {
        }

        public static void StartListening()
        {
            //Socket listener;
            //if (Environment.)
            //{

            //}
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8086);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8086);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(int.MaxValue);

                while (true)
                {
                    allDone.Reset();

                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {

            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            var handler = state.workSocket;

            if (!SocketState(handler))
            {
                int bytesRead = handler.EndReceive(ar);
                state.sb.Append(Encoding.UTF8.GetString(
                    state.buffer, 0, bytesRead));
                state.Id = int.Parse(state.sb.ToString());
                if (StateObject.DicSocket.ContainsKey(state.Id))
                {
                    Send(StateObject.DicSocket[state.Id], "802：因为另外一个连接进入，您被迫下线");
                    StateObject.DicSocket.Remove(state.Id);
                }
                StateObject.DicSocket.Add(state.Id, state);
            }
            else
            {
                StateObject.DicSocket.Remove(state.Id);
                state.workSocket.Dispose();
            }

        }

        public static void Send(StateObject state, string data)
        {
            if (SocketState(state.workSocket))
            {
                // 连接已断开
                StateObject.DicSocket.Remove(state.Id);
                state.workSocket.Dispose();
                return;
            }
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            state.workSocket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, state);
        }

        public static void SendCallback(IAsyncResult ar)
        {
            try
            {
                var state = (StateObject)ar.AsyncState;
                if (state.Msg.StartsWith("802"))
                {
                    state.workSocket.Shutdown(SocketShutdown.Both);
                    state.workSocket.Close();
                }
            }
            catch (Exception e)
            {

            }
        }
        private static bool SocketState(Socket socket)
        {
            return socket.Poll(10, SelectMode.SelectRead);
        }
    }
}
