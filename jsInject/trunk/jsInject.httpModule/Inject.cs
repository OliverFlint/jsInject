using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Configuration;
using System.IO;
using System.Web.UI.HtmlControls;

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
            context.PostMapRequestHandler += new EventHandler(OnPostMapRequestHandler);
        }

        void OnPostMapRequestHandler(object sender, EventArgs e)
        {
            _context = (HttpApplication)sender;
            Page page = HttpContext.Current.CurrentHandler as Page;
            if (page != null)
            {
                page.PreRenderComplete += new EventHandler(OnPreRenderComplete);
            }
        }

        public void OnPreRenderComplete(object sender, System.EventArgs args)
        {
            _jsInjectRoot = (string)ConfigurationManager.AppSettings["jsInjectRoot"];
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
                injectIntoHead(_jsFilename);
            }
        }

        private void injectFileNameRule()
        {
            string _fileName = VirtualPathUtility.GetFileName(_context.Request.Path);
            string _jsFilename = string.Format("{0}FileNames/{1}.js",_jsInjectRoot,_fileName);
            if (File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                injectIntoHead(_jsFilename);
            }
        }

        private void injectPathRule()
        {
            string _path = _context.Request.Path;
            string _jsFilename = string.Format("{0}Paths{1}.js",_jsInjectRoot,_path);
            if (File.Exists(_context.Request.MapPath(_jsFilename)))
            {
                injectIntoHead(_jsFilename);
            }
        }

        private void injectIntoHead(string jsFileName)
        {
            Page page = HttpContext.Current.CurrentHandler as Page;
            if (page != null)
            {
                if (page.Header != null)
                {
                    HtmlGenericControl Include = new HtmlGenericControl("script");
                    Include.Attributes.Add("type", "text/javascript");
                    Include.Attributes.Add("src", jsFileName);
                    page.Header.Controls.Add(Include);
                }
                else if (!page.ClientScript.IsClientScriptIncludeRegistered(page.GetType(), jsFileName))
                {
                    page.ClientScript.RegisterClientScriptInclude(page.GetType(), jsFileName, jsFileName);
                }
            }
        }
    }
}
