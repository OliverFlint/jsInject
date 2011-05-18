using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Configuration;
using System.IO;

namespace jsInject.httpModule
{
    public class Inject : IHttpModule
    {
        private string _jsInjectRoot {get; set;}
        private HttpApplication _context { get; set; }

        public void Dispose()
        {
            //Dispose stuff here...
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(OnEndRequest);
        }

        public void OnEndRequest(object sender, System.EventArgs args)
        {
            HttpApplication context = (HttpApplication)sender;
            _context = context;
            _jsInjectRoot = (string)ConfigurationSettings.AppSettings["jsInjectRoot"];
            injectFileExtensionRule();
            injectFileNameRule();
            injectPathRule();
        }

        private void injectFileExtensionRule()
        {
            string fileExtension = VirtualPathUtility.GetExtension(_context.Request.FilePath);
            string _jsFilename = _jsInjectRoot + "Extensions/" + fileExtension + ".js";
            if(File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                _context.Response.Write("<script language='javascript' src='" + _jsFilename + "' ></script>");
            }
        }

        private void injectFileNameRule()
        {
            string fileName = VirtualPathUtility.GetFileName(_context.Request.Path);
            string _jsFilename = _jsInjectRoot + "FileNames/" + fileName + ".js";
            if (File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                _context.Response.Write("<script language='javascript' src='" + _jsFilename + "' ></script>");
            }
        }

        private void injectPathRule()
        {
            string path = _context.Request.Path;
            string _jsFilename = _jsInjectRoot + "Paths" + path + ".js";
            //_context.Response.Write(_jsFilename);
            if (File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                _context.Response.Write("<script language='javascript' src='" + _jsFilename + "' ></script>");
            }
        }
    }
}
