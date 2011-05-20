using System;
using System.IO;
using System.Web;
using System.Configuration;

namespace jsInject.httpModule
{
    public class Inject : IHttpModule
    {
        private string _jsInjectRoot { get; set; }
        private HttpApplication _context { get; set; }

        public void Dispose()
        {
            //
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OnBeginRequest);
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            _context = (HttpApplication)sender;

            if (_context.Context.Response.ContentType == "text/html")
            {
                _jsInjectRoot = (string)ConfigurationManager.AppSettings["jsInjectRoot"];
                string injectBefore = (string)ConfigurationManager.AppSettings["jsInjectBefore"];
                string _stringToInject = string.Format("{0}{1}{2}", GetFileExtensionRule(), GetFileNameRule(), GetPathRule());
                _context.Context.Response.Filter = new jsInjectFilter(_context.Context.Response.Filter, _stringToInject, injectBefore);
            }
        }

        private string GetFileExtensionRule()
        {
            string _fileExtension = VirtualPathUtility.GetExtension(_context.Request.FilePath);
            string _jsFilename = string.Format("{0}Extensions/{1}.js", _jsInjectRoot, _fileExtension);
            string _returnString = GetHelperText("FileExtension", _jsFilename, _context.Request.Path);
            if (File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                _returnString += string.Format("\r\n<script language='javascript' src='{0}' ></script>", _jsFilename);
            }
            else
            {
                _returnString += string.Empty;
            }
            return _returnString;
        }

        private string GetFileNameRule()
        {
            string _fileName = VirtualPathUtility.GetFileName(_context.Request.Path);
            string _jsFilename = string.Format("{0}FileNames/{1}.js", _jsInjectRoot, _fileName);
            string _returnString = GetHelperText("FileName", _jsFilename, _context.Request.Path);
            if (File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                _returnString += string.Format("\r\n<script language='javascript' src='{0}' ></script>", _jsFilename);
            }
            else
            {
                _returnString += string.Empty;
            }
            return _returnString;
        }

        private string GetPathRule()
        {
            string _path = _context.Request.Path;
            string _jsFilename = string.Format("{0}Paths{1}.js", _jsInjectRoot, _path);
            string _returnString = GetHelperText("Path", _jsFilename, _context.Request.Path);
            if (File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                _returnString += string.Format("\r\n<script language='javascript' src='{0}' ></script>", _jsFilename);
            }
            else
            {
                _returnString += string.Empty;
            }
            return _returnString;
        }

        private string GetHelperText(string rule, string jsFileName, string requestPath)
        {
            return string.Format("\r\n<!-- jsInject rule='{0}' jsFileName='{1}' requestPath='{2}' -->", rule, jsFileName, requestPath);
        }
    }
}
