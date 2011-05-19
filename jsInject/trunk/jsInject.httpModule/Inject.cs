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
            _jsInjectRoot = null;
            _context.Dispose();
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(OnEndRequest);
        }

        public void OnEndRequest(object sender, System.EventArgs args)
        {
            _context = (HttpApplication)sender;
            _jsInjectRoot = (string)ConfigurationSettings.AppSettings["jsInjectRoot"];
            injectFileExtensionRule();
            injectFileNameRule();
            injectPathRule();
        }

        private void injectFileExtensionRule()
        {
            string _fileExtension = VirtualPathUtility.GetExtension(_context.Request.FilePath);
            string _jsFilename = string.Format("{0}Extensions/{1}.js", _jsInjectRoot, _fileExtension);
            if(File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                _context.Response.Write("<script language='javascript' src='" + _jsFilename + "' ></script>");
            }
        }

        private void injectFileNameRule()
        {
            string _fileName = VirtualPathUtility.GetFileName(_context.Request.Path);
            string _jsFilename = string.Format("{0}FileNames/{1}.js",_jsInjectRoot,_fileName);
            if (File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                _context.Response.Write("<script language='javascript' src='" + _jsFilename + "' ></script>");
            }
        }

        private void injectPathRule()
        {
            string _path = _context.Request.Path;
            string _jsFilename = string.Format("{0}Paths{1}.js",_jsInjectRoot,_path);
            if (File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                _context.Response.Write("<script language='javascript' src='" + _jsFilename + "' ></script>");
            }
        }
    }
}
