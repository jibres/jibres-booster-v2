﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using JibresBooster1.translation;

namespace JibresBooster1.lib
{
    class listener
    {

        public static readonly string JibresLocalServer = "http://localhost:9759/jibres/";
        static HttpListener myListener = new HttpListener();
        public static void runListener()
        {
            try
            {
                log.save("Starting server...");

                // add prefix
                myListener.Prefixes.Add(JibresLocalServer);
                myListener.Prefixes.Add("http://127.0.0.1:9759/jibres/");
                myListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                // start server (Run application as Administrator!)
                myListener.Start();

                // save log
                log.save("Server started.");
                notif.appStart();

                // start the response thread
                Thread _responseThread = new Thread(ResponseThread);
                _responseThread.Start();
            }
            catch (Exception e)
            {
                notif.appErrorServer();
                log.save("Error on running program! " + e.Message);
            }
        }


        static void ResponseThread()
        {
            while (true)
            {
                HttpListenerContext myContext = myListener.GetContext();
                HttpListenerRequest myRequest = myContext.Request;
                HttpListenerResponse myResponse = myContext.Response;
                var myData = new StreamReader(myRequest.InputStream, myRequest.ContentEncoding).ReadToEnd();
                //using System.Web and Add a Reference to System.Web
                Dictionary<string, string> postParams = new Dictionary<string, string>();


                // generate response and close connection
                var jsonResult = "{\"okay\":true, \"status\":200}";
                byte[] _responseArray = Encoding.UTF8.GetBytes(jsonResult);
                try
                {
                    // write bytes to the output stream
                    myResponse.OutputStream.Write(_responseArray, 0, _responseArray.Length);
                    // set the KeepAlive bool to false
                    myResponse.KeepAlive = false;
                    // allow cross origin
                    myResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                    // allow post and get request
                    myResponse.Headers.Add("Access-Control-Allow-Methods", "POST, GET");
                    // disable cache in header
                    myResponse.AddHeader("Cache-Control", "no-cache");
                    // set header as json
                    myResponse.ContentType = "application/json";
                    // set status
                    myResponse.StatusCode = 200;
                    // set status desc
                    myResponse.StatusDescription = "Okay";
                    // close the connection
                    myResponse.Close();
                }
                catch
                {
                    log.save("Error on generate response!");
                }

                if (myRequest.HttpMethod == "GET")
                {
                    log.save(string.Concat(Enumerable.Repeat("-", 50)) + " Get detected");

                    var getParams = myRequest.Url.Query.ToString();
                    if(getParams.Length > 0)
                    {
                        getParams = getParams.Substring(1);
                    }

                    myData = getParams;
                }
                else if (myRequest.HttpMethod == "POST")
                {
                    // if user post something try to do something
                    log.save(string.Concat(Enumerable.Repeat("-", 50)) + " Post detected");
                }



                // Here i can read all parameters in string but how to parse each one i don't know
                if (myData == "")
                {
                    log.save("Request without data !!!");
                }
                else
                {
                    log.save(myData.ToString());
                    Console.WriteLine(myData.ToString());

                    string[] myDataParams = myData.Split('&');
                    foreach (string param in myDataParams)
                    {
                        string[] mytmpStr = param.Split('=');
                        string key = mytmpStr[0];
                        string value = System.Web.HttpUtility.UrlDecode(mytmpStr[1]);
                        postParams.Add(key, value);
                    }

                    if (postParams.ContainsKey("type"))
                    {
                        log.save("Request type is " + postParams["type"]);

                        if (postParams["type"] == "PcPosKiccc")
                        {
                            lib.PcPos.JibresKiccc myKiccc = new PcPos.JibresKiccc();
                            myKiccc.fire(postParams);
                        }
                        else if (postParams["type"] == "PcPosAsanpardakht")
                        {
                            var myAsanPardakht = new PcPos.Asnapardakht();
                            myAsanPardakht.fire(postParams);
                        }
                    }

                }

            }
        }


    }
}
